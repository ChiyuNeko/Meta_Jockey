using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissionController : MonoBehaviour
{
    [Header("隨機顏色池 (支援 HDR 發光)")]
    [ColorUsage(true, true)] public Color color1 = Color.white;
    [ColorUsage(true, true)] public Color color2 = Color.red;
    [ColorUsage(true, true)] public Color color3 = Color.green;
    [ColorUsage(true, true)] public Color color4 = Color.blue;
    [ColorUsage(true, true)] public Color color5 = Color.yellow;

    [Header("碰撞變色設定")]
    public LayerMask targetLayer; // 觸發變色的特定 Layer
    [ColorUsage(true, true)] public Color hitColor = Color.cyan; // 碰撞後變成的顏色
    public float hitDuration = 2.0f; // 維持變色的秒數

    private Material mat;
    private Color selectedRandomColor; // 紀錄一開始抽到的顏色
    private Coroutine colorRoutine;

    void Start()
    {
        // 取得物件上的 Material
        mat = GetComponent<Renderer>().material;

        // 確保材質的發光功能有被開啟
        mat.EnableKeyword("_EMISSION");

        // 將五個顏色變數放入一個陣列中
        Color[] colorPool = new Color[] { color1, color2, color3, color4, color5 };

        // 隨機產生一個 0 到 4 的整數索引
        int randomIndex = Random.Range(0, colorPool.Length);
        
        // 根據抽到的索引決定顏色
        selectedRandomColor = colorPool[randomIndex];

        // 設定初始隨機發光顏色
        SetEmissionColor(selectedRandomColor);
    }

    // 將 OnCollisionEnter 改為 OnTriggerEnter，並將參數改為 Collider other
    void OnTriggerEnter(Collider other)
    {
        // 檢查穿過觸發區的物件 Layer 是否包含在我們指定的 targetLayer 中
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            TriggerColorChange();
        }
    }

    private void TriggerColorChange()
    {
        // 如果目前已經在倒數變色，先停止它，避免多次觸發導致計時錯亂
        if (colorRoutine != null)
        {
            StopCoroutine(colorRoutine);
        }
        
        // 開始變色倒數的協程
        colorRoutine = StartCoroutine(ChangeColorTemporarily());
    }

    private IEnumerator ChangeColorTemporarily()
    {
        // 變成觸發後指定的顏色
        SetEmissionColor(hitColor);

        // 等待指定的秒數
        yield return new WaitForSeconds(hitDuration);

        // 秒數到後，恢復成一開始抽到的顏色
        SetEmissionColor(selectedRandomColor);
    }

    // 封裝設定 Emission 顏色的方法
    private void SetEmissionColor(Color color)
    {
        if (mat != null)
        {
            mat.SetColor("_EmissionColor", color);
        }
    }
}