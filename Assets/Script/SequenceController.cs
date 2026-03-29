using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceController : MonoBehaviour
{
    [Header("物件清單")]
    [Tooltip("請將你要控制的物件拖曳到這個 List 中")]
    public List<GameObject> objectList = new List<GameObject>();

    [Header("模式設定 (自動互斥，一次只能勾選一個)")]
    public bool mode1_Sequential = true; // 預設：單個按順序
    public bool mode2_Pairs = false;     // 更新：兩兩一組 -> 結尾全暗 -> 兩兩一組 -> 結尾全亮
    public bool mode3_Random = false;    // 預設：隨機順序

    [Header("節奏與循環設定")]
    [Tooltip("每分鐘節拍數 (BPM)")]
    public float bpm = 120f;
    [Tooltip("跑完「一圈」要佔用幾拍？")]
    public float beatsPerSequence = 1f;
    [Tooltip("每次觸發要連續循環跑幾圈？")]
    public int loopCount = 2;

    private bool isPlaying = false; 

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
        {
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
    }

    // --- 模式一：單個按順序 + 結尾全亮 ---
    private IEnumerator PlayMode1(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = objectList.Count + 1;
        float waitTime = durationPerLoop / totalSteps;
        
        GameObject lastActiveObj = null;

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                if (i == 0) TurnOffAllObjects(); 
                else if (lastActiveObj != null) lastActiveObj.SetActive(false);
                
                objectList[i].SetActive(true);
                lastActiveObj = objectList[i];

                yield return new WaitForSeconds(waitTime);
            }

            TurnOffAllObjects(); 
            TurnOnAllObjects();  
            yield return new WaitForSeconds(waitTime);
        }

        FinishSequence();
    }

    // --- 模式二：兩兩一組 -> 結尾全暗 -> 兩兩一組 -> 結尾全亮 ---
    private IEnumerator PlayMode2(float durationPerLoop)
    {
        isPlaying = true;
        
        // 計算兩兩一組需要幾步 (例如 5 個物件 = 3步)
        int pairSteps = Mathf.CeilToInt(objectList.Count / 2.0f);
        
        // 每個階段的步數 = 組合的步數 + 1次結尾 (全暗或全亮)
        int stepsPerPass = pairSteps + 1; 
        
        // 總步數 = 兩個階段相加
        int totalSteps = stepsPerPass * 2; 
        
        float waitTime = durationPerLoop / totalSteps;
        
        List<GameObject> lastActiveGroup = new List<GameObject>();

        for (int loop = 0; loop < loopCount; loop++)
        {
            // === 第一階段：兩兩一組 -> 結尾全暗 ===
            for (int i = 0; i < pairSteps; i++)
            {
                if (i == 0) TurnOffAllObjects(); 
                else
                {
                    foreach (var obj in lastActiveGroup) obj.SetActive(false);
                }
                lastActiveGroup.Clear();

                int curr1 = i * 2;
                int curr2 = curr1 + 1;
                
                if (curr1 < objectList.Count) 
                { 
                    objectList[curr1].SetActive(true); 
                    lastActiveGroup.Add(objectList[curr1]); 
                }
                if (curr2 < objectList.Count) 
                { 
                    objectList[curr2].SetActive(true); 
                    lastActiveGroup.Add(objectList[curr2]); 
                }

                yield return new WaitForSeconds(waitTime);
            }
            // 第一階段結尾：全暗
            TurnOffAllObjects();
            lastActiveGroup.Clear(); // 進入下一階段前清空暫存
            yield return new WaitForSeconds(waitTime);


            // === 第二階段：兩兩一組 -> 結尾全亮 ===
            for (int i = 0; i < pairSteps; i++)
            {
                if (i == 0) TurnOffAllObjects(); 
                else
                {
                    foreach (var obj in lastActiveGroup) obj.SetActive(false);
                }
                lastActiveGroup.Clear();

                int curr1 = i * 2;
                int curr2 = curr1 + 1;
                
                if (curr1 < objectList.Count) 
                { 
                    objectList[curr1].SetActive(true); 
                    lastActiveGroup.Add(objectList[curr1]); 
                }
                if (curr2 < objectList.Count) 
                { 
                    objectList[curr2].SetActive(true); 
                    lastActiveGroup.Add(objectList[curr2]); 
                }

                yield return new WaitForSeconds(waitTime);
            }
            // 第二階段結尾：全亮
            TurnOffAllObjects(); 
            TurnOnAllObjects();
            lastActiveGroup.Clear();
            yield return new WaitForSeconds(waitTime);
        }

        FinishSequence();
    }

    // --- 模式三：隨機順序 + 結尾全亮 ---
    private IEnumerator PlayMode3(float durationPerLoop)
    {
        isPlaying = true;
        int totalSteps = objectList.Count + 1;
        float waitTime = durationPerLoop / totalSteps;
        
        GameObject lastActiveObj = null;

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
                if (i == 0) TurnOffAllObjects();
                else if (lastActiveObj != null) lastActiveObj.SetActive(false);
                
                int currentIndex = randomIndices[i];
                objectList[currentIndex].SetActive(true);
                lastActiveObj = objectList[currentIndex];

                yield return new WaitForSeconds(waitTime);
            }

            TurnOffAllObjects(); 
            TurnOnAllObjects();  
            yield return new WaitForSeconds(waitTime);
        }

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

    private void TurnOnAllObjects()
    {
        foreach (GameObject obj in objectList)
        {
            if (obj != null) obj.SetActive(true);
        }
    }

    private void FinishSequence()
    {
        TurnOffAllObjects();
        isPlaying = false; 
    }
}