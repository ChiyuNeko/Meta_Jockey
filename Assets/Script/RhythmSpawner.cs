using System.Collections;
using UnityEngine;
using Unity.Entities;   // 引入 ECS 核心
using Unity.Transforms; // 引入 ECS 的 Transform 系統

public class RhythmSpawner : MonoBehaviour
{
    [Header("生成的物件 (Prefabs)")]
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    [Header("生成的特殊物件 (Prefabs)")]
    [Tooltip("與 Object 1 同時生成的第四個物件")]
    public GameObject object4;
    public GameObject object5;

    [Header("節奏與速度設定")]
    [Tooltip("基礎每分鐘節拍數 (Beats Per Minute)")]
    public float bpm = 120f;
    
    // 👇 新增的速度倍率變數
    [Tooltip("速度倍率 (1 = 正常速度, 2 = 兩倍速, 0.5 = 半速)")]
    public float speedMultiplier = 1f; 
    
    [Tooltip("是否要無限循環這個生成過程？")]
    public bool isLooping = true;

    private float timer = 0f;
    private int currentStep = 0;
    private bool isPlaying = true;

    void Start()
    {
    }

    void Update()
    {
        // 防呆機制：確保 bpm 和倍率都必須大於 0，否則暫停執行
        if (!isPlaying || bpm <= 0f || speedMultiplier <= 0f) return;

        // 關鍵修改：先計算「實際運作的 BPM」，再換算成秒數
        // 例如：120 BPM * 2倍速 = 240 BPM (每 0.25 秒生成一次)
        float effectiveBpm = bpm * speedMultiplier;
        float secondsPerBeat = 60f / effectiveBpm;

        // 累加時間
        timer += Time.deltaTime;

        // 當時間到達一拍的間隔時，觸發生成
        if (timer >= secondsPerBeat)
        {
            // 扣除一拍的時間，保持節奏精準
            timer -= secondsPerBeat;
            
            ExecuteSpawnStep(currentStep);

            currentStep++;

            // 目前設定為 3 個步驟為一個完整週期 (0, 1, 2)
            if (currentStep > 2) 
            {
                if (isLooping)
                {
                    currentStep = 0; // 重置步驟，回到第一拍
                }
                else
                {
                    isPlaying = false; // 停止生成
                }
            }
        }
    }

    private void ExecuteSpawnStep(int step)
    {
        switch (step)
        {
            case 0:
                // 第一拍：同時生成 第一個 和 第四個 物件
                SpawnPrefab(object1);
                SpawnPrefab(object4);
                SpawnPrefab(object5);

                SpawnECSSphere(transform.position, 1);
                break;
            case 1:
                // 第二拍：生成 第二個 物件
                SpawnPrefab(object2);
                break;
            case 2:
                // 第三拍：生成 第三個 物件
                SpawnPrefab(object3);
                break;
        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            // 在 Prefab 原本設定的位置與旋轉角度生成物件
            Instantiate(prefab, prefab.transform.position, prefab.transform.rotation);
        }
        else
        {
            Debug.LogWarning("有物件欄位未填寫，請檢查 Inspector！");
        }
    }

    private void SpawnECSSphere(Vector3 spawnPosition, int indexToSpawn)
    {
        World world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;
        EntityManager entityManager = world.EntityManager;

        // 尋找我們的「菜單倉庫」
        var query = entityManager.CreateEntityQuery(typeof(SphereBlueprintElement));
        if (query.IsEmpty)
        {
            Debug.LogWarning("找不到 TriggerSphereVault！請確認 ECS_Vault 有在場景中且 List 有放東西。");
            return;
        }

        // 取得整份菜單 (Buffer)
        var entity = query.GetSingletonEntity();
        var buffer = entityManager.GetBuffer<SphereBlueprintElement>(entity);

        // 防呆：如果亂填 ID 超出菜單範圍，就強制給他第一顆球
        if (indexToSpawn < 0 || indexToSpawn >= buffer.Length)
        {
            Debug.LogWarning($"飛彈要求的球編號 {indexToSpawn} 超出範圍！改為生成第 0 顆球。");
            indexToSpawn = 0;
        }

        // 根據 ID 從菜單拿出對應的藍圖
        Entity prefabEntity = buffer[indexToSpawn].Prefab;

        // 命令 ECS 生成該球並設定位置
        Entity spawnedSphere = entityManager.Instantiate(prefabEntity);
        entityManager.SetComponentData(spawnedSphere, Unity.Transforms.LocalTransform.FromPosition(spawnPosition));
    }
}