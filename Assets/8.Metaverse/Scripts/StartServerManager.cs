using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartServerManager : MonoBehaviour
{
    [SerializeField] MetaNetManager NetManager;

    [SerializeField] GameObject Obj_LoadingPopup;

    [SerializeField] bool IsStartAsServer;

    public void Start()
    {
        if(NetManager == null)
        {
            return;
        }

        if (IsStartAsServer)
        {
            TryStartServer();
        }
        else
        {
            TryConnectToServer();
        }
    }

    private void TryStartServer()
    {
        NetManager.StartServer();
    }

    private void LateUpdate()
    {
        TryConnectToServer();
    }

    private void TryConnectToServer()
    {
        if (NetManager.GetNetworkClientConnected())
        {
            StartCoroutine(CoDelayCloseLoadingPopup());
            return;
        }

        if(NetManager.OnMetaStartClientCallback != null)
        {
            NetManager.OnMetaStartClientCallback -= OnMetaStartClient;
        }

        NetManager.OnMetaStartClientCallback += OnMetaStartClient;
        NetManager.StartClient();
    }

    IEnumerator CoDelayCloseLoadingPopup()
    {
        yield return new WaitForSeconds(3.0f);

        if (Obj_LoadingPopup.activeSelf)
        {
            Obj_LoadingPopup.gameObject.SetActive(false);
        }

    }

    private void OnMetaStartClient()
    {
        Obj_LoadingPopup.gameObject.SetActive(!NetManager.GetNetworkClientConnected());
    }

}
