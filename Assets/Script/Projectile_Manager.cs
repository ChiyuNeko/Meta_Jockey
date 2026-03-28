using System.Collections;
using UnityEngine;
using Unity.Entities;   // 引入 ECS 核心
using Unity.Transforms; // 引入 ECS 的 Transform 系統
public class Projectile_Manager : MonoBehaviour
{
    [Header("功能開關")]
    public bool isHoming = true; // 若關閉，則不執行此腳本的任何位移邏輯
    public bool isSpawning = false;

    public bool isRigidBody = false;

    [Header("目標物件")]
    public Transform target;
    public float DeadTimer;

    [Header("飛行速度")]
    public float speed = 1f;

    [Header("生成設定")]
    [Tooltip("自毀時要生成的預製體")]
    public GameObject spawnPrefab;

    [Header("生成設定")]
    [Tooltip("自毀時要生成的傳統預製體 (爆炸特效)")]
    public GameObject sphere;
    
    // 👇 新增這行：決定這顆飛彈要生出哪一種球
    [Tooltip("對應 ECS_Vault 清單中的索引值 (0代表第一顆球, 1代表第二顆...)")]
    public int sphereIndex = 0;
    

    [Header("弧線高度")]
    public float arcHeight = 2f;

    [Header("經度偏轉角")]
    [Range(-180f, 180f)]
    public float longitudeOffset = 30f;

    [Header("是否隨機產生偏轉角")]
    public bool randomizeLongitude = false;

    [Header("隨機偏轉角範圍")]
    public Vector2 randomLongitudeRange = new Vector2(-60f, 60f);

    private Vector3 startPos;
    private float t;
    private Vector3 rotatedTargetPos;

    private bool isHit=true;

    void Start()
    {
        // 如果功能關閉，連 Start 的計算都跳過
        if (!isHoming) return;

        if (target == null)
        {
            GameObject found = GameObject.Find("Target_Cube");
            if (found != null)
                target = found.transform;
        }

        startPos = transform.position;

        if (target == null)
        {
            Debug.LogWarning($"{name} 沒有找到 target，HomingObject 無法運作。");
            return;
        }

        if (randomizeLongitude)
        {
            longitudeOffset = Random.Range(randomLongitudeRange.x, randomLongitudeRange.y);
        }

        Vector3 dir = target.position - startPos;
        Quaternion rotation = Quaternion.AngleAxis(longitudeOffset, dir.normalized);
        Vector3 upVector = rotation * Vector3.up;

        rotatedTargetPos = (startPos + target.position) / 2 + upVector * arcHeight;
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Controllor")
        {
            Debug.Log(collision.gameObject.name);
        }
        else
        {
            if (isRigidBody && isHit)
            {
                StartCoroutine(SelfDestruction());
                isHit=false;
            }
            
        }
        
    }


    void Update()
    {
        // ✅ 核心改動：如果 isHoming 為 false，直接不執行任何飛行邏輯
        if (!isHoming) return;

        if (target == null)
            return;

        t += Time.deltaTime * speed;
        t = Mathf.Clamp01(t);

        // 貝茲曲線插值
        Vector3 part1 = Vector3.Lerp(startPos, rotatedTargetPos, t);
        Vector3 part2 = Vector3.Lerp(rotatedTargetPos, target.position, t);
        transform.position = Vector3.Lerp(part1, part2, t);

        // 朝向目標
        transform.LookAt(target);

        // 抵達目標 → 自毀
        if (t >= 1f)
        {
            StartCoroutine(SelfDestruction());
        }
    }

    private IEnumerator SelfDestruction()
    {
        transform.localScale*= 0.01f;
        if (isSpawning && spawnPrefab != null)
        {
            // ✅ 關鍵：Instantiate 的第三個參數傳入 null，確保它在 Hierarchy 的最頂層（無父物件）
            // 使用當前物件的位置與旋轉值
            Instantiate(spawnPrefab, transform.position, transform.rotation, null);
            //Instantiate(sphere, gameObject.transform.position, Quaternion.identity);
            SpawnECSSphere(transform.position, sphereIndex);
            //Debug.Log($"{spawnPrefab.name} 已在最頂層生成。");
        }
        yield return new WaitForSeconds(DeadTimer);
        Destroy(gameObject);
    }
    // ==========================================
    // 跨界生成：從 MonoBehaviour 呼叫 ECS
    // ==========================================
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