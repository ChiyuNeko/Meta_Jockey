using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReact : MonoBehaviour
{
    [Header("Debug 即時監控設定")]
    [Tooltip("想觀察哪一個頻段 (0 ~ Bands-1)")]
    public int DebugBandIndex = 0;

    [Tooltip("此欄位會即時顯示選定頻段的數值")]
    public float DebugBandValue = 0f;


    [Header("FFT 分析器")]
    public FrequencyBandAnalyser _FFT;
    public FrequencyBandAnalyser.Bands _FreqBands = FrequencyBandAnalyser.Bands.Eight;


    // ---------------------------------------------------------
    // ⭐⭐ 新功能：材質控制 ⭐⭐
    // ---------------------------------------------------------
    [Header("材質控制（可選）")]
    [Tooltip("指定你想控制材質的物件")]
    public GameObject TargetMaterialObject;

    [Tooltip("輸入要控制的材質屬性名稱，例如 _EmissionStrength 或 _MyFloat")]
    public string MaterialFloatName = "_MyFloat";

    [Tooltip("使用哪一個頻段控制材質數值")]
    public int MaterialBandIndex = 0;

    [Tooltip("頻段值乘以此倍率後再寫入材質")]
    public float MaterialMultiplier = 1f;

    Material _targetMaterial;   // 自動抓取的材質
    // ---------------------------------------------------------


    [Header("生成物件設定")]
    GameObject[] _FFTGameObjects;
    public GameObject _ObjectToSpawn;
    public float _Spacing = 1;

    [Header("縮放設定")]
    Vector3 _BaseScale;
    public Vector3 _ScalingStrength = Vector3.up;


    void Start()
    {
        // 依照頻段數量建立物件
        _FFTGameObjects = new GameObject[(int)_FreqBands];
        _BaseScale = _ObjectToSpawn.transform.localScale;

        for (int i = 0; i < _FFTGameObjects.Length; i++)
        {
            GameObject newFFTObject = Instantiate(_ObjectToSpawn);
            newFFTObject.transform.SetParent(transform);
            newFFTObject.transform.localPosition = new Vector3(i * _Spacing, 0, 0);
            _FFTGameObjects[i] = newFFTObject;
        }

        // ⭐ 自動取得材質
        if (TargetMaterialObject != null)
        {
            Renderer r = TargetMaterialObject.GetComponent<Renderer>();
            if (r != null) _targetMaterial = r.material;
        }
    }


    void Update()
    {
        // ⭐ 即時監控頻段
        if (_FFT != null)
        {
            int maxIndex = (int)_FreqBands - 1;
            DebugBandIndex = Mathf.Clamp(DebugBandIndex, 0, maxIndex);

            DebugBandValue = _FFT.GetBandValue(DebugBandIndex, _FreqBands);
        }


        // ⭐ 控制材質參數
        if (_targetMaterial != null && !string.IsNullOrEmpty(MaterialFloatName))
        {
            int maxIndex = (int)_FreqBands - 1;
            MaterialBandIndex = Mathf.Clamp(MaterialBandIndex, 0, maxIndex);

            float v = _FFT.GetBandValue(MaterialBandIndex, _FreqBands);

            // 寫入材質
            _targetMaterial.SetFloat(MaterialFloatName, v * MaterialMultiplier);
        }


        // ⭐ 原本的視覺化縮放
        for (int i = 0; i < _FFTGameObjects.Length; i++)
        {
            _FFTGameObjects[i].transform.localScale =
                _BaseScale +
                (_ScalingStrength * _FFT.GetBandValue(i, _FreqBands));
        }
    }
}
