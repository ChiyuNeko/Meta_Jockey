using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceController : MonoBehaviour
{
    [Header("物件清單")]
    [Tooltip("請將你要控制的物件拖曳到這個 List 中")]
    public List<GameObject> objectList = new List<GameObject>();

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
    public int loopCount = 2;

    [Header("視覺重疊設定")]
    [Range(0f, 0.8f)]
    [Tooltip("燈光延長亮起的時間比例。")]
    public float overlapRatio = 0.5f; 

    // 用來防呆的鎖定變數，設為 public 方便外部讀取狀態
    [HideInInspector] 
    public bool isPlaying = false; 

    private void OnValidate()
    {
        if (mode1_Sequential) { mode2_Pairs = false; mode3_Random = false; }
        else if (mode2_Pairs) { mode1_Sequential = false; mode3_Random = false; }
        else if (mode3_Random) { mode1_Sequential = false; mode2_Pairs = false; }
    }

    void Start()
    {
        TurnOffAllObjects();
    }

    // ==========================================
    // ★ 新增：方便開發測試用的鍵盤觸發
    // ==========================================
    void Update()
    {
        // 按下 P 鍵即可觸發測試 (會共用下方的防呆機制)
        if (Input.GetKeyDown(KeyCode.P))
        {
            TriggerSequence();
        }
    }

    // ==========================================
    // Public 觸發方法，供外部腳本或 UI 按鈕呼叫
    // ==========================================
    public void TriggerSequence()
    {
        // 防呆機制：如果目前正在播放中，直接略過本次呼叫
        if (isPlaying) return;

        if (objectList.Count == 0)
        {
            Debug.LogWarning("SequenceController: 你的 List 裡面沒有放東西喔！");
            return;
        }

        float durationPerLoop = (60f / bpm) * beatsPerSequence;

        if (mode1_Sequential) StartCoroutine(PlayMode1(durationPerLoop));
        else if (mode2_Pairs) StartCoroutine(PlayMode2(durationPerLoop));
        else if (mode3_Random) StartCoroutine(PlayMode3(durationPerLoop));
    }

    private void ActivateObject(GameObject obj, float activeDuration)
    {
        obj.SetActive(true); 
        
        LightEffectController effect = obj.GetComponent<LightEffectController>();
        if (effect != null)
        {
            effect.PlayEffect(activeDuration);
        }
    }

    // --- 模式一 ---
    private IEnumerator PlayMode1(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = objectList.Count + 1;
        
        float waitTime = durationPerLoop / totalSteps; 
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                ActivateObject(objectList[i], effectDuration);
                yield return new WaitForSeconds(waitTime);
            }

            TurnOnAllObjects(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        FinishSequence();
    }

    // --- 模式二 ---
    private IEnumerator PlayMode2(float durationPerLoop)
    {
        isPlaying = true;
        int pairSteps = Mathf.CeilToInt(objectList.Count / 2.0f);
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
                
                if (curr1 < objectList.Count) ActivateObject(objectList[curr1], effectDuration); 
                if (curr2 < objectList.Count) ActivateObject(objectList[curr2], effectDuration); 
                
                yield return new WaitForSeconds(waitTime);
            }
            yield return new WaitForSeconds(waitTime);

            for (int i = 0; i < pairSteps; i++)
            {
                int curr1 = i * 2;
                int curr2 = curr1 + 1;
                
                if (curr1 < objectList.Count) ActivateObject(objectList[curr1], effectDuration); 
                if (curr2 < objectList.Count) ActivateObject(objectList[curr2], effectDuration); 
                
                yield return new WaitForSeconds(waitTime);
            }
            TurnOnAllObjects(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        FinishSequence();
    }

    // --- 模式三 ---
    private IEnumerator PlayMode3(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = objectList.Count + 1;
        
        float waitTime = durationPerLoop / totalSteps;
        float effectDuration = waitTime * (1f + overlapRatio); 

        for (int loop = 0; loop < loopCount; loop++)
        {
            List<int> randomIndices = new List<int>();
            for (int i = 0; i < objectList.Count; i++) randomIndices.Add(i);
            
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
                ActivateObject(objectList[currentIndex], effectDuration); 
                yield return new WaitForSeconds(waitTime);
            }

            TurnOnAllObjects(effectDuration); 
            yield return new WaitForSeconds(waitTime);
        }

        if (overlapRatio > 0) yield return new WaitForSeconds(waitTime * overlapRatio);
        FinishSequence();
    }

    // --- 共用方法 ---
    private void TurnOffAllObjects()
    {
        foreach (GameObject obj in objectList)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private void TurnOnAllObjects(float duration)
    {
        foreach (GameObject obj in objectList)
        {
            if (obj != null) ActivateObject(obj, duration);
        }
    }

    private void FinishSequence()
    {
        TurnOffAllObjects();
        isPlaying = false; 
    }
}