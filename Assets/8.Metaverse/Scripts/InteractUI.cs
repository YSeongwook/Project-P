using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractUI : MonoBehaviour
{
    [Header("MetatNetworkManager")]
    public MetaNetworkManager MetaNetworkManager;

    public void OnClick_StartGame()
    {
        MetaNetworkManager.ReqStopClient();
    }

    public void OnClick_CloseGame()
    {
        Application.Quit();
    }

    public void OnClick_CreateInteractObj()
    {
        MetaNetworkManager.RequestSpawnFieldObject();
    }

    public void OnClick_InteractMotion()
    {
        MetaNetworkManager.RequestChangeAnimState("InteractLoop", true);
    }
}
