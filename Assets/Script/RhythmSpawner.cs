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

    public int ECSindex = 1;

    [Header("節奏與速度設定")]
    public float bpm = 120f;
    [Tooltip("速度倍率 (1 = 正常速度, 2 = 兩倍速, 0.5 = 半速)")]
    public float speedMultiplier = 1f; 

    [Header("防呆設定")]
    [Tooltip("每次觸發後的冷卻時間 (秒)。在此時間內無法再次觸發。")]
    public float cooldownTime = 2.0f; // ★ 新增：可自訂的冷卻秒數
    
    // 記錄下一次可以觸發的時間點
    private float nextAllowedTriggerTime = 0f; 

    // 用來防呆，避免狂按導致多組流程疊加在一起
    private bool isPlayingSequence = false;

    void Start()
    {
    }

    void Update()
    {
        // 偵測按下 K 鍵
        if (Input.GetKeyDown(KeyCode.K))
        {
            // 觸發邏輯統一交給 RYSpawn 處理
            RYSpawn();
        }
    }

    public void RYSpawn()
    {
        // ★ 防呆雙重鎖：
        // 1. 如果流程還在跑 (isPlayingSequence == true) -> 拒絕
        // 2. 如果現在的遊戲時間還沒超過允許觸發的時間 -> 拒絕
        if (isPlayingSequence || Time.time < nextAllowedTriggerTime)
        {
            return;
        }

        // 成功觸發！先更新下一次允許觸發的時間點
        nextAllowedTriggerTime = Time.time + cooldownTime;

        StartCoroutine(RunSpawnSequence());
    }

    // ==========================================
    // 自動連續生成的協程
    // ==========================================
    private IEnumerator RunSpawnSequence()
    {
        isPlayingSequence = true; // 上鎖，防止重複觸發

        // 計算實際的 BPM 與每拍的秒數
        float effectiveBpm = bpm * speedMultiplier;
        
        // 防呆：避免除以零
        if (effectiveBpm <= 0f) effectiveBpm = 120f; 
        
        float secondsPerBeat = 60f / effectiveBpm;

        // --- 第 1 拍 ---
        ExecuteSpawnStep(0);
        yield return new WaitForSeconds(secondsPerBeat);

        // --- 第 2 拍 ---
        ExecuteSpawnStep(1);
        yield return new WaitForSeconds(secondsPerBeat);

        // --- 第 3 拍 ---
        ExecuteSpawnStep(2);
        
        // 整個流程跑完後解鎖
        isPlayingSequence = false; 
    }

    private void ExecuteSpawnStep(int step)
    {
        switch (step)
        {
            case 0:
                SpawnPrefab(object1);
                SpawnPrefab(object4);
                SpawnPrefab(object5);

                SpawnECSSphere(transform.position, ECSindex);
                break;
            case 1:
                SpawnPrefab(object2);
                break;
            case 2:
                SpawnPrefab(object3);
                break;
        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (prefab != null)
        {
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

        var query = entityManager.CreateEntityQuery(typeof(SphereBlueprintElement));
        if (query.IsEmpty)
        {
            Debug.LogWarning("找不到 TriggerSphereVault！請確認 ECS_Vault 有在場景中且 List 有放東西。");
            return;
        }

        var entity = query.GetSingletonEntity();
        var buffer = entityManager.GetBuffer<SphereBlueprintElement>(entity);

        if (indexToSpawn < 0 || indexToSpawn >= buffer.Length)
        {
            Debug.LogWarning($"飛彈要求的球編號 {indexToSpawn} 超出範圍！改為生成第 0 顆球。");
            indexToSpawn = 0;
        }

        Entity prefabEntity = buffer[indexToSpawn].Prefab;

        Entity spawnedSphere = entityManager.Instantiate(prefabEntity);
        entityManager.SetComponentData(spawnedSphere, Unity.Transforms.LocalTransform.FromPosition(spawnPosition));
    }
}