using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BPMRotator : MonoBehaviour
{
    public BPMSpawner bpmSpawner;
    [Header("材質設定")]
    public Material material;
    public float initialAlpha = 0.178f;
    public float alphaScale = 0.178f;
    public float decreasSpeed = 1.0f;
    [Header("旋轉設定")]
    public float maxAngle = 45f;
    public float bpm = 60f;

    [Header("邊緣懸停感 (數值越大，邊緣停留越久)")]
    [Range(1f, 5f)]
    public float sharpness = 2.0f; 

    [Header("軸向設定")]
    public Vector3 rotationAxis = Vector3.up; 

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
        if(bpmSpawner != null)
            bpm = bpmSpawner.bpm;
    }

    void Update()
    {
        // 基礎頻率計算
        float frequency = bpm / 480f;
        
        // 1. 取得標準正弦波 (-1 到 1)
        float rawSin = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f);

        // 2. 使用冪函數處理，讓曲線在邊緣變得平坦
        // 我們使用 Mathf.Abs 處理後再補回正負號，以保持對稱
        float smoothSin = Mathf.Sign(rawSin) * (1f - Mathf.Pow(1f - Mathf.Abs(rawSin), sharpness));

        // 3. 套用旋轉
        float angleOffset = smoothSin * maxAngle;
        Quaternion offsetRotation = Quaternion.Euler(rotationAxis * angleOffset);
        transform.localRotation = initialRotation * offsetRotation;
        // 4. 更新材質透明度
        if (material != null)
            material.SetFloat("_Alpha", alphaScale);
        alphaScale = Mathf.Lerp (alphaScale, 0, Time.deltaTime * decreasSpeed);
    }
    public void ResetAlpha()
    {
        alphaScale = initialAlpha;
    }
}