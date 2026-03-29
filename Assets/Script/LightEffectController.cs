using System.Collections;
using UnityEngine;

// 確保掛載此腳本的物件上一定有 Renderer 元件 (因為我們要抓它的 Material)
[RequireComponent(typeof(Renderer))]
public class LightEffectController : MonoBehaviour
{
    [Header("控制設定")]
    [Tooltip("Shader 中控制透明度的 Reference 名稱")]
    public string alphaPropertyName = "_AllAlpha";

    [Header("時間控制 (秒)")]
    [Tooltip("淡入所需時間")]
    public float fadeInTime = 0.1f;
    [Tooltip("淡出所需時間")]
    public float fadeOutTime = 0.15f;

    [Header("數值設定")]
    [Tooltip("Spot Light 亮起時的最大強度")]
    public float maxIntensity = 10f;
    [Tooltip("Shader _AllAlpha 亮起時的最大值")]
    public float maxAlpha = 1f;

    // 改為私有變數，不顯示在 Inspector 中
    private Light targetSpotLight;
    private Material mat;

    void Awake()
    {
        // 1. 自動抓取自身的 Renderer 並取得材質實體 (Instantiate Material)
        Renderer targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null)
        {
            mat = targetRenderer.material;
        }
        
        // 2. 自動尋找自身或「子物件」中的 Light 元件
        targetSpotLight = GetComponentInChildren<Light>();

        // 防呆提醒：確保抓到的真的是 Spot Light 類型
        if (targetSpotLight != null && targetSpotLight.type != LightType.Spot)
        {
            Debug.LogWarning($"{gameObject.name} 抓到的 Light 不是 Spot 類型喔，請檢查子物件設定！");
        }

        // 確保初始狀態為全暗
        SetVisuals(0f, 0f);
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
            float progress = timer / actualFadeIn;
            SetVisuals(Mathf.Lerp(0, maxIntensity, progress), Mathf.Lerp(0, maxAlpha, progress));
            yield return null;
        }
        SetVisuals(maxIntensity, maxAlpha); 

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
            float progress = timer / actualFadeOut;
            SetVisuals(Mathf.Lerp(maxIntensity, 0, progress), Mathf.Lerp(maxAlpha, 0, progress));
            yield return null;
        }
        SetVisuals(0f, 0f); 
    }

    // 封裝數值設定
    private void SetVisuals(float intensity, float alpha)
    {
        if (targetSpotLight != null) targetSpotLight.intensity = intensity;
        
        if (mat != null && mat.HasProperty(alphaPropertyName))
        {
            mat.SetFloat(alphaPropertyName, alpha);
        }
    }
}