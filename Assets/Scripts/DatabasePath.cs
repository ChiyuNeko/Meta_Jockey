using UnityEngine;
using System.IO;

public static class DatabasePath
{
    public static string ChatDb =>
        Path.Combine(Application.persistentDataPath, "Test/chat.db");
}
