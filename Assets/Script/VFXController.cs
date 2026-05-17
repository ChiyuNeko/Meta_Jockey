using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; 

public class VFXController : MonoBehaviour
{
    [Header("VFX 特效清單")]
    [Tooltip("請將你的 Visual Effect 物件拖曳到這個 List 中")]
    public List<VisualEffect> vfxList = new List<VisualEffect>();

    [Header("模式設定 (自動互斥，一次只能勾選一個)")]
    public bool mode1_Sequential = true; 
    public bool mode2_Pairs = false;     
    public bool mode3_Random = false;    

    [Header("節奏與循環設定")]
    [Tooltip("每分鐘節拍數 (BPM)")]
    public float bpm = 120f;
    [Tooltip("跑完「一圈」要佔用幾拍？")]
    public float beatsPerSequence = 1f;
    [Tooltip("每次觸發要連續循環跑幾圈？")]
    public int loopCount = 1; // 預設改為 1

    [Header("視覺重疊設定")]
    [Range(0f, 16f)]
    [Tooltip("特效延長噴發的時間比例。")]
    public float overlapRatio = 0.5f; 

    [Header("結尾加花設定")]
    [Tooltip("是否在每圈的最後一段，觸發「全部一起亮」的效果？")]
    public bool triggerAllAtEnd = true; 

    [HideInInspector] 
    public bool isPlaying = false; 

    private Dictionary<VisualEffect, Coroutine> vfxStopRoutines = new Dictionary<VisualEffect, Coroutine>();

    private void OnValidate()
    {
        if (mode1_Sequential) { mode2_Pairs = false; mode3_Random = false; }
        else if (mode2_Pairs) { mode1_Sequential = false; mode3_Random = false; }
        else if (mode3_Random) { mode1_Sequential = false; mode2_Pairs = false; }
    }

    void Start()
    {
        StopAllVFX();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerSequence();
        }
    }

    public void TriggerSequence()
    {
        if (isPlaying) return;

        if (vfxList.Count == 0)
        {
            Debug.LogWarning("SequenceController: 你的 VFX List 裡面沒有放東西喔！");
            return;
        }

        float durationPerLoop = (60f / bpm) * beatsPerSequence;

        if (mode1_Sequential) StartCoroutine(PlayMode1(durationPerLoop));
        else if (mode2_Pairs) StartCoroutine(PlayMode2(durationPerLoop));
        else if (mode3_Random) StartCoroutine(PlayMode3(durationPerLoop));
    }

    private void ActivateVFX(VisualEffect vfx, float activeDuration)
    {
        if (vfx == null) return;

        if (vfxStopRoutines.ContainsKey(vfx) && vfxStopRoutines[vfx] != null)
        {
            StopCoroutine(vfxStopRoutines[vfx]);
        }

        vfx.Play();
        vfxStopRoutines[vfx] = StartCoroutine(StopVFXAfterDelay(vfx, activeDuration));
    }

    private IEnumerator StopVFXAfterDelay(VisualEffect vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (vfx != null)
        {
            vfx.Stop();
        }
    }

    // --- 模式一 ---
    private IEnumerator PlayMode1(float durationPerLoop)
    {
        isPlaying = true;
        
        int totalSteps = triggerAllAtEnd ? vfxList.Count + 1 : vfxList.Count;
        float waitTime = durationPerLoop / totalSteps; 
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < vfxList.Count; i++)
            {
                ActivateVFX(vfxList[i], effectDuration);
                yield return new WaitForSeconds(waitTime);
            }

            if (triggerAllAtEnd)
            {
                PlayAllVFX(effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        FinishSequence();
    }

    // --- 模式二 (已修復：移除多餘雙重迴圈) ---
    private IEnumerator PlayMode2(float durationPerLoop)
    {
        isPlaying = true;
        int pairSteps = Mathf.CeilToInt(vfxList.Count / 2.0f);
        
        // ★ 修正：讓總步數回歸單一迴圈的數量，不再乘以 2
        int totalSteps = triggerAllAtEnd ? pairSteps + 1 : pairSteps; 
        
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < pairSteps; i++)
            {
                int curr1 = i * 2;
                int curr2 = curr1 + 1;
                
                if (curr1 < vfxList.Count) ActivateVFX(vfxList[curr1], effectDuration); 
                if (curr2 < vfxList.Count) ActivateVFX(vfxList[curr2], effectDuration); 
                
                yield return new WaitForSeconds(waitTime);
            }
            
            // ★ 修正：一輪跑完後直接進入結尾判斷，不再重複跑一次
            if (triggerAllAtEnd)
            {
                PlayAllVFX(effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        FinishSequence();
    }

    // --- 模式三 ---
    private IEnumerator PlayMode3(float durationPerLoop)
    {
        isPlaying = true;
        
        int totalSteps = triggerAllAtEnd ? vfxList.Count + 1 : vfxList.Count;
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            List<int> randomIndices = new List<int>();
            for (int i = 0; i < vfxList.Count; i++) randomIndices.Add(i);
            
            for (int i = 0; i < randomIndices.Count; i++)
            {
                int temp = randomIndices[i];
                int randomIndex = Random.Range(i, randomIndices.Count);
                randomIndices[i] = randomIndices[randomIndex];
                randomIndices[randomIndex] = temp;
            }

            for (int i = 0; i < randomIndices.Count; i++)
            {
                ActivateVFX(vfxList[randomIndices[i]], effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }

            if (triggerAllAtEnd)
            {
                PlayAllVFX(effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        FinishSequence();
    }

    // --- 共用方法 ---
    private void StopAllVFX()
    {
        foreach (VisualEffect vfx in vfxList)
        {
            if (vfx != null)
            {
                vfx.Stop();
            }
        }
        vfxStopRoutines.Clear();
    }

    private void PlayAllVFX(float duration)
    {
        foreach (VisualEffect vfx in vfxList)
        {
            if (vfx != null) ActivateVFX(vfx, duration);
        }
    }

    private void FinishSequence()
    {
        StopAllVFX();
        isPlaying = false; 
    }
}