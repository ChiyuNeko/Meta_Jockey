using UnityEngine;
using System.Collections;

public class StartButtonEffect : MonoBehaviour
{
    [System.Serializable]
    public class RotatingObject
    {
        public GameObject obj;
        public float rotationSpeed = 100f;
        
        [Header("移動與縮放目標")]
        public Vector3 targetOffset = Vector3.zero; // 相對於起始位置的 XYZ 偏移量
        public Vector3 targetScale = Vector3.one;   // 最終縮放大小
        
        public MeshRenderer renderer; 
        
        [HideInInspector] public bool isStopping = false;
        [HideInInspector] public int lerpPropertyID;
        [HideInInspector] public Vector3 startPosition; // 遊戲開始時的原始位置
    }

    [Header("物件設定")]
    public RotatingObject[] targets = new RotatingObject[3];

    [Header("數值控制 (1-100)")]
    [Range(1, 100)]
    public float progressValue = 1f;

    [Header("共同動畫設定")]
    public float resetDuration = 0.8f;      
    public float extraSpinThreshold = 90f;  
    public AnimationCurve resetCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("延遲與淡出")]
    public float fadeDelay = 1.0f;          
    public float alphaFadeSpeed = 2f;       
    
    [Header("Shader 屬性名稱")]
    public string lerpPropertyName = "_Lerp"; 
    public string alphaPropertyName = "_Alpha"; 

    private bool[] hasReset = new bool[3];
    private bool fadingStarted = false;

    void Awake()
    {
        int lerpID = Shader.PropertyToID(lerpPropertyName);
        foreach (var t in targets)
        {
            t.lerpPropertyID = lerpID;
            if (t.obj != null)
            {
                // 紀錄場景中最初擺放的位置，作為偏移的基準
                t.startPosition = t.obj.transform.localPosition;
            }
        }
    }

    void Update()
    {
        if (fadingStarted) return;
        HandleRotation();
        CheckThresholds();
    }

    void HandleRotation()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].obj != null && !targets[i].isStopping)
            {
                targets[i].obj.transform.Rotate(0, 0, -targets[i].rotationSpeed * Time.deltaTime);
            }
        }
    }

    void CheckThresholds()
    {
        if (progressValue >= 33 && !hasReset[0]) StartCoroutine(ResetObject(0));
        if (progressValue >= 66 && !hasReset[1]) StartCoroutine(ResetObject(1));
        if (progressValue >= 100 && !hasReset[2]) StartCoroutine(ResetAndThenFade());
    }

    IEnumerator ResetObject(int index)
    {
        hasReset[index] = true;
        targets[index].isStopping = true;
        yield return StartCoroutine(SmartResetProcess(targets[index], resetDuration));
    }

    IEnumerator ResetAndThenFade()
    {
        hasReset[2] = true;
        targets[2].isStopping = true;
        yield return StartCoroutine(SmartResetProcess(targets[2], resetDuration));
        yield return new WaitForSeconds(fadeDelay);
        StartCoroutine(FadeAllAlpha());
    }

    IEnumerator SmartResetProcess(RotatingObject targetData, float duration)
    {
        GameObject targetObj = targetData.obj;
        if (targetObj == null) yield break;

        // 1. 旋轉初始值計算
        float startAngle = targetObj.transform.localEulerAngles.z;
        float totalRotation = startAngle; 
        if (totalRotation < extraSpinThreshold) totalRotation += 360f;

        // 2. 位置起始值與目標值 (基於 Awake 時紀錄的原始位置)
        Vector3 currentPos = targetObj.transform.localPosition;
        Vector3 finalPos = targetData.startPosition + targetData.targetOffset;

        // 3. 縮放起始值
        Vector3 currentScale = targetObj.transform.localScale;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = resetCurve.Evaluate(t);
            
            // --- 同步執行所有變換 ---

            // 旋轉 (自定義順時針回正)
            float currentAngle = startAngle - (totalRotation * curveValue);
            targetObj.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            // 位移 (XYZ 軸)
            targetObj.transform.localPosition = Vector3.Lerp(currentPos, finalPos, curveValue);

            // 縮放 (Scale)
            targetObj.transform.localScale = Vector3.Lerp(currentScale, targetData.targetScale, curveValue);

            // Shader Lerp (0 -> 1)
            if (targetData.renderer != null)
            {
                targetData.renderer.material.SetFloat(targetData.lerpPropertyID, curveValue);
            }

            yield return null;
        }

        // --- 強制校準最終精確值 ---
        targetObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        targetObj.transform.localPosition = finalPos;
        targetObj.transform.localScale = targetData.targetScale;
        if (targetData.renderer != null)
            targetData.renderer.material.SetFloat(targetData.lerpPropertyID, 1f);
    }

    IEnumerator FadeAllAlpha()
    {
        fadingStarted = true;
        float currentAlpha = 1.0f;
        while (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime * alphaFadeSpeed;
            foreach (var target in targets)
            {
                if (target.renderer != null)
                    target.renderer.material.SetFloat(alphaPropertyName, Mathf.Max(0, currentAlpha));
            }
            yield return null;
        }
    }
}