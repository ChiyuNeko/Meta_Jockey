using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Auth; // 新增 Auth 命名空間
using Firebase.Extensions;
using UnityEngine.UI;

public class HostClient : MonoBehaviour
{
    [Header("主機端管理員帳號 (請與 Firebase 後台一致)")]
    public string adminEmail = "admin@host.com"; 
    public string adminPassword = "HostPassword123!";

    [Header("UI 參考")]
    public RectTransform ContainerObject;
    public TextMeshProUGUI chatDisplay;
    public TextMeshProUGUI statusText;
    public GameObject msgBox;

    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    
    // 用來存放需要回到主執行緒執行的動作
    private readonly Queue<System.Action> mainThreadActions = new Queue<System.Action>();

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            auth = FirebaseAuth.DefaultInstance;
            
            statusText.text = "主機端：Firebase 初始化成功，正在登入管理員...";
            
            // 初始化完成後，立刻執行登入
            LoginAsAdmin();
        });
    }

    void Update()
    {
        // 確保 UI 更新在主執行緒執行
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                System.Action action = mainThreadActions.Dequeue();
                action?.Invoke();
            }
        }
    }

    void LoginAsAdmin()
    {
        auth.SignInWithEmailAndPasswordAsync(adminEmail, adminPassword).ContinueWithOnMainThread(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("管理員登入失敗: " + task.Exception);
                // 將錯誤訊息推回主執行緒顯示
                lock (mainThreadActions) { mainThreadActions.Enqueue(() => { statusText.text = "管理員登入失敗，請檢查帳號密碼。"; }); }
                return;
            }

            currentUser = task.Result.User;
            
            lock (mainThreadActions) 
            { 
                mainThreadActions.Enqueue(() => { 
                    statusText.text = $"主機端已登入 ({currentUser.Email})，正在監聽訊息..."; 
                }); 
            }
            
            // 登入成功且取得權限後，才開始監聽訊息
            StartListeningForMessages();
        });
    }

    void StartListeningForMessages()
    {
        dbReference.Child("Messages").ChildAdded += HandleMessageAdded;
    }

    void HandleMessageAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) return;

        string json = args.Snapshot.GetRawJsonValue();
        MessageData receivedMsg = JsonUtility.FromJson<MessageData>(json);

        string formattedMessage = $"[{receivedMsg.senderEmail}]: {receivedMsg.content}\n";
        GameObject newMsgBox = Instantiate(msgBox, ContainerObject);
        TextMeshProUGUI newMsgOwner = newMsgBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI newMsg = newMsgBox.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        newMsgOwner.text = $"[{receivedMsg.senderEmail}]";
        newMsg.text = $"{receivedMsg.content}";
        
        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(() => {
                if (chatDisplay != null) chatDisplay.text += formattedMessage;
            });
        }
    }

    // --- 綁定給「清除按鈕」的函式 ---
    public void ClearMessagePool()
    {
        // 必須確保已連線且已登入管理員
        if (dbReference == null || currentUser == null)
        {
            statusText.text = "權限不足或尚未連線，無法清除資料！";
            return;
        }

        statusText.text = "正在清除資料庫...";

        // 呼叫 RemoveValueAsync 刪除 Messages 節點
        dbReference.Child("Messages").RemoveValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("清除訊息失敗：" + task.Exception);
                lock (mainThreadActions) { mainThreadActions.Enqueue(() => { statusText.text = "清除失敗，權限不足或網路異常。"; }); }
            }
            else
            {
                Debug.Log("雲端訊息池已成功清除！");
                
                // 清空主機端畫面上的文字
                lock (mainThreadActions)
                {
                    mainThreadActions.Enqueue(() => {
                        statusText.text = "訊息池已成功清除。";
                        if (chatDisplay != null) chatDisplay.text = ""; 
                    });
                }
            }
        });
    }

    void OnDestroy()
    {
        if (dbReference != null)
        {
            dbReference.Child("Messages").ChildAdded -= HandleMessageAdded;
        }
    }
}