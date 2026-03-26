using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 移除了 RequireComponent，因為現在腳本是掛在母物件上，不一定要有 Renderer
public class EmissionController : MonoBehaviour
{
    [Header("目標螢光棒物件")]
    [Tooltip("請將第一根螢光棒的 GameObject 拖放到這裡")]
    public GameObject glowstick1; 
    [Tooltip("請將第二根螢光棒的 GameObject 拖放到這裡")]
    public GameObject glowstick2; 

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

    private Material mat1;
    private Material mat2;
    private Color selectedRandomColor; // 紀錄一開始抽到的顏色
    private Coroutine colorRoutine;

    void Start()
    {
        // 取得第一根螢光棒的 Material，並確保發光功能開啟
        if (glowstick1 != null)
        {
            mat1 = glowstick1.GetComponent<Renderer>().material;
            mat1.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("EmissionController: 你還沒指派第一根螢光棒喔！");
        }

        // 取得第二根螢光棒的 Material，並確保發光功能開啟
        if (glowstick2 != null)
        {
            mat2 = glowstick2.GetComponent<Renderer>().material;
            mat2.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogWarning("EmissionController: 你還沒指派第二根螢光棒喔！");
        }

        // 將五個顏色變數放入一個陣列中
        Color[] colorPool = new Color[] { color1, color2, color3, color4, color5 };

        // 隨機產生一個 0 到 4 的整數索引
        int randomIndex = Random.Range(0, colorPool.Length);
        
        // 根據抽到的索引決定顏色
        selectedRandomColor = colorPool[randomIndex];

        // 設定初始隨機發光顏色 (兩根螢光棒會同時套用同一個隨機顏色)
        SetEmissionColor(selectedRandomColor);
    }

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

    // 封裝設定 Emission 顏色的方法，同時應用到兩根螢光棒上
    private void SetEmissionColor(Color color)
    {
        if (mat1 != null)
        {
            mat1.SetColor("_EmissionColor", color);
        }

        if (mat2 != null)
        {
            mat2.SetColor("_EmissionColor", color);
        }
    }
}