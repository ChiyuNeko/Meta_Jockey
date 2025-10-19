using System.Collections;
using UnityEngine;

public class HomingObject : MonoBehaviour
{
    [Header("目標物件 (可在Inspector指定)")]
    public Transform target;

    [Header("飛行速度")]
    public float speed = 1f;

    [Header("弧線高度 (越大弧度越高)")]
    public float arcHeight = 2f;

    [Header("經度偏轉角 (模擬地球經線旋轉)")]
    [Range(-180f, 180f)]
    public float longitudeOffset = 30f;

    [Header("是否隨機產生偏轉角")]
    public bool randomizeLongitude = false;

    [Header("隨機偏轉角範圍")]
    public Vector2 randomLongitudeRange = new Vector2(-60f, 60f);

    private Vector3 startPos;
    private float t;
    private Vector3 rotatedTargetPos;

    void Start()
    {
        // 如果沒指定 target，自動找名稱為 "Target_Cube" 的物件
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

        // ✅ 若啟用隨機偏轉角，則於啟動時計算一個隨機角度
        if (randomizeLongitude)
        {
            longitudeOffset = Random.Range(randomLongitudeRange.x, randomLongitudeRange.y);
        }

        // --- 計算經度偏轉後的「虛擬弧線控制點」 ---
        Vector3 dir = target.position - startPos;
        Quaternion rotation = Quaternion.AngleAxis(longitudeOffset, dir.normalized); // 繞飛行方向軸旋轉
        Vector3 upVector = rotation * Vector3.up; // 偏轉後的上方向

        // 生成弧線頂點
        rotatedTargetPos = (startPos + target.position) / 2 + upVector * arcHeight;
    }

    void Update()
    {
        if (target == null)
            return;

        t += Time.deltaTime * speed;
        t = Mathf.Clamp01(t);

        // 使用三點插值產生平滑拋物線
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
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
