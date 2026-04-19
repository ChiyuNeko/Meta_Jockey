using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialPropertyRhythmTest : MonoBehaviour
{
    [Header("填入準確的 Reference 名稱")]
    public string parameterName = "_TimerValue";
    
    public float minValue = 0f;
    public float maxValue = 1f;

    [Header("節奏同步設定")]
    public float bpm = 120f;
    public float speedMultiplier = 1f;
    public float durationInBeats = 1f;

    private Renderer myRenderer;
    private float timer = 0f;

    // 儲存這個物件專屬的材質球實體
    private Material myMaterialInstance; 

    void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        // 這裡會強制複製一顆獨立的材質球給這個物件
        myMaterialInstance = myRenderer.material; 
        
        myMaterialInstance.SetFloat(parameterName, minValue);
    }

    void Update()
    {
        float effectiveBpm = Mathf.Max(bpm * speedMultiplier, 0.0001f);
        float secondsPerBeat = 60f / effectiveBpm;
        float totalDurationInSeconds = secondsPerBeat * durationInBeats;

        if (timer < totalDurationInSeconds)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / totalDurationInSeconds);
            float currentValue = Mathf.Lerp(minValue, maxValue, t);

            // 直接修改這顆獨立材質球的數值
            myMaterialInstance.SetFloat(parameterName, currentValue);
        }
    }
}