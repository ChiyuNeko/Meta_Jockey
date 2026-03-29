using UnityEngine;

// 關鍵 1：這個標籤讓腳本在沒有按 Play 的編輯模式下也能執行
[ExecuteAlways] 
[RequireComponent(typeof(Renderer))]
public class MaterialOverride : MonoBehaviour
{
    [Header("自訂材質數值")]
    // 在這裡宣告你想要在 Inspector 面板中獨立調整的數值
    public Color customColor = Color.white;
    public float customValue = 1.0f;
    public float  AlphaValue = 8f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    // 關鍵 2：OnValidate 會在你在 Inspector 拖拉數值時即時觸發更新
    void OnValidate()
    {
        ApplyProperties();
    }

    void OnEnable()
    {
        ApplyProperties();
    }

    void ApplyProperties()
    {
        // 初始化
        if (rend == null) rend = GetComponent<Renderer>();
        if (propBlock == null) propBlock = new MaterialPropertyBlock();

        // 讀取物件目前的 PropertyBlock（避免覆蓋掉其他程式設定好的值）
        rend.GetPropertyBlock(propBlock);

        // 關鍵 3：設定你要覆寫的屬性，字串必須跟 Shader 裡的 "Reference" 完全一致
        propBlock.SetColor("_Color", customColor); 
        propBlock.SetFloat("_light_Strengh", customValue); // 請改成你 Shader 裡的變數名稱
        propBlock.SetFloat("_Alpha", AlphaValue); // 請改成你 Shader 裡的變數名稱

        // 將覆寫套用回物件上
        rend.SetPropertyBlock(propBlock);
    }
}