using UnityEngine;
using System.Collections;

public class SkyboxSwitcher : MonoBehaviour
{
    [Header("Skybox Blend Material (Shader Graph)")]
    public Material skyboxMaterial;   // 使用帶有 _Blend 的材質

    [Header("Directional Light")]
    public Light directionalLight;

    [Header("Original Sky Settings")]
    public float originalIntensity = 1f;
    public Color originalColor = Color.white;

    [Header("Target Sky Settings")]
    public float targetIntensity = 0.2f;
    public Color targetColor = new Color(0.4f, 0.5f, 1f);

    [Header("Transition")]
    public float transitionDuration = 2f;

    private bool isSwitched = false;
    private bool isTransitioning = false;

    void Start()
    {
        // 確保一開始是白天
        skyboxMaterial.SetFloat("_Blend", 0f);
        directionalLight.intensity = originalIntensity;
        directionalLight.color = originalColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTransitioning)
        {
            StartCoroutine(SwitchSkybox());
        }
    }
    public void TriggerSkyboxSwitch()
    {
        if (!isTransitioning)
        {
            StartCoroutine(SwitchSkybox());
        }
    }

    IEnumerator SwitchSkybox()
    {
        isTransitioning = true;

        float time = 0f;

        float startBlend = skyboxMaterial.GetFloat("_Blend");
        float endBlend = isSwitched ? 0f : 1f;

        float startIntensity = directionalLight.intensity;
        float endIntensity = isSwitched ? originalIntensity : targetIntensity;

        Color startColor = directionalLight.color;
        Color endColor = isSwitched ? originalColor : targetColor;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            // 🌅 天空混合
            float currentBlend = Mathf.Lerp(startBlend, endBlend, t);
            skyboxMaterial.SetFloat("_Blend", currentBlend);

            // 💡 光線強度同步
            directionalLight.intensity =
                Mathf.Lerp(startIntensity, endIntensity, t);

            // 🎨 光線顏色同步
            directionalLight.color =
                Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        skyboxMaterial.SetFloat("_Blend", endBlend);
        directionalLight.intensity = endIntensity;
        directionalLight.color = endColor;

        isSwitched = !isSwitched;
        isTransitioning = false;
    }
}