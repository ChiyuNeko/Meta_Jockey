using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Burst;

// ==========================================
// 1. 材質屬性組件 (GPU 溝通橋樑)
// ==========================================
// 必須與 Shader Graph 中的 Reference 名稱完全一致

// ==========================================
// 3. 烘焙器 (掛在物件上)
// ==========================================
public class ECSOneShotLerpAuthoring : MonoBehaviour
{
    public float startValue = 0f;
    public float endValue = 1f;
    public float duration = 1.0f;
    public bool playOnStart = true;

    class Baker : Baker<ECSOneShotLerpAuthoring>
    {
        public override void Bake(ECSOneShotLerpAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // 加入邏輯組件
            AddComponent(entity, new LerpProgressData
            {
                StartValue = authoring.startValue,
                EndValue = authoring.endValue,
                Duration = authoring.duration,
                ElapsedTime = 0f,
                IsActive = authoring.playOnStart
            });

            // 加入材質屬性組件 (初始值為 A)
            AddComponent(entity, new AlphaOverride
            {
                Value = authoring.startValue
            });
        }
    }
}


[MaterialProperty("_Alpha1")]
public struct AlphaOverride : IComponentData
{
    public float Value;
}

// ==========================================
// 2. 漸變邏輯資料 (儲存 A, B 與時間)
// ==========================================
public struct LerpProgressData : IComponentData
{
    public float StartValue;    // A
    public float EndValue;      // B
    public float Duration;      // 總時間 (秒)
    public float ElapsedTime;   // 已過時間
    public bool IsActive;       // 是否正在執行
}


// ==========================================
// 4. 漸變系統 (處理數值運算)
// ==========================================
[BurstCompile]
public partial struct OneShotLerpSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // 偵測點 1：系統有沒有活著？
        // (先註解掉，不然一秒印 60 次會洗頻，如果連偵測點 2 都沒出現，再打開這行檢查)
        // Debug.Log("【偵測點 1】系統有在執行 OnUpdate！"); 

        float deltaTime = SystemAPI.Time.DeltaTime;

        // 尋找目標物件
        foreach (var (progress, alpha) in SystemAPI.Query<RefRW<LerpProgressData>, RefRW<AlphaOverride>>())
        {
            // 偵測點 2：系統有沒有抓到這個物件？
            Debug.Log($"【偵測點 2】抓到物件了！目前的 IsActive 狀態是：{progress.ValueRO.IsActive}");

            if (!progress.ValueRO.IsActive) continue;

            // 計算時間與比例
            progress.ValueRW.ElapsedTime += deltaTime;
            float t = math.saturate(progress.ValueRO.ElapsedTime / progress.ValueRO.Duration);
            
            // 計算漸變
            float currentValue = math.lerp(progress.ValueRO.StartValue, progress.ValueRO.EndValue, t);
            alpha.ValueRW.Value = currentValue;

            // 偵測點 3：監視核心數值的變化！
            Debug.Log($"【偵測點 3】已過時間:{progress.ValueRO.ElapsedTime:F2}秒 | 進度 t:{t:F2} | 計算出的 Alpha值:{currentValue:F2}");

            // 結束判斷
            if (t >= 1.0f)
            {
                Debug.Log("【偵測點 4】漸變到達 100%，已將 IsActive 設為 false，任務結束！");
                progress.ValueRW.IsActive = false;
            }
        }
    }
}


