using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;

//主要功能:將輸入欄位的訊息送出並存入Json檔

public class MessageSender : MonoBehaviour
{
  public InputAction SendMessageAction;
  public TMP_InputField MessageField;
  public Chat Chat;

  private void OnEnable() => 
    SendMessageAction.Enable();

  private void OnDisable() => 
    SendMessageAction.Disable();

  private void Awake() => 
    SendMessageAction.performed += OnSendMessageAction;

  public void Send()
    {
        if (string.IsNullOrEmpty(MessageField.text)) return;
        
        // 直接叫 Chat 發送，邏輯都寫在 Chat 裡了
        Chat.SendChatMessage(MessageField.text);
        //Chat.SaveMessageToJson(MessageField.text);
        MessageField.text = string.Empty;
    }

  private void OnSendMessageAction(InputAction.CallbackContext ctx)
  {
    if(MessageField.isFocused) 
      Send();
      
  }

  private void Reset() => 
    Chat = FindObjectOfType<Chat>();



}


