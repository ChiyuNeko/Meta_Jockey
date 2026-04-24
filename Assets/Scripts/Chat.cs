using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using LiteDB;
using System.Linq;
//using LiteDB;

//接收Message的訊息並呼叫MessagerContainer管理訊息

public class Chat : MonoBehaviour
{
  public string Owner;

  [Header("發言控制")]
  [Tooltip("0 = Owner, 1 以上為 Members 列表索引")]
  public int _ID = 0;

  public List<string> Members = new List<string>();
  public MessageContainer Container;

  // LiteDB 對應的 Repository
    private LiteDbMessageRepository liteDbRepository;

    public string CurrentSenderName => GetNameByIndex(_ID);

    public string GetNameByIndex(int index)
    {
        if (index == 0) return Owner;
        int listIndex = index - 1;
        return (listIndex >= 0 && listIndex < Members.Count) ? Members[listIndex] : "Unknown";
    }
    

    //在DebugLog 印出訊息
    public void DebugPrintAllLiteDBMessages()
    {
        using var db = new LiteDatabase(DatabasePath.ChatDb);
        var col = db.GetCollection<MessageRecordLiteDB>("messages");

        var allRecords = col.FindAll().ToList();
        Debug.Log($"[LiteDB] 資料庫共有 {allRecords.Count} 筆訊息");

        foreach (var record in allRecords)
        {
            Message msg = record.Message;
            Debug.Log($"ID: {record.Id}, SenderID: {msg.SenderID}, SenderName: {msg.SenderName}, Content: {msg.Content}, SendTime: {msg.SendTime}");
        }
    }
    //建立資料庫介面
    private void Awake()
    {
        liteDbRepository = new LiteDbMessageRepository();
    }

    void Start()
    {
     
        LoadLiteDB();
        LoadChatHistoryLiteDB();
        DebugPrintAllLiteDBMessages();
    }

    //載入資料庫
    private void LoadLiteDB(){
        Debug.Log("DB Path: " + DatabasePath.ChatDb);
        using var db = new LiteDatabase(DatabasePath.ChatDb);
        Debug.Log(File.Exists(DatabasePath.ChatDb)? "DB Created": "DB Not Found");
        var col = db.GetCollection<MessageRecordLiteDB>("messages");
        int count = col.Count(); // LiteDB 內建 Count() 方法
        Debug.Log($"[LiteDB] 資料庫中共有 {count} 則訊息");
    }
    //將訊息存入資料庫
    public void SaveMessageToJsonLiteDB(Message message)
    {

        
        liteDbRepository.Insert(message);
        Debug.Log($"[LiteDB] 儲存訊息：{message.SenderName} - {message.Content}");
    }
    //將資料庫的訊息全部轉乘UI
    public void LoadChatHistoryLiteDB()
    {
        List<Message> messages = liteDbRepository.GetAll();
        Debug.Log($"[LiteDB] 載入 {messages.Count} 則訊息");

        foreach (var msg in messages)
        {
           ReceiveMessage(msg);
        }
    }


    public void SendChatMessage(string content)
    {
        // 取得當前名字
        string senderName = CurrentSenderName;
        
        // 建立訊息：傳入名字、內容、以及當前的索引 (SenderType)
        Message message = new Message(_ID, senderName, content);
        SaveMessageToJsonLiteDB(message);
        ReceiveMessage(message);
        Debug.Log($"{_ID}, {senderName}, {content}");
    }

    public void ReceiveMessage(Message message) => 
        Container.AddMessage(message);

    private void Reset() => 
        Container =  FindObjectOfType<MessageContainer>();



    #region SrollBar功能

    public float threshold = 1f; // 拉過頭多少觸發（1.1 代表拉過頂端 10%）
    private bool isRefreshing = false;

    public void OnScrollValueChanged(Vector2 pos)
    {

        //Debug.Log(pos.y);
        // pos.y > 1 代表正在往下拉（拉過頭）
        if (pos.y > threshold && !isRefreshing)
        {
            TriggerRefresh();
        }

        //float anchoredY = scrollRect.content.anchoredPosition.y;

    // 這裡的 150 是像素單位，代表下拉 150 像素就觸發

    }


    private void TriggerRefresh()
    {
        isRefreshing = true;
        Debug.Log("開始刷新訊息...");

        // 模擬異步加載訊息（例如呼叫 API）
        Invoke("EndRefresh", 0.5f); 
    }

    private void EndRefresh()
    {
        isRefreshing = false;
        Debug.Log("刷新完成！");
        // 這裡可以寫入把新訊息放入 Content 的邏輯
    }
    #endregion

}





//拿來記錄訊息的類別 會抓取Message類別的東西並且將這些數值存進json
[Serializable]
public class MessageRecord
{
    public int id;              // JSON 專用的訊息ID
    public int senderId;
    public string sender;
    public string content;
    public string sendTime;
}


[Serializable]
public class ChatHistory
{
    public List<MessageRecord> records = new List<MessageRecord>();
}