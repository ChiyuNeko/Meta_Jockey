using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyBandAnalyser : MonoBehaviour
{
    public enum Bands
    {
        Three = 3,
        Eight = 8,
        SixtyFour = 64,
    }

    int _FrequencyBins = 512;

    float[] _Samples;
    float[] _SampleBuffer;

    public float _SmoothDownRate = 0;
    public float _Scalar = 1;

    public bool _DrawGizmos = false;

    [HideInInspector] public float[] _FreqBands3;
    [HideInInspector] public float[] _FreqBands8;
    [HideInInspector] public float[] _FreqBands64;

    void Start()
    {
        _Samples = new float[_FrequencyBins];
        _SampleBuffer = new float[_FrequencyBins];

        _FreqBands3 = new float[3];
        _FreqBands8 = new float[8];
        _FreqBands64 = new float[64];
    }

    // ---------------------------
    //  三段 Low / Mid / High
    // ---------------------------
    void UpdateFreqBands3()
    {
        float low = 0, mid = 0, high = 0;

        int lowEnd = 6;   // 0–5
        int midEnd = 47;  // 6–46
        int highEnd = 512;

        for (int i = 0; i < lowEnd; i++)
            low += _Samples[i] * (i + 1);
        low /= lowEnd;

        for (int i = lowEnd; i < midEnd; i++)
            mid += _Samples[i] * (i + 1);
        mid /= (midEnd - lowEnd);

        for (int i = midEnd; i < highEnd; i++)
            high += _Samples[i] * (i + 1);
        high /= (highEnd - midEnd);

        _FreqBands3[0] = low;
        _FreqBands3[1] = mid;
        _FreqBands3[2] = high;
    }

    // ---------------------------
    //  八段（原本的）
    // ---------------------------
    void UpdateFreqBands8()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
                sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += _Samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _FreqBands8[i] = average;
        }
    }

    // ---------------------------
    //  六十四段（原本的）
    // ---------------------------
    void UpdateFreqBands64()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < 64; i++)
        {
            float average = 0;

            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 3)
                    sampleCount -= 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _Samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _FreqBands64[i] = average;
        }
    }

    // ---------------------------
    //  Main Update
    // ---------------------------
    void Update()
    {
        // ⭐⭐⭐ 這裡是最重要的修改：抓 Master Output ⭐⭐⭐
        AudioListener.GetSpectrumData(_SampleBuffer, 0, FFTWindow.BlackmanHarris);

        // 平滑
        for (int i = 0; i < _Samples.Length; i++)
        {
            if (_SampleBuffer[i] > _Samples[i])
                _Samples[i] = _SampleBuffer[i];
            else
                _Samples[i] = Mathf.Lerp(_Samples[i], _SampleBuffer[i], Time.deltaTime * _SmoothDownRate);
        }

        UpdateFreqBands3();
        UpdateFreqBands8();
        UpdateFreqBands64();
    }

    // ---------------------------
    //  取得頻率段值
    // ---------------------------
    public float GetBandValue(int index, Bands bands)
    {
        switch (bands)
        {
            case Bands.Three:
                return _FreqBands3[index];
            case Bands.Eight:
                return _FreqBands8[index];
            case Bands.SixtyFour:
                return _FreqBands64[index];
        }
        return 0;
    }

    // ---------------------------
    //  Gizmos 顯示（原樣保留）
    // ---------------------------
    private void OnDrawGizmos()
    {
        if (_DrawGizmos && Application.isPlaying)
        {
            for (int i = 1; i < 63; i++)
            {
                float x0 = (float)(i - 1) / 63f;
                float x1 = (float)i / 63f;

                Gizmos.color = Color.white;
                Gizmos.DrawLine(
                    transform.position + new Vector3(x0 * 4, _FreqBands64[i - 1] * _Scalar, 0),
                    transform.position + new Vector3(x1 * 4, _FreqBands64[i] * _Scalar, 0)
                );
            }
        }
    }

    public float GetMasterValue()
    {
        float sum = 0f;

        for (int i = 0; i < _Samples.Length; i++)
        {
            sum += _Samples[i];
        }

        return sum / _Samples.Length;
    }
}
