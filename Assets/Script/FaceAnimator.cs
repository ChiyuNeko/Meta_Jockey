using System.Collections.Generic;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct ExpressionAnim
    {
        public string expressionName;  // 動畫名稱 (例如: "Idle", "Blink")
        public Texture2D[] frames;     // 該動畫的影格圖片
        public float fps;              // 播放速度
    }

    [Header("Animation Settings")]
    public ExpressionAnim[] expressions; 
    public string defaultExpression = "Idle";

    private Renderer targetRenderer;
    private MaterialPropertyBlock propBlock;
    
    // 這裡改為抓取你指定的 "_Texture2D"
    private readonly int targetTextureID = Shader.PropertyToID("_Texture2D");

    private Dictionary<string, ExpressionAnim> animDictionary;
    private ExpressionAnim currentAnim;
    
    private float timer;
    private int currentFrameIndex;
    private bool isPlaying = false;

    void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        animDictionary = new Dictionary<string, ExpressionAnim>();

        // 將陣列載入字典，方便後續用名稱呼叫
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

        timer += Time.deltaTime;
        float frameInterval = 1f / currentAnim.fps;

        // 當時間超過單幀停留時間，切換到下一張圖
        if (timer >= frameInterval)
        {
            timer -= frameInterval;
            currentFrameIndex = (currentFrameIndex + 1) % currentAnim.frames.Length;
            UpdateTexture(currentAnim.frames[currentFrameIndex]);
        }
    }

    // 呼叫這個方法來切換不同的表情動畫
    public void PlayExpression(string name)
    {
        if (animDictionary.TryGetValue(name, out ExpressionAnim newAnim))
        {
            if (currentAnim.expressionName == name) return; 

            currentAnim = newAnim;
            currentFrameIndex = 0;
            timer = 0f;
            isPlaying = true;
            
            UpdateTexture(currentAnim.frames[currentFrameIndex]);
        }
        else
        {
            Debug.LogWarning($"找不到名為 {name} 的動畫設定！");
        }
    }

    // 透過 MaterialPropertyBlock 更新貼圖，避免產生多餘的材質實體
    private void UpdateTexture(Texture2D tex)
    {
        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetTexture(targetTextureID, tex);
        targetRenderer.SetPropertyBlock(propBlock);
    }
}