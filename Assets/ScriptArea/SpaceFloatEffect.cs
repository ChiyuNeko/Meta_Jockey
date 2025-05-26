using UnityEngine;

public class SpaceFloatEffect : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatStrength = 0.5f;          // 漂浮的強度
    public float floatSpeed = 1.0f;             // 漂浮的速度

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;   // 旋轉軸
    public float rotationSpeed = 10f;           // 旋轉速度

    [Header("Random Drift Settings")]
    public float driftStrength = 0.2f;          // 隨機漂移的強度
    public float driftInterval = 2.0f;          // 漂移變化的時間間隔

    private Vector3 startPosition;
    private Vector3 driftDirection;
    private float driftTimer;

    void Start()
    {
        startPosition = transform.position;
        GenerateRandomDrift();
    }

    void Update()
    {
        // 漂浮效果
        transform.position = startPosition + new Vector3(0.0f, Mathf.Sin(Time.time * floatSpeed) * floatStrength, 0.0f);

        // 旋轉效果
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        // 隨機漂移
        driftTimer += Time.deltaTime;
        if (driftTimer >= driftInterval)
        {
            GenerateRandomDrift();
            driftTimer = 0;
        }
        transform.position += driftDirection * Time.deltaTime;
    }

    void GenerateRandomDrift()
    {
        driftDirection = new Vector3(
            Random.Range(-driftStrength, driftStrength),
            Random.Range(-driftStrength, driftStrength),
            Random.Range(-driftStrength, driftStrength)
        );
    }
}

