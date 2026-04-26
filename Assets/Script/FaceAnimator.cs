using System.Collections.Generic;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct ExpressionAnim
    {
        public string expressionName;  // 動畫名稱 (例如: "Idle", "Blink")
        public Texture2D[] frames;     // 該動畫的影格圖片
        public float fps;              // 基準播放速度 (當 BPM 為 120 時的速度)
        
        // ★ 新增：是否允許被自動循環選中？
        [Tooltip("勾選：參與自動循環。不勾選：自動循環跳過，僅供手動呼叫。")]
        public bool allowInAutoLoop; 
    }

    [Header("Animation Settings")]
    public ExpressionAnim[] expressions; 
    public string defaultExpression = "Idle";

    [Header("Rhythm Settings")]
    [Tooltip("目前的音樂節拍 (基準設定為 120 BPM)")]
    public float bpm = 120f; 

    [Header("Auto Loop Settings (自動切換動畫)")]
    [Tooltip("是否開啟自動按順序切換動畫？")]
    public bool isAutoLooping = false; 
    [Tooltip("經過幾拍後切換到下一個動畫？")]
    public float beatsPerExpression = 4f; 

    private Renderer targetRenderer;
    private MaterialPropertyBlock propBlock;
    
    private readonly int targetTextureID = Shader.PropertyToID("_Texture2D");

    private Dictionary<string, ExpressionAnim> animDictionary;
    private ExpressionAnim currentAnim;
    
    private float frameTimer;
    private float autoSwitchTimer; 
    private int currentFrameIndex;
    private int currentExpressionIndex = 0; 
    private bool isPlaying = false;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        animDictionary = new Dictionary<string, ExpressionAnim>();

        foreach (var anim in expressions)
        {
            if (!animDictionary.ContainsKey(anim.expressionName))
            {
                animDictionary.Add(anim.expressionName, anim);
            }
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(defaultExpression))
        {
            PlayExpression(defaultExpression);
        }
    }

    void Update()
    {
        if (!isPlaying || currentAnim.frames == null || currentAnim.frames.Length == 0) return;
        if (bpm <= 0f) return;

        // 1. 處理單一動畫的 FPS 播放邏輯
        float effectiveFps = currentAnim.fps * (bpm / 120f);
        if (effectiveFps > 0f)
        {
            frameTimer += Time.deltaTime;
            float frameInterval = 1f / effectiveFps;

            if (frameTimer >= frameInterval)
            {
                frameTimer -= frameInterval;
                currentFrameIndex = (currentFrameIndex + 1) % currentAnim.frames.Length;
                UpdateTexture(currentAnim.frames[currentFrameIndex]);
            }
        }

        // 2. 處理自動切換到下一組動畫的邏輯
        if (isAutoLooping && expressions.Length > 1 && beatsPerExpression > 0f)
        {
            autoSwitchTimer += Time.deltaTime;
            float secondsPerSwitch = (60f / bpm) * beatsPerExpression;

            if (autoSwitchTimer >= secondsPerSwitch)
            {
                autoSwitchTimer -= secondsPerSwitch; 
                GoToNextExpression();
            }
        }
    }

    // ★ 修改：智慧尋找下一個允許循環的動畫
    private void GoToNextExpression()
    {
        int nextIndex = currentExpressionIndex;
        int attempts = 0;
        int maxAttempts = expressions.Length; // 防呆：最多只找陣列長度的次數

        while (attempts < maxAttempts)
        {
            // 推進到下一個索引
            nextIndex = (nextIndex + 1) % expressions.Length;
            attempts++;

            // 檢查該動畫是否允許自動循環
            if (expressions[nextIndex].allowInAutoLoop)
            {
                PlayExpression(expressions[nextIndex].expressionName);
                return; // 找到就直接播放並結束
            }
        }

        // 防呆機制：如果迴圈跑完了都沒找到，代表所有動畫的 allowInAutoLoop 都被設為 false
        Debug.LogWarning("TextureAnimator: 自動切換失敗！因為所有的動畫都被設定為跳過循環。自動循環已暫時強制關閉。");
        isAutoLooping = false; 
    }

    // 呼叫這個方法來切換不同的表情動畫 (一般呼叫直接無視 allowInAutoLoop)
    public void PlayExpression(string name)
    {
        if (animDictionary.TryGetValue(name, out ExpressionAnim newAnim))
        {
            if (currentAnim.expressionName == name) return; 

            currentAnim = newAnim;
            currentFrameIndex = 0;
            frameTimer = 0f;
            
            autoSwitchTimer = 0f; 
            SyncExpressionIndex(name); 

            isPlaying = true;
            UpdateTexture(currentAnim.frames[currentFrameIndex]);
        }
        else
        {
            Debug.LogWarning($"找不到名為 {name} 的動畫設定！");
        }
    }

    private void SyncExpressionIndex(string name)
    {
        for (int i = 0; i < expressions.Length; i++)
        {
            if (expressions[i].expressionName == name)
            {
                currentExpressionIndex = i;
                break;
            }
        }
    }

    private void UpdateTexture(Texture2D tex)
    {
        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetTexture(targetTextureID, tex);
        targetRenderer.SetPropertyBlock(propBlock);
    }
}