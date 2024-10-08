using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaNetManager : NetworkManager
{
    public Action OnMetaStartServerCallback;
    public Action OnMetaStartClientCallback;

    public override void OnDestroy()
    {
        OnMetaStartServerCallback = null;
        OnMetaStartClientCallback = null;
        base.OnDestroy();
    }

    public bool GetNetworkClientConnected()
    {
        return NetworkClient.isConnected;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if(OnMetaStartServerCallback != null)
        {
            OnMetaStartServerCallback.Invoke();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(OnMetaStartClientCallback != null)
        {
            OnMetaStartClientCallback.Invoke();
        }
    }

}
