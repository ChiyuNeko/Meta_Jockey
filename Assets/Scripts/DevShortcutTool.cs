using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Events;
using System.IO;
using System;
using LiteDB;
using System.Linq;
using System.Text;


/// <summary>
/// DevShortcutTool 是一個僅供開發階段使用的快捷鍵工具管理器。
/// 
/// 功能：
/// - 使用 Unity New Input System 綁定多組開發者快捷鍵
/// - 在遊戲執行期間快速觸發除錯或測試用指令
/// - 提供清理資料、重置狀態、自訂開發指令等功能入口
/// 
/// 目前內建功能包含：
/// - 清空聊天室 JSON 存檔（不刪檔，只重置內容）
/// - 預留角色位置重置功能
/// - 預留自訂開發者指令
/// 
/// 設計目的：
/// - 加速開發與測試流程
/// - 避免將除錯邏輯散落在正式遊戲程式中
/// - 所有快捷鍵皆可在 Inspector 中自由綁定
/// 
/// 注意事項：
/// - 建議僅在開發或 Debug 模式下啟用
/// - 不應影響正式遊戲流程或玩家體驗
/// </summary>

public class DevShortcutTool : MonoBehaviour
{


  
    [Header("快捷鍵設定")]
    [Tooltip("在 Inspector 中點擊 '+' 並選擇 'Add Binding' 來設定按鍵")]
    public InputAction Delete_Json;
    public InputAction Load_Json;
    public InputAction AddMessageToDB;
    public InputAction resetPlayerPosition;
    public InputAction customAction;

    [Header("觸發事件")]
    public UnityEvent Delete_Json_Event;
    public UnityEvent Load_Json_Event;
    public UnityEvent AddMessageToDB_Event;
    [Header("其他腳本抓取區")]
    public Chat chat;


    void Start(){

        Debug.Log($"<color=green>[LiteDB]</color> 資料庫已連線: {DatabasePath.ChatDb}");
    }

    private void OnEnable()
    {
        // 啟用所有輸入動作
        Delete_Json.Enable();
        Load_Json.Enable();
        AddMessageToDB.Enable();
        resetPlayerPosition.Enable();
        customAction.Enable();

        // 訂閱事件 (當按鍵被按下時觸發)
        Delete_Json.performed += _ => Delete_json();
        Load_Json.performed += _ => load_json();
        AddMessageToDB.performed +=_ =>addMessageToDB();
        resetPlayerPosition.performed += _ => ResetPlayer();
        customAction.performed += _ => ExecuteCustomAction();
    }

    private void OnDisable()
    {
        // 停用以避免記憶體洩漏
        Delete_Json.Disable();
        Load_Json.Disable();
        AddMessageToDB.Disable();
        resetPlayerPosition.Disable();
        customAction.Disable();
    }

    // --- 具體功能方法 ---

    public void Delete_json()
    {
        Debug.Log("實作：開啟/關閉開發者選單");
        //OnToggleDebugMenu?.Invoke();
        string filePath = Path.Combine(Application.dataPath, "Resources/ABCDE.json");
        if (File.Exists(filePath))
        {
            try
            {
                // 方案 A: 直接寫入一個空的 JSON 大括號
                // 如果你的資料結構是 Array，則改用 "[]"
               string emptyJson = "{ \"records\": [] }";
                
                File.WriteAllText(filePath, emptyJson);
                
                Debug.Log($"<color=cyan>[DevTool]</color> 檔案的內容已成功清空為 {emptyJson}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"無法清理檔案內容: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"檔案不存在，無法清除內容: {filePath}");
        }
    }
    

    public void load_json(){

        Load_Json_Event?.Invoke();
        string path = Path.Combine(Application.dataPath, "Resources/ABCDE.json");

        if (!File.Exists(path))
        {
            Debug.Log("[Chat] 找不到聊天紀錄檔，略過載入");
            return;
        }

        string json = File.ReadAllText(path);
        ChatHistory history = JsonUtility.FromJson<ChatHistory>(json);

        if (history == null || history.records == null || history.records.Count == 0)
        {
            Debug.Log("[Chat] 聊天紀錄存在，但沒有任何訊息");
            return;
        }

        Debug.Log($"[Chat] 載入聊天紀錄，共 {history.records.Count} 則");

        foreach (MessageRecord record in history.records)
        {
            // 將 MessageRecord 轉回 Message（給 UI 用）
            Message message = new Message(
                record.senderId,
                record.sender,
                record.content
            );

            // 使用存檔時間（覆蓋建構子產生的時間）
            message.SendTime = DateTime
                .Parse(record.sendTime)
                .ToString("HH:mm");

            // 直接送進 UI（不再存檔）
            
            chat.ReceiveMessage(message);
        }
    }

    public void addMessageToDB()
    {
        for(int i=0;i<50;i++){
            chat.SendChatMessage("呼嚕呼嚕");
        }

        for(int j=0;j<50;j++){
            chat.SendChatMessage("嘰嘰喳喳姑姑哇哇");
        }
        
    }

    public void ResetPlayer()
    {
        Debug.Log("實作：重置角色位置");
        // GameObject.FindWithTag("Player").transform.position = Vector3.zero;
    }

    public void ExecuteCustomAction()
    {
        Debug.Log("實作：自定義開發者指令");
    }
}