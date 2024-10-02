using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] private InputField Input_ChatMsg;
    [SerializeField] private Text Text_ChatList;
    [SerializeField] private GameObject GObj_ChatExtended;
    [SerializeField] private GameObject GObj_ChatMinimized;

    private void OnEnable()
    {
        ToggleExtendChatArea(false);
        StartCoroutine(CoDelayRecvMsg());
    }

    private IEnumerator CoDelayRecvMsg()
    {
        yield return new WaitForSeconds(3.0f);
        ChatManager.Inst.BindRecvMsgCallback(OnChatMsgReceived);
    }

    private void OnChatMsgReceived(string msg)
    {
        Text_ChatList.text = $"{Text_ChatList.text}\n{msg}";
    }

    private void SendChatMsg()
    {
        ChatManager.Inst.SendMsg(Input_ChatMsg.text);
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
