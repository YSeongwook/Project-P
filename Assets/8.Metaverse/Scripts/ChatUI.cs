using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private MetaNetworkManager ChatManager;

    [SerializeField] private GameObject GObj_ChatExtended;
    [SerializeField] private GameObject GObj_ChatMinimized;

    [Header("Chat")]
    [SerializeField] private InputField Input_ChatMsg;
    [SerializeField] private Text Text_ChatList;
    [SerializeField] private Text Text_ChatListExtended;

    private void OnEnable()
    {
        ToggleExtendChatArea(false);
        StartCoroutine(CoDelayRecvMsg());
    }

    private IEnumerator CoDelayRecvMsg()
    {
        yield return new WaitForSeconds(1.0f);
        ChatManager.BindRecvMsgCallback(OnChatMsgReceived);
    }

    private void OnChatMsgReceived(uint id, string msg)
    {
        Text_ChatList.text = $"{Text_ChatList.text}\n{msg}";
        Text_ChatListExtended.text = $"{Text_ChatListExtended.text}\n{msg}";
    }

    private void SendChatMsg()
    {
        ChatManager.SendMsg(Input_ChatMsg.text);
    }

    public void OnClick_SendMsg()
    {
        SendChatMsg();
    }

    public void OnSubmit_ChatMsg()
    {
        SendChatMsg();
    }

    public void OnClick_ExtendChatArea()
    {
        ToggleExtendChatArea(true);
    }

    public void OnClick_MinimizeChatArea()
    {
        ToggleExtendChatArea(false);
    }

    private void ToggleExtendChatArea(bool isExtend)
    {
        GObj_ChatExtended.SetActive(isExtend);
        GObj_ChatMinimized.SetActive(!isExtend);
    }

}
