using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundReact : MonoBehaviour
{
    public float A;
    // Start is called before the first frame update
    [Header("FFT 分析器")]
    public FrequencyBandAnalyser _FFT;                     // 取得頻段數值
    public FrequencyBandAnalyser.Bands _FreqBands = FrequencyBandAnalyser.Bands.Eight;

    [Header("生成物件設定")]
    GameObject[] _FFTGameObjects;                          // 存放所有生成出來的物件
    public GameObject _ObjectToSpawn;                      // 要生成的物件 Prefab
    public float _Spacing = 1;                             // 物件之間的間隔

    [Header("縮放設定")]
    Vector3 _BaseScale;                                    // 物件的原始大小
    public Vector3 _ScalingStrength = Vector3.up;          // 要放大的方向（通常是向上）

    // Start is called before the first frame update
    void Start()
    {
        // 依照頻段數量建立物件陣列
        _FFTGameObjects = new GameObject[(int)_FreqBands];

        // 記錄 prefab 初始大小
        _BaseScale = _ObjectToSpawn.transform.localScale;

        for (int i = 0; i < _FFTGameObjects.Length; i++)
        {
            GameObject newFFTObject = Instantiate(_ObjectToSpawn);

            // 設為本物件的子物件
            newFFTObject.transform.SetParent(transform);

            // 排列位置：x = i * 間隔
            newFFTObject.transform.localPosition = new Vector3(i * _Spacing, 0, 0);

            _FFTGameObjects[i] = newFFTObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _FFTGameObjects.Length; i++)
        {
            // 根據頻段值改變物件縮放
            _FFTGameObjects[i].transform.localScale =
                _BaseScale +
                (_ScalingStrength * _FFT.GetBandValue(i, _FreqBands));
        }
    }
}


