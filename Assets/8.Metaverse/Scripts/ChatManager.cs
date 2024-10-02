using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class MetaNetworkManager : NetworkBehaviour
{
    private Dictionary<uint, List<string>> _msgList = new Dictionary<uint, List<string>>();

    private uint _localPlayerNetId;
    private Action<uint, string> _recvMsgCallback;
    private Action<uint, string, bool> _rpcAnimStateChange;

    [Header("InteractionFieldObject")]
    [SerializeField] GameObject Prefab_SpawnInteractFieldObj;

    //임시
    private NetPlayer _localPlayer = null;

    private void OnDestroy()
    {
        if(_rpcAnimStateChange != null)
        {
            _rpcAnimStateChange = null;
        }
    }

    #region FieldObject
    public void BindLocalPlayer(NetPlayer player)
    {
        _localPlayer = player;
    }

    public void RequestSpawnFieldObject()
    {
        if(_localPlayer == null)
        {
            return;
        }

        var transform = _localPlayer.GetSpawnObjTransform();
        if(transform == null)
        {
            return;
        }

        CommandSpawnFieldObject(transform.position, transform.rotation);
    }

    [Command(requiresAuthority = false)]
    public void CommandSpawnFieldObject(Vector3 pos, Quaternion rotate)
    {
        GameObject spawnedObj = Instantiate(Prefab_SpawnInteractFieldObj, pos, rotate);
        NetworkServer.Spawn(spawnedObj);
    }
    #endregion

    #region Interact
    public void BindLocalPlayerNetId(uint netId)
    {
        _localPlayerNetId = netId;
    }

    public void BindRpcAnimStateChangedCallback(Action<uint, string, bool> onRpcAnimStateChange)
    {
        _rpcAnimStateChange += onRpcAnimStateChange;
    }

    public void RequestChangeAnimStateByRemoteId(uint remoteNetId, string animStateKey, bool isActive)
    {
        ReqChangeAnimStateBool(remoteNetId, animStateKey, isActive);
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

    public void BindRecvMsgCallback(Action<uint, string> onRecvMsg)
    {
        _recvMsgCallback += onRecvMsg;
    }

    public void SendMsg(string msg)
    {
        SendMsgCommand(_localPlayerNetId, msg);
    }

    [Command(requiresAuthority = false)]
    public void SendMsgCommand(uint id, string msg)
    {
        AddMsgList(id, msg);
        RecvMsg(id, msg);
    }

    [ClientRpc]
    public void RecvMsg(uint id, string msg)
    {
        AddMsgList(id, msg);

        if(_recvMsgCallback != null)
        {
            _recvMsgCallback.Invoke(id, msg);
        }
    }
#endregion
}
