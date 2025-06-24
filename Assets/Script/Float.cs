using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    public float floatAmplitude = 0.5f; // 浮動的幅度
    public float floatFrequency = 1f;   // 浮動的速度
    public float rotationSpeed = 50f;   // 每秒旋轉角度（設為 0 則不旋轉）

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 上下浮動（使用 Sine 波）
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, yOffset, 0);

        // 自動旋轉（可選）
        if (rotationSpeed != 0f)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
