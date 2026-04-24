using System;

/// <summary>
/// Message 是聊天室系統中使用的純資料模型。
/// 
/// 功能：
/// - 描述一則訊息的基本資訊（SenderID、顯示名稱、內容、時間）
/// - 作為 UI 顯示與邏輯傳遞的共同資料結構
/// 
/// 特性：
/// - 不依賴 Unity 或 UI 元件
/// - 可由 Chat 建立，並傳遞至 MessageContainer / MessagePresenter
/// - 與 MessageRecord（存檔模型）分離，避免執行期與序列化耦合
/// 
/// 本類別代表「一則正在使用中的聊天訊息」。
/// </summary>

[Serializable]
public class Message
{
    public int SenderID { get; set; }
    public string SenderName { get; set; }
    public string Content { get; set; }
    public string SendTime { get; set; }

  public Message() { }
  
  public Message(int senderID, string senderName, string content)
  {
      SenderID = senderID;
      SenderName = senderName;
      Content = content;
      SendTime = DateTime.Now.ToString("HH:mm");
      //SendTime="2";
  }
  
}