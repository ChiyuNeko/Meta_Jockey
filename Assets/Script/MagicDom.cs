using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; // ★ 必須引入 VFX 命名空間才能控制 Visual Effect

// 確保掛載這個腳本的物件上，一定會有 VisualEffect 元件
[RequireComponent(typeof(VisualEffect))]
public class MagicDom : MonoBehaviour
{
    [Header("VFX 參數設定")]
    [Tooltip("請輸入你在 VFX Graph 中對外開放的 Boolean 參數名稱")]
    public string boolPropertyName = "CanSpawn";

    private VisualEffect vfx;

    void Start()
    {
        // 遊戲開始時，自動抓取掛在同一個 GameObject 上的 VFX 元件
        vfx = GetComponent<VisualEffect>();
        vfx.SetBool(boolPropertyName, false);
    }

    void Update()
    {
        // 如果你需要測試，可以把下面這行解除註解，按空白鍵測試
        // if (Input.GetKeyDown(KeyCode.Space)) CanSpawnEffect();
    }

    public void CanSpawnEffect()
    {
        if (vfx != null)
        {
            // 1. 先讀取目前 VFX 中該布林值的狀態
            bool currentValue = vfx.GetBool(boolPropertyName);
            
            // 2. 將狀態反轉 (!currentValue) 後，重新寫入 VFX 中
            vfx.SetBool(boolPropertyName, !currentValue);
            
            // 可選：在 Console 印出目前的狀態方便除錯
            Debug.Log($"MagicDom: {boolPropertyName} 已切換為 {!currentValue}");
        }
        else
        {
            Debug.LogWarning("MagicDom: 找不到 VisualEffect 元件！");
        }
    }
}