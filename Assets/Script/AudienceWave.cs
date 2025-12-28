using UnityEngine;

public class SimpleAudienceWave : MonoBehaviour
{
    [Header("Arms")]
    public Transform leftArmPivot;
    public Transform rightArmPivot;

    [Header("Wave Settings")]
    public float bpm = 120f;
    public float waveAngle = 40f;   // 往前揮的角度
    public float upAngle = -60f;    // 舉過頭基礎角度

    private float waveSpeed;

    void Start()
    {
        waveSpeed = bpm / 60f * Mathf.PI * 2f;
    }

    void Update()
    {
        float wave = Mathf.Sin(Time.time * waveSpeed);

        float finalAngle = upAngle + wave * waveAngle;

        leftArmPivot.localRotation  = Quaternion.Euler(finalAngle, 0, 0);
        rightArmPivot.localRotation = Quaternion.Euler(finalAngle, 0, 0);
    }

    public void SetBPM(float newBpm)
    {
        bpm = newBpm;
        waveSpeed = bpm / 60f * Mathf.PI * 2f;
    }
}
