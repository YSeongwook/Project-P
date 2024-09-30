using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private InputField Input_ChatMsg;
    [SerializeField] private Text Text_ChatList;

    public void OnSubmit_ChatMsg()
    {
        ChatManager.Inst.SendMsg(Input_ChatMsg.text, OnChatMsgReceived);
    }

    private void OnChatMsgReceived(string msg)
    {
        Text_ChatList.text = $"{Text_ChatList.text}\n{msg}";
    }
}
