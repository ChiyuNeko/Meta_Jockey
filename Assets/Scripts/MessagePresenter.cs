using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// MessagePresenter 負責單一聊天室訊息的 UI 顯示與互動。
/// 
/// 功能包含：
/// - 接收 Message 資料並更新對應的 UI（暱稱、內容、時間、頭像）
/// - 向 PortraitProvider 取得對應 SenderID 的頭像
/// - 控制訊息的顯示狀態（例如是否顯示頭像 / 尾巴）
/// - 提供刪除按鈕並透過事件通知外部（MessageContainer）
/// 
/// 本類別只關心「這一則訊息怎麼顯示」，
/// 不負責：
/// - 訊息排序
/// - 訊息生成
/// - 其他訊息的狀態
/// 
/// 可視為聊天室中「單一訊息的 UI 代言人」。
/// </summary>

public class MessagePresenter : MonoBehaviour
{
  public TMP_Text Nickname;
  public TMP_Text Content;
  public TMP_Text SendTime;
  public Image Sticker;
  public Button DeleteButton;
  //public GameObject Tail;
  public GameObject PortraitObject;
  public Image PortraitImage;

  private const string TimeFormat = "HH:mm:ss";
  private readonly CultureInfo _cultureInfoProvider = new CultureInfo("ru-RU");
  public event Action<Message> OnMessageDelete;

  private Message _message;

  public Message Message
  {
    get => _message;
    set
    {
      _message = value;
      UpdatePresenter();
    }
  }
  
  private void Awake()
  {
    if(DeleteButton)
      DeleteButton.onClick.AddListener(OnDeleteButtonClick);
  }

  private void Reset() => 
    DeleteButton = GetComponentInChildren<Button>();

  private void UpdatePresenter()
  {
      Nickname.SetText(Message.SenderName);
      Content.SetText(Message.Content);
      SendTime.SetText(Message.SendTime);
      PortraitImage.sprite = PortraitProvider.ForMember(Message.SenderID);
  }

  private void OnDeleteButtonClick()
  {
    OnMessageDelete?.Invoke(_message);
  }

  public void Redraw(bool asLast)
  {
    //Tail.SetActive(asLast);
    PortraitObject.SetActive(asLast);
  }
}