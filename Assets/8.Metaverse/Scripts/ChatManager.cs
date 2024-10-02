using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MetaNetworkManager : NetworkBehaviour
{
    private Dictionary<uint, List<string>> _msgList = new Dictionary<uint, List<string>>();

    private uint _localPlayerNetId;
    private Action<string> _recvMsgCallback;
    private Action<uint, string, bool> _rpcAnimStateChange;

    private void OnDestroy()
    {
        if(_rpcAnimStateChange != null)
        {
            _rpcAnimStateChange = null;
        }
    }

    #region Interact
    public void BindLocalPlayerNetId(uint netId)
    {
        _localPlayerNetId = netId;
    }

    public void BindRpcAnimStateChangedCallback(Action<uint, string, bool> onRpcAnimStateChange)
    {
        _rpcAnimStateChange += onRpcAnimStateChange;
    }

    public void RequestChangeAnimState(string animStateKey, bool isActive)
    {
        ReqChangeAnimStateBool(_localPlayerNetId, animStateKey, isActive);
    }

    [Command(requiresAuthority=false)]
    void ReqChangeAnimStateBool(uint netId, string animStateKey, bool isActive)
    {
        RpcOnAnimStateChange(netId, animStateKey, isActive);
    }

    [ClientRpc]
    void RpcOnAnimStateChange(uint netId, string animStateKey, bool isActive)
    {
        if(_rpcAnimStateChange != null)
        {
            _rpcAnimStateChange.Invoke(netId, animStateKey, isActive);
        }
    }

    #endregion

    #region Chat
    private void AddMsgList(uint id, string msg)
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
        var id = _localPlayerNetId;

        AddMsgList(id, msg);
        RecvMsg(id, msg);
    }

    [ClientRpc]
    public void RecvMsg(uint id, string msg)
    {
        AddMsgList(id, msg);

        if(_recvMsgCallback != null)
        {
            _recvMsgCallback.Invoke(msg);
        }
    }
#endregion
}
