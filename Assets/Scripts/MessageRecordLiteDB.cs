using LiteDB;

public class MessageRecordLiteDB
{
    [BsonId] public int Id { get; set; }
    public Message Message { get; set; }

    //public MessageRecordLiteDB() { } // LiteDB 需要
}
