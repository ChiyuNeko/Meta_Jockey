using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PortraitProvider 是一個靜態頭像資料提供者。
/// 
/// 功能：
/// - 在啟動時自動載入 Resources/Portraits 資料夾內的所有 Sprite
/// - 以檔名作為 SenderID（例如 "0.png" → SenderID = 0）
/// - 提供 SenderID 對應的頭像 Sprite
/// - 若找不到對應 ID，則回傳 Default 頭像（檔名需為 "Default"）
/// 
/// 設計目的：
/// - 將頭像載入與查詢邏輯集中管理
/// - 避免每個 MessagePresenter 各自載入資源
/// - 與 Chat / Message 使用相同的 SenderID 系統保持一致
/// 
/// 本類別不依賴任何 UI 或 MonoBehaviour，
/// 是純資料層工具類別。
/// </summary>


public static class PortraitProvider
{
    private const int DefaultID = -1; // 用 -1 代表預設頭像的 Key
    private static readonly Dictionary<int, Sprite> Portraits;

    static PortraitProvider()
    {
        // 載入 Resources/Portraits 資料夾下的所有圖片
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Portraits");
        Portraits = new Dictionary<int, Sprite>();

        foreach (var sprite in allSprites)
        {
            // 嘗試將檔名轉換為整數 ID (例如檔名是 "0" 就轉成 int 0)
            if (int.TryParse(sprite.name, out int id))
            {
                Portraits[id] = sprite;
            }
            else if (sprite.name == "Default")
            {
                Portraits[DefaultID] = sprite;
            }
        }
    }

    public static Sprite ForMember(int senderID)
    {
        // 優先找對應 ID 的圖片，找不到就找 Default，再找不到就回傳 null
        if (Portraits.TryGetValue(senderID, out Sprite portrait))
            return portrait;
        
        return Portraits.TryGetValue(DefaultID, out Sprite defaultPortrait) 
            ? defaultPortrait 
            : null;
    }
}