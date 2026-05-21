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
    // ⭐⭐ 新功能：多物件材質控制 ⭐⭐
    // ---------------------------------------------------------
    [Header("材質控制（可選）")]
    [Tooltip("指定你想控制材質的物件們 (在面板上可設定數量，例如 3)")]
    public GameObject[] TargetMaterialObjects = new GameObject[3];

    [Tooltip("輸入要控制的材質屬性名稱，例如 _EmissionStrength 或 _MyFloat")]
    public string MaterialFloatName = "_MyFloat";

    [Tooltip("使用哪一個頻段控制材質數值")]
    public int MaterialBandIndex = 0;

    [Tooltip("頻段值乘以此倍率後再寫入材質")]
    public float MaterialMultiplier = 1f;

    // 用來儲存抓取到的多個材質
    List<Material> _targetMaterials = new List<Material>(); 
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

        // ⭐ 自動取得陣列中所有物件的材質
        if (TargetMaterialObjects != null && TargetMaterialObjects.Length > 0)
        {
            for (int i = 0; i < TargetMaterialObjects.Length; i++)
            {
                // 確保該欄位有放入物件
                if (TargetMaterialObjects[i] != null) 
                {
                    Renderer r = TargetMaterialObjects[i].GetComponent<Renderer>();
                    if (r != null)
                    {
                        // 將材質加入清單中統一管理
                        _targetMaterials.Add(r.material); 
                    }
                }
            }
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

        // ⭐ 控制所有指定的材質參數
        if (_targetMaterials.Count > 0 && !string.IsNullOrEmpty(MaterialFloatName))
        {
            int maxIndex = (int)_FreqBands - 1;
            MaterialBandIndex = Mathf.Clamp(MaterialBandIndex, 0, maxIndex);

            float v = _FFT.GetBandValue(MaterialBandIndex, _FreqBands);
            float finalValue = v * MaterialMultiplier;

            // 迴圈寫入每一個材質
            for (int i = 0; i < _targetMaterials.Count; i++)
            {
                if (_targetMaterials[i] != null)
                {
                    _targetMaterials[i].SetFloat(MaterialFloatName, finalValue);
                }
            }
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