using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ChatManager : NetworkBehaviour
{
    private static ChatManager _inst = null;
    private Dictionary<int, List<string>> _msgList = new Dictionary<int, List<string>>();

    private int _playerNetId;
    private Action<string> _recvMsgCallback;


    private void Awake()
    {
        if(null == _inst)
        {
            _inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public static ChatManager Inst
    {
        get { return _inst; }
    }


    private void AddMsgList(int id, string msg)
    {
        if (_msgList.ContainsKey(id))
        {
            _msgList[id].Add(msg);
        }
        else
        {
            var msgList = new List<string>();
            _msgList.Add(id, msgList);
        }
    }

    public void BindRecvMsgCallback(Action<string> onRecvMsg)
    {
        _recvMsgCallback = onRecvMsg;
    }

    public void SendMsg(string msg)
    {
        SendMsgCommand(msg);
    }

    [Command(requiresAuthority = false)]
    public void SendMsgCommand(string msg)
    {
        var id = _playerNetId;

        AddMsgList(id, msg);

        RecvMsg(id, msg);
    }

    [ClientRpc]
    public void RecvMsg(int id, string msg)
    {
        AddMsgList(id, msg);

        if(_recvMsgCallback != null)
        {
            _recvMsgCallback.Invoke(msg);
        }
    }
}
