using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// MessageContainer 負責管理聊天室中所有訊息 UI 的生命週期。
/// 
/// 功能包含：
/// - 根據 Message.SenderID 決定使用哪一種訊息 Prefab（自己 / 其他成員）
/// - 生成並管理 MessagePresenter 實例
/// - 維護訊息順序列表，確保聊天室顯示順序正確
/// - 處理連續訊息的 UI 連貫性（例如：只顯示最後一則的頭像）
/// - 接收 MessagePresenter 的刪除事件，並在刪除後重新修正前後訊息的顯示狀態
/// 
/// 本類別不負責：
/// - 訊息內容的建立
/// - 頭像資源的載入
/// - 單一訊息的 UI 細節顯示
/// 
/// 可視為聊天室 UI 的「總管 / 排程者」。
/// </summary>



public class MessageContainer : MonoBehaviour
{
  public Chat Chat;
  public RectTransform ContainerObject;
  public GameObject MessagePrefab;
  public GameObject ChatOwnerMessagePrefab;

  private readonly List<MessagePresenter> _presenters = new List<MessagePresenter>();

  private void OnDestroy()
  {
    foreach (MessagePresenter presenter in _presenters)
      presenter.OnMessageDelete -= DeleteMessage;
  }

  private void Reset() =>
    Chat = FindObjectOfType<Chat>();

  public void AddMessage(Message message)
  {
    MessagePresenter presenter = InstantiatePresenter(message);
    presenter.OnMessageDelete += DeleteMessage;
  }

  private MessagePresenter InstantiatePresenter(Message message)
  {
      // 核心邏輯：不再比對 message.Sender == Chat.Owner
      // 直接根據 SenderType 是否為 0 來決定 Prefab
      bool isOwner = (message.SenderID == 0); 

      GameObject prefab = isOwner ? ChatOwnerMessagePrefab : MessagePrefab;
      
      // 生成對應的 UI 物件
      MessagePresenter presenter = Instantiate(prefab, ContainerObject)
                                  .GetComponent<MessagePresenter>();

      // 設定訊息內容（Presenter 會去跑 UpdatePresenter 顯示文字和頭像）
      presenter.Message = message;
      
      // 處理連續訊息的 UI 連貫性 (Redraw 邏輯)
      MessagePresenter lastMessage = _presenters.LastOrDefault();
      if (lastMessage && lastMessage.Message.SenderID == message.SenderID)
      {
          lastMessage.Redraw(asLast: false);
      }

      _presenters.Add(presenter);
      return presenter;
  }

  private void DeleteMessage(Message message)
  {
    MessagePresenter presenter = _presenters.FirstOrDefault(o => o.Message == message);
    if (!presenter)
      return;
    
    RedrawPreviousIfNeeded(presenter);
    DestroyMessagePresenter(presenter);
  }

  private void DestroyMessagePresenter(MessagePresenter presenter)
  {
    presenter.OnMessageDelete -= DeleteMessage;
    _presenters.Remove(presenter);
    Destroy(presenter.gameObject);
  }

  private void RedrawPreviousIfNeeded(MessagePresenter presenter)
  {
    var index = _presenters.IndexOf(presenter);

    MessagePresenter previous = ValidIndex(index - 1) ? _presenters[index - 1] : null;

    MessagePresenter next = ValidIndex(index + 1) ? _presenters[index + 1] : null;

    if (ShouldRedrawPrevious())
      previous.Redraw(asLast: true);

    bool ShouldRedrawPrevious() =>
      previous && (!next || next && next.Message.SenderID != presenter.Message.SenderID);
  }

  private bool ValidIndex(int index) => 
    index >= 0 && index < _presenters.Count;
}