using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractUI : MonoBehaviour
{
    [Header("MetatNetworkManager")]
    public MetaNetworkManager MetaNetworkManager;

    [SerializeField] private string Str_GameStartSceneName;

    public void OnClick_StartGame()
    {
        SceneManager.LoadScene(Str_GameStartSceneName);
    }

    public void OnClick_CloseGame()
    {
        Application.Quit();
    }

    public void OnClick_CreateInteractObj()
    {
        MetaNetworkManager.RequestChangeAnimState("InteractLoop", true);
    }

    public void OnClick_InteractMotion()
    {

    }
}
