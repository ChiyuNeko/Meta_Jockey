using System.Collections;
using UnityEngine;

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
        if (collision.gameObject.tag != "Controllor")
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
            
            //Debug.Log($"{spawnPrefab.name} 已在最頂層生成。");
        }
        yield return new WaitForSeconds(DeadTimer);
        Destroy(gameObject);
    }
}