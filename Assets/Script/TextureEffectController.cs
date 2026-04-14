using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 建立一個可以在 Inspector 中顯示的自訂類別，用來儲存 Shader 參數設定
[System.Serializable]
public class ShaderParameter
{
    [Tooltip("Shader Graph 中的 Reference 名稱 (例如 _AllAlpha)")]
    public string propertyName;
    [Tooltip("燈光全暗時的數值 (通常為 0)")]
    public float minValue = 0f;
    [Tooltip("燈光全亮時的最大數值")]
    public float maxValue = 1f;

    // 隱藏變數，用來快取 Property ID 以增進效能
    [HideInInspector] public int propertyID;
}

// 確保掛載此腳本的物件上一定有 Renderer 元件
[RequireComponent(typeof(Renderer))]
public class TextureEffectController : MonoBehaviour
{
    [Header("時間控制 (秒)")]
    [Tooltip("淡入所需時間")]
    public float fadeInTime = 0.1f;
    [Tooltip("淡出所需時間")]
    public float fadeOutTime = 0.15f;

    [Header("Shader 參數控制清單")]
    [Tooltip("在這裡加入你想漸變的 Shader Graph Float 數值")]
    public List<ShaderParameter> shaderParameters = new List<ShaderParameter>();

    private Material mat;

    void Awake()
    {
        // 自動抓取自身的 Renderer 並取得材質實體 (Instantiate Material)
        Renderer targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            mat = targetRenderer.material;
        }

        // 在遊戲開始前，先把所有的字串名稱轉換為 Shader Property ID，這樣執行時效能更好
        foreach (var param in shaderParameters)
        {
            param.propertyID = Shader.PropertyToID(param.propertyName);
        }

        // 確保初始狀態為全暗 (漸變進度為 0)
        SetVisualsProgress(0f);
    }

    // 當 SequenceController 呼叫這個物件時觸發
    public void PlayEffect(float totalDuration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(totalDuration));
    }

    private IEnumerator FadeRoutine(float totalDuration)
    {
        // 防呆機制：確保淡入淡出時間不會超過節奏配給的總時間
        float actualFadeIn = fadeInTime;
        float actualFadeOut = fadeOutTime;
        if (actualFadeIn + actualFadeOut > totalDuration)
        {
            float scale = totalDuration / (actualFadeIn + actualFadeOut);
            actualFadeIn *= scale;
            actualFadeOut *= scale;
        }

        float holdTime = totalDuration - actualFadeIn - actualFadeOut;

        // --- 1. Fade In (淡入) ---
        float timer = 0f;
        while (timer < actualFadeIn)
        {
            timer += Time.deltaTime;
            // 計算目前的進度 (0 到 1)
            float progress = timer / actualFadeIn;
            SetVisualsProgress(progress);
            yield return null;
        }
        SetVisualsProgress(1f); // 確保到達完美的最大值 100%

        // --- 2. Hold (維持最亮) ---
        if (holdTime > 0)
        {
            yield return new WaitForSeconds(holdTime);
        }

        // --- 3. Fade Out (淡出) ---
        timer = 0f;
        while (timer < actualFadeOut)
        {
            timer += Time.deltaTime;
            // 計算目前的進度 (從 1 降到 0)
            float progress = 1f - (timer / actualFadeOut);
            SetVisualsProgress(progress);
            yield return null;
        }
        SetVisualsProgress(0f); // 確保最後完全變暗 0%
    }

    // 封裝數值設定，統一透過 0~1 的進度來控制所有清單中的參數
    private void SetVisualsProgress(float progress)
    {
        if (mat == null) return;

        // 走訪清單中的每一個 Shader 參數
        foreach (var param in shaderParameters)
        {
            // 利用 Lerp (線性差值) 根據進度算出目前的數值
            float currentValue = Mathf.Lerp(param.minValue, param.maxValue, progress);
            
            // 將數值套用到材質上
            mat.SetFloat(param.propertyID, currentValue);
        }
    }
}