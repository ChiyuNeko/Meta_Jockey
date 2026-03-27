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
    public GameObject sphere;

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
            SpawnECSSphere(transform.position);
            //Debug.Log($"{spawnPrefab.name} 已在最頂層生成。");
        }
        yield return new WaitForSeconds(DeadTimer);
        Destroy(gameObject);
    }
    // ==========================================
    // 跨界生成：從 MonoBehaviour 呼叫 ECS
    // ==========================================
    private void SpawnECSSphere(Vector3 spawnPosition)
    {
        // 取得當前運作的 ECS 世界與上帝權限 (EntityManager)
        World world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;
        EntityManager entityManager = world.EntityManager;

        // 在 ECS 世界中尋找我們剛剛建立的「藍圖倉庫」
        var query = entityManager.CreateEntityQuery(typeof(TriggerSphereVaultData));
        if (!query.HasSingleton<TriggerSphereVaultData>())
        {
            Debug.LogWarning("找不到 TriggerSphereVault！請確認 SubScene 中有放置這個倉庫。");
            return;
        }

        // 把藍圖 ID 拿出來
        Entity prefabEntity = query.GetSingleton<TriggerSphereVaultData>().SpherePrefab;

        // 命令 ECS 瞬間生出一顆球
        Entity spawnedSphere = entityManager.Instantiate(prefabEntity);

        // 命令 ECS 把這顆球移到飛彈爆炸的座標
        entityManager.SetComponentData(spawnedSphere, LocalTransform.FromPosition(spawnPosition));
    }
}