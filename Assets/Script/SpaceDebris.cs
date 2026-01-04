using UnityEngine;

public class SpaceDebris : MonoBehaviour
{
    [Header("Base Settings")]
    public float baseSpeed = 10f;       // 平常速度
    public float warpMultiplier = 5f;   // 按 G 加速倍率
    public float speedMultiplier = 1f;  // 每個 Prefab 自訂速度
    public Vector2 randomXYRange = new Vector2(0.2f, 0.2f); // 微漂浮範圍 X/Y

    [Header("Spawn / Recycle Settings")]
    public Transform spawnAreaMin;
    public Transform spawnAreaMax;
    public float recycleX = -20f;
    public float recycleZ = -20f;

    [Header("Camera")]
    public Transform cameraTarget;

    private bool isWarp = false;
    private Vector3 randomOffset;

    void Start()
    {
        if (cameraTarget == null)
            cameraTarget = Camera.main.transform;

        SetRandomOffset();
    }

    void Update()
    {
        // Warp 開關
        isWarp = Input.GetKey(KeyCode.G);

        // 計算速度
        float speed = baseSpeed * speedMultiplier * (isWarp ? warpMultiplier : 1f);

        // 移動 + 微漂浮
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.left * speed * Time.deltaTime + randomOffset * Time.deltaTime;
        //transform.Translate(Vector3.left * speed * Time.deltaTime + randomOffset * Time.deltaTime, Space.World);

        // 面向攝影機（保持主要朝向）
        Vector3 dir = transform.position - cameraTarget.position;
        transform.rotation = Quaternion.LookRotation(dir);

        // Warp 時 Z 軸拉長（平滑）
        float targetZScale = isWarp ? 2f : 1f;
        Vector3 scale = transform.localScale;
        scale.z = Mathf.Lerp(scale.z, targetZScale, Time.deltaTime * 5f);
        transform.localScale = scale;

        // 超過回收位置就重生
        if (transform.position.z < recycleZ)
            Recycle();
        if (transform.position.x < recycleX)
            Recycle();
    }

    public void Recycle()
    {
        // 隨機生成位置
        Vector3 pos = new Vector3(
            Random.Range(spawnAreaMin.position.x, spawnAreaMax.position.x),
            Random.Range(spawnAreaMin.position.y, spawnAreaMax.position.y),
            Random.Range(spawnAreaMin.position.z, spawnAreaMax.position.z)
        );

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTarget.position); // 直接面向攝影機
        transform.localScale = Vector3.one * Random.Range(0.3f, 2f);

        SetRandomOffset();
    }

    private void SetRandomOffset()
    {
        randomOffset = new Vector3(
            Random.Range(-randomXYRange.x, randomXYRange.x),
            Random.Range(-randomXYRange.y, randomXYRange.y),
            0f
        );
    }
}










