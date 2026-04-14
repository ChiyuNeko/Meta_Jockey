using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSequenceController : MonoBehaviour
{
    [Header("材質特效清單 (Texture 專屬大腦)")]
    [Tooltip("請將你要控制的 TextureEffectController 拖曳到這個 List 中")]
    public List<TextureEffectController> effectList = new List<TextureEffectController>();

    [Header("模式設定 (自動互斥，一次只能勾選一個)")]
    public bool mode1_Sequential = true; 
    public bool mode2_Pairs = false;     
    public bool mode3_Random = false;    

    [Header("節奏與循環設定")]
    public float bpm = 120f;
    public float beatsPerSequence = 1f;
    public int loopCount = 2;

    [Header("視覺重疊設定")]
    [Range(0f, 0.8f)]
    public float overlapRatio = 0.5f; 

    [HideInInspector] 
    public bool isPlaying = false; 

    private void OnValidate()
    {
        if (mode1_Sequential) { mode2_Pairs = false; mode3_Random = false; }
        else if (mode2_Pairs) { mode1_Sequential = false; mode3_Random = false; }
        else if (mode3_Random) { mode1_Sequential = false; mode2_Pairs = false; }
    }

    void Update()
    {
        // 測試用：按下 P 鍵觸發
        if (Input.GetKeyDown(KeyCode.L))
        {
            TriggerSequence();
        }
    }

    public void TriggerSequence()
    {
        if (isPlaying) return;
        if (effectList.Count == 0) return;

        float durationPerLoop = (60f / bpm) * beatsPerSequence;

        if (mode1_Sequential) StartCoroutine(PlayMode1(durationPerLoop));
        else if (mode2_Pairs) StartCoroutine(PlayMode2(durationPerLoop));
        else if (mode3_Random) StartCoroutine(PlayMode3(durationPerLoop));
    }

    // ★ 核心改變：不再開關 GameObject，而是直接發送「播放特效」的指令
    private void TriggerAllEffects(float duration)
    {
        foreach (var effect in effectList)
        {
            if (effect != null) effect.PlayEffect(duration);
        }
    }

    // --- 模式一 ---
    private IEnumerator PlayMode1(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = effectList.Count + 1;
        
        float waitTime = durationPerLoop / totalSteps; 
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                if (effectList[i] != null) effectList[i].PlayEffect(effectDuration);
                yield return new WaitForSeconds(waitTime);
            }

            TriggerAllEffects(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        isPlaying = false; // 解除鎖定
    }

    // --- 模式二 ---
    private IEnumerator PlayMode2(float durationPerLoop)
    {
        isPlaying = true;
        int pairSteps = Mathf.CeilToInt(effectList.Count / 2.0f);
        int stepsPerPass = pairSteps + 1; 
        int totalSteps = stepsPerPass * 2; 
        
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < pairSteps; i++)
            {
                int curr1 = i * 2;
                int curr2 = curr1 + 1;
                
                if (curr1 < effectList.Count && effectList[curr1] != null) effectList[curr1].PlayEffect(effectDuration); 
                if (curr2 < effectList.Count && effectList[curr2] != null) effectList[curr2].PlayEffect(effectDuration); 
                
                yield return new WaitForSeconds(waitTime);
            }
            // 第一趟結尾 (全暗等待)
            yield return new WaitForSeconds(waitTime);

            for (int i = 0; i < pairSteps; i++)
            {
                int curr1 = i * 2;
                int curr2 = curr1 + 1;
                
                if (curr1 < effectList.Count && effectList[curr1] != null) effectList[curr1].PlayEffect(effectDuration); 
                if (curr2 < effectList.Count && effectList[curr2] != null) effectList[curr2].PlayEffect(effectDuration); 
                
                yield return new WaitForSeconds(waitTime);
            }
            // 第二趟結尾 (全亮)
            TriggerAllEffects(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        isPlaying = false;
    }

    // --- 模式三 ---
    private IEnumerator PlayMode3(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = effectList.Count + 1;
        
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            List<int> randomIndices = new List<int>();
            for (int i = 0; i < effectList.Count; i++) randomIndices.Add(i);
            
            for (int i = 0; i < randomIndices.Count; i++)
            {
                int temp = randomIndices[i];
                int randomIndex = Random.Range(i, randomIndices.Count);
                randomIndices[i] = randomIndices[randomIndex];
                randomIndices[randomIndex] = temp;
            }

            for (int i = 0; i < randomIndices.Count; i++)
            {
                int currentIndex = randomIndices[i];
                if (effectList[currentIndex] != null) effectList[currentIndex].PlayEffect(effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }

            TriggerAllEffects(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        isPlaying = false;
    }
}