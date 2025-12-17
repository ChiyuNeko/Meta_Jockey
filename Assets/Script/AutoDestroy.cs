using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Header("設定銷毀時間（秒）")]
    [Tooltip("物體會在生成後幾秒自動刪除")]
    public float delay = 3.0f;

    void Start()
    {
        // 使用 Unity 內建的 Destroy 方法，並帶入延遲參數
        Destroy(gameObject, delay);
    }
}