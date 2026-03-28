using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


// ==========================================
// 2. 烘焙器 (掛在 Unity 場景的空物件上)
// ==========================================
public class MultiSpawnerAuthoring : MonoBehaviour
{
    [Header("觀眾模型清單")]
    [Tooltip("請在這裡放入各種不同的觀眾 Prefab (必須是 Project 裡的藍色 Prefab)")]
    public List<GameObject> spectatorPrefabs = new List<GameObject>();
    
    [Header("生成設定")]
    public int spawnCount = 1000;
    public float spawnRadius = 20f;
    public float scaleFactor = 1.0f; 

    class Baker : Baker<MultiSpawnerAuthoring>
    {
        public override void Bake(MultiSpawnerAuthoring authoring)
        {
            // 防呆：如果清單是空的，就不建立資料
            if (authoring.spectatorPrefabs == null || authoring.spectatorPrefabs.Count == 0) return;

            Entity entity = GetEntity(TransformUsageFlags.None);
            
            // 寫入生成器的基本數值
            AddComponent(entity, new MultiSpawnerData
            {
                SpawnCount = authoring.spawnCount,
                SpawnRadius = authoring.spawnRadius,
                CenterPosition = authoring.transform.position,
                ScaleFactor = authoring.scaleFactor
            });

            // 建立動態清單 Buffer
            DynamicBuffer<MultiPrefabElement> prefabBuffer = AddBuffer<MultiPrefabElement>(entity);

            // 將 Unity 的 List 轉入 ECS 的 Buffer 裡面
            foreach (GameObject prefab in authoring.spectatorPrefabs)
            {
                if (prefab != null)
                {
                    prefabBuffer.Add(new MultiPrefabElement
                    {
                        PrefabEntity = GetEntity(prefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }

#if UNITY_EDITOR
    // 在編輯器場景中畫出紅色的生成範圍圓圈
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, spawnRadius);
        Handles.color = new Color(1f, 0f, 0f, 0.1f); 
        Handles.DrawSolidDisc(transform.position, Vector3.up, spawnRadius);
    }
#endif
}

// ==========================================
// 3. 安全生成系統 (處理大量生成的邏輯)
// ==========================================
[BurstCompile]
public partial struct MultiSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // 確保場景裡有生成器資料才啟動系統，避免空轉浪費效能
        state.RequireForUpdate<MultiSpawnerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 為了避免每一幀都在無限生成，執行一次後就立刻關閉此系統
        state.Enabled = false;

        // 建立 EntityCommandBuffer (ECB) 待辦清單，這是防止 ECS 記憶體衝突的核心！
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        // 建立隨機種子
        uint seed = (uint)System.DateTime.Now.Millisecond;
        if (seed == 0) seed = 1;
        var random = new Unity.Mathematics.Random(seed);

        // 尋找世界上的生成器
        foreach (var (spawnerData, prefabBuffer, entity) in SystemAPI.Query<RefRO<MultiSpawnerData>, DynamicBuffer<MultiPrefabElement>>().WithEntityAccess())
        {
            int totalCount = spawnerData.ValueRO.SpawnCount;
            int prefabTypesCount = prefabBuffer.Length;

            // 如果沒有抓到任何模型藍圖，銷毀生成器並跳過
            if (prefabTypesCount == 0)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            // 計算每種模型應該平均分配多少隻
            int baseCountPerPrefab = totalCount / prefabTypesCount; 
            int remainder = totalCount % prefabTypesCount;          

            // 針對每一種 Prefab 進行迴圈生成
            for (int i = 0; i < prefabTypesCount; i++)
            {
                Entity currentPrefab = prefabBuffer[i].PrefabEntity;
                if (currentPrefab == Entity.Null) continue;

                // 如果有餘數，分給前面幾個模型
                int spawnAmount = baseCountPerPrefab + (i < remainder ? 1 : 0);

                // 使用 ECB 安全地進行迴圈生成
                for (int j = 0; j < spawnAmount; j++)
                {
                    // 在待辦清單裡寫下：請幫我複製一個實體
                    Entity newInstance = ecb.Instantiate(currentPrefab);

                    // 計算圓形範圍內的隨機位置 (使用 sqrt 讓內部密度均勻)
                    float angle = random.NextFloat(0f, math.PI * 2f);
                    float distance = math.sqrt(random.NextFloat(0f, 1f)) * spawnerData.ValueRO.SpawnRadius;
                    float3 offset = new float3(math.cos(angle) * distance, 0f, math.sin(angle) * distance);
                    float3 finalPos = spawnerData.ValueRO.CenterPosition + offset;

                    // 👇 最新修正版：使用絕對安全的標準賦值法
                    ecb.SetComponent(newInstance, new LocalTransform 
                    {
                        Position = finalPos,
                        Rotation = quaternion.identity, // 預設無旋轉
                        Scale = spawnerData.ValueRO.ScaleFactor
                    });
                }
            }

            // 在待辦清單裡寫下：1000 隻都生成完畢了，把母體生成器銷毀
            ecb.DestroyEntity(entity);
        }

        // 統一執行剛才記下的所有待辦清單 (完美避開 Structural Change 崩潰問題)
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}


// ==========================================
// 1. 資料區塊 (ECS 純數據)
// ==========================================
public struct MultiSpawnerData : IComponentData
{
    public int SpawnCount;
    public float SpawnRadius;
    public float3 CenterPosition;
    public float ScaleFactor;
}

// 用來裝載多種不同觀眾 Prefab 的動態緩衝區元素
[InternalBufferCapacity(16)]
public struct MultiPrefabElement : IBufferElementData
{
    public Entity PrefabEntity;
}
