using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Header("基礎銷毀設定")]
    [Tooltip("物體總生存時間（秒）")]
    public float delay = 3.0f;

    [Header("光源與數值控制")]
    public bool modifyLightIntensity = false;
    public Light targetLight;
    
    [Tooltip("數值變化的最大值")]
    public float maxValue = 5.0f;

    [Header("曲線速度控制")]
    [Tooltip("在 Inspector 視窗點擊曲線來繪製變化的形狀。\n左側(0)是出生，右側(1)是自毀。")]
    public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0, 0, 1, 0); 
    // 預設建立一個頭尾都是0的曲線

    private float timer = 0f;

    void Start()
    {
        // 初始自動抓取
        if (modifyLightIntensity && targetLight == null)
        {
            targetLight = GetComponentInChildren<Light>();
        }

        // 初始化一條預設曲線（如果使用者沒畫的話）
        // 這會產生 0 -> 1 -> 0 的對稱效果
        if (intensityCurve.length <= 0)
        {
            intensityCurve.AddKey(0f, 0f);    // 出生時 0
            intensityCurve.AddKey(0.5f, 1f);  // 中間 1
            intensityCurve.AddKey(1f, 0f);    // 銷毀時 0
        }

        Destroy(gameObject, delay);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / delay);

        // 1. 取得自定義曲線的數值
        float currentDynamicValue = GetCurveValue(progress, maxValue);

        // 2. 應用於光源
        if (modifyLightIntensity && targetLight != null)
        {
            targetLight.intensity = currentDynamicValue;
        }
    }

    /// <summary>
    /// 根據動畫曲線回傳對應的強度數值
    /// </summary>
    /// <param name="progress">目前時間進度 (0-1)</param>
    /// <param name="max">最大倍率</param>
    public float GetCurveValue(float progress, float max)
    {
        // Evaluate 會根據 progress 取得曲線對應的 Y 軸數值 (通常是 0~1)
        return intensityCurve.Evaluate(progress) * max;
    }
}