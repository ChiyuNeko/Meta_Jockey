using System.Collections.Generic;
using LiteDB;
using UnityEngine;

public class LiteDbMessageRepository
{
    private readonly string dbPath;

    public LiteDbMessageRepository()
    {
        dbPath = DatabasePath.ChatDb;
    }

    /// <summary>
    /// 新增訊息
    /// </summary>
    public void Insert(Message message)
    {
        using var db = new LiteDatabase(dbPath);
        var col = db.GetCollection<MessageRecordLiteDB>("messages");

        var record = new MessageRecordLiteDB
        {
            Message = message
        };

        col.Insert(record);
    }

    /// <summary>
    /// 取得所有訊息
    /// </summary>
    public List<Message> GetAll()
    {
        using var db = new LiteDatabase(dbPath);
        var col = db.GetCollection<MessageRecordLiteDB>("messages");

        var allRecords = col.FindAll();
        List<Message> messages = new List<Message>();

        foreach (var record in allRecords)
        {
            messages.Add(record.Message);
        }

        return messages;
    }
}
