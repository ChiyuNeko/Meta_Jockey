using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShaderSequenceParameter
{
    public string propertyName;
    public float minValue = 0f;
    public float maxValue = 1f;
    [HideInInspector] public int propertyID;
    [HideInInspector] public Coroutine currentRoutine; 
}

[RequireComponent(typeof(Renderer))]
public class MaterialSequenceController : MonoBehaviour
{
    [Header("Shader 參數清單 (子參數)")]
    public List<ShaderSequenceParameter> parameterList = new List<ShaderSequenceParameter>();

    [Header("主透明度控制 (_MainAlpha)")]
    [Tooltip("Shader 中控制全局透明度的名稱")]
    public string mainAlphaPropertyName = "_MainAlpha";
    public float mainAlphaMin = 0f;
    public float mainAlphaMax = 1f;
    private int mainAlphaID;
    private Coroutine mainAlphaFadeRoutine;

    [Header("時間控制 (秒)")]
    public float fadeInTime = 0.1f;
    public float fadeOutTime = 0.15f;

    [Header("模式設定")]
    public bool mode1_Sequential = true; 
    public bool mode2_Pairs = false;     
    public bool mode3_Random = false;    

    [Header("節奏與循環設定")]
    public float bpm = 120f;
    public float beatsPerSequence = 1f;
    public int loopCount = 2;

    [Header("視覺重疊設定")]
    [Range(0f, 0.8f)]
    public float overlapRatio = 0.5f; 

    [Header("控制模式 (L鍵開關 / P鍵加花)")]
    public bool isSystemActive = false;
    public float repeatIntervalBeats = 4f;
    public float fillSpeedMultiplier = 2f; 

    [HideInInspector] public bool isPlaying = false; 

    private Material mat;
    private Coroutine autoRepeatRoutine;

    private void OnValidate()
    {
        if (mode1_Sequential) { mode2_Pairs = false; mode3_Random = false; }
        else if (mode2_Pairs) { mode1_Sequential = false; mode3_Random = false; }
        else if (mode3_Random) { mode1_Sequential = false; mode2_Pairs = false; }
    }

    void Awake()
    {
        Renderer targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null) mat = targetRenderer.material;

        // 初始化子參數 ID
        foreach (var param in parameterList)
            param.propertyID = Shader.PropertyToID(param.propertyName);
        
        // 初始化主參數 ID
        mainAlphaID = Shader.PropertyToID(mainAlphaPropertyName);
        
        ResetAllParametersImmediate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) ToggleSystem();
        if (Input.GetKeyDown(KeyCode.P)) TriggerSequence(true); 

        HandleAutoRepeatState();
    }

    // ==========================================
    // 系統開關邏輯：包含 _MainAlpha 的淡入淡出
    // ==========================================
    public void ToggleSystem()
    {
        isSystemActive = !isSystemActive;

        if (mainAlphaFadeRoutine != null) StopCoroutine(mainAlphaFadeRoutine);

        if (isSystemActive)
        {
            Debug.Log("燈光系統：開啟 (MainAlpha 淡入)");
            mainAlphaFadeRoutine = StartCoroutine(FadeMainAlpha(mainAlphaMax, fadeInTime));
        }
        else
        {
            Debug.Log("燈光系統：關閉 (MainAlpha 淡出)");
            // 停止自動循環
            if (autoRepeatRoutine != null) { StopCoroutine(autoRepeatRoutine); autoRepeatRoutine = null; }
            
            // 讓主透明度淡出
            mainAlphaFadeRoutine = StartCoroutine(FadeMainAlpha(mainAlphaMin, fadeOutTime));
            
            // 同時讓所有子參數平滑回到最小值 (避免殘留)
            StartCoroutine(GlobalSubParametersFadeOut());
        }
    }

    private IEnumerator FadeMainAlpha(float targetValue, float duration)
    {
        float startValue = mat.GetFloat(mainAlphaID);
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            mat.SetFloat(mainAlphaID, Mathf.Lerp(startValue, targetValue, timer / duration));
            yield return null;
        }
        mat.SetFloat(mainAlphaID, targetValue);
        mainAlphaFadeRoutine = null;
    }

    private void HandleAutoRepeatState()
    {
        if (isSystemActive)
        {
            if (autoRepeatRoutine == null)
                autoRepeatRoutine = StartCoroutine(AutoRepeatLogic());
        }
        else
        {
            if (autoRepeatRoutine != null)
            {
                StopCoroutine(autoRepeatRoutine);
                autoRepeatRoutine = null;
            }
        }
    }

    private IEnumerator AutoRepeatLogic()
    {
        while (isSystemActive)
        {
            if (!isPlaying) TriggerSequence(false);
            float intervalInSeconds = (60f / bpm) * repeatIntervalBeats;
            yield return new WaitForSeconds(intervalInSeconds);
        }
    }

    private IEnumerator GlobalSubParametersFadeOut()
    {
        float timer = 0f;
        List<float> startValues = new List<float>();
        foreach (var param in parameterList)
            startValues.Add(mat.GetFloat(param.propertyID));

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            float progress = 1f - (timer / fadeOutTime);
            for (int i = 0; i < parameterList.Count; i++)
            {
                mat.SetFloat(parameterList[i].propertyID, Mathf.Lerp(parameterList[i].minValue, startValues[i], progress));
            }
            yield return null;
        }
        ResetAllParametersImmediate();
    }

    public void TriggerSequence(bool isFill = false)
    {
        if (isPlaying || (isFill == false && !isSystemActive)) return; 
        if (parameterList.Count == 0 || mat == null) return;

        float durationPerLoop = (60f / bpm) * beatsPerSequence;
        if (isFill) durationPerLoop /= fillSpeedMultiplier;

        if (mode1_Sequential) StartCoroutine(PlayMode1(durationPerLoop));
        else if (mode2_Pairs) StartCoroutine(PlayMode2(durationPerLoop));
        else if (mode3_Random) StartCoroutine(PlayMode3(durationPerLoop));
    }

    private void ActivateParameter(ShaderSequenceParameter param, float totalDuration)
    {
        if (param.currentRoutine != null) StopCoroutine(param.currentRoutine);
        param.currentRoutine = StartCoroutine(FadePropertyRoutine(param, totalDuration));
    }

    private IEnumerator FadePropertyRoutine(ShaderSequenceParameter param, float totalDuration)
    {
        float actualFadeIn = fadeInTime;
        float actualFadeOut = fadeOutTime;

        if (actualFadeIn + actualFadeOut > totalDuration)
        {
            float scale = totalDuration / (actualFadeIn + actualFadeOut);
            actualFadeIn *= scale;
            actualFadeOut *= scale;
        }

        float holdTime = totalDuration - actualFadeIn - actualFadeOut;
        float timer = 0f;

        while (timer < actualFadeIn)
        {
            timer += Time.deltaTime;
            mat.SetFloat(param.propertyID, Mathf.Lerp(param.minValue, param.maxValue, timer / actualFadeIn));
            yield return null;
        }
        mat.SetFloat(param.propertyID, param.maxValue);

        if (holdTime > 0) yield return new WaitForSeconds(holdTime);

        timer = 0f;
        while (timer < actualFadeOut)
        {
            timer += Time.deltaTime;
            mat.SetFloat(param.propertyID, Mathf.Lerp(param.maxValue, param.minValue, timer / actualFadeOut));
            yield return null;
        }
        mat.SetFloat(param.propertyID, param.minValue);
        param.currentRoutine = null;
    }

    // --- 播放模式 (1, 2, 3) ---
    private IEnumerator PlayMode1(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = parameterList.Count + 1;
        float waitTime = durationPerLoop / totalSteps; 
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < parameterList.Count; i++)
            {
                ActivateParameter(parameterList[i], effectDuration);
                yield return new WaitForSeconds(waitTime);
            }
            TurnOnAllParameters(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }
        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        isPlaying = false;
    }

    private IEnumerator PlayMode2(float durationPerLoop)
    {
        isPlaying = true;
        int pairSteps = Mathf.CeilToInt(parameterList.Count / 2.0f);
        int stepsPerPass = pairSteps + 1; 
        int totalSteps = stepsPerPass * 2; 
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < pairSteps; i++)
            {
                int c1 = i * 2; int c2 = c1 + 1;
                if (c1 < parameterList.Count) ActivateParameter(parameterList[c1], effectDuration); 
                if (c2 < parameterList.Count) ActivateParameter(parameterList[c2], effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }
            yield return new WaitForSeconds(waitTime);

            for (int i = 0; i < pairSteps; i++)
            {
                int c1 = i * 2; int c2 = c1 + 1;
                if (c1 < parameterList.Count) ActivateParameter(parameterList[c1], effectDuration); 
                if (c2 < parameterList.Count) ActivateParameter(parameterList[c2], effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }
            TurnOnAllParameters(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }
        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        isPlaying = false;
    }

    private IEnumerator PlayMode3(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = parameterList.Count + 1;
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            List<int> randomIndices = new List<int>();
            for (int i = 0; i < parameterList.Count; i++) randomIndices.Add(i);
            for (int i = 0; i < randomIndices.Count; i++) {
                int temp = randomIndices[i]; int r = Random.Range(i, randomIndices.Count);
                randomIndices[i] = randomIndices[r]; randomIndices[r] = temp;
            }
            for (int i = 0; i < randomIndices.Count; i++) {
                ActivateParameter(parameterList[randomIndices[i]], effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }
            TurnOnAllParameters(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }
        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        isPlaying = false;
    }

    private void TurnOnAllParameters(float duration) {
        foreach (var param in parameterList) ActivateParameter(param, duration);
    }

    private void ResetAllParametersImmediate() {
        if (mat == null) return;
        foreach (var param in parameterList) mat.SetFloat(param.propertyID, param.minValue);
        mat.SetFloat(mainAlphaID, mainAlphaMin);
    }
}