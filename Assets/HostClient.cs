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
    [Header("彈跳訊息")]
    public Vector3 popArea;
    public GameObject msgPop;
    public GameObject effectPop;

    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private long hostStartupTime;
    
    // 用來存放需要回到主執行緒執行的動作
    private readonly Queue<System.Action> mainThreadActions = new Queue<System.Action>();

    void Start()
    {
        hostStartupTime = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // 記錄主機啟動的時間戳
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
        if(Input.GetKeyDown(KeyCode.C))
        {
            ClearMessagePool();
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
        bool isNewMessage = receivedMsg.timestamp >= hostStartupTime;

        Debug.Log($"收到訊息：{formattedMessage} (時間戳: {receivedMsg.timestamp}, 啟動時間: {hostStartupTime}, 是否新訊息: {isNewMessage})");

        if(receivedMsg.content[0] == '%') // 如果訊息以 % 開頭，視為指令
        {
            if (isNewMessage) 
            {
                EffectInRandomArea(0); // 目前只有一種效果，未來可根據指令內容選擇不同效果
                Debug.Log($"新指令：{formattedMessage}"); // 只在主機端的 Console 顯示新指令，過濾掉啟動前的歷史訊息
            }
        }
        else
        {
            GameObject newMsgBox = Instantiate(msgBox, ContainerObject); // 在 UI 中生成新的訊息框
            TextMeshProUGUI newMsgOwner = newMsgBox.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI newMsg = newMsgBox.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            newMsgOwner.text = $"[{receivedMsg.senderEmail}]";
            newMsg.text = $"{receivedMsg.content}";
            if (isNewMessage) 
            {
                PopInRandomArea(receivedMsg.content); // 在主機端的彈跳區域生成彈跳訊息，內容為收到的訊息文本
                Debug.Log($"新訊息：{formattedMessage}"); // 只在主機端的 Console 顯示新訊息，過濾掉啟動前的歷史訊息
            }
        }
        
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

    public void PopInRandomArea(string text)
    {
        float x = Random.Range(0,popArea.x);
        float z = Random.Range(0,popArea.z); 
        Vector3 popPos = new Vector3(x, 0, z) + gameObject.transform.position;
        GameObject newPopMsg = Instantiate(msgPop, popPos, Quaternion.Euler(0, 90, 0));
        newPopMsg.GetComponent<SetText>().SetContent(text);
        
    }

    public void EffectInRandomArea(int effectIndex)
    {
        float x = Random.Range(0,popArea.x);
        float z = Random.Range(0,popArea.z); 
        Vector3 popPos = new Vector3(x, 0, z) + gameObject.transform.position;
        GameObject newPopMsg = Instantiate(effectPop, popPos, Quaternion.Euler(0, 90, 0));
    }

    void OnDestroy()
    {
        if (dbReference != null)
        {
            dbReference.Child("Messages").ChildAdded -= HandleMessageAdded;
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Set the color with custom alpha.
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        // Draw the cube.
        Gizmos.DrawCube(transform.position + popArea / 2, popArea);


    }
}