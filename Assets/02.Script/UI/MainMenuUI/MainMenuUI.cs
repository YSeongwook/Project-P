using EnumTypes;
using EventLibrary;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Awake()
    {
        EventManager<StageEvent>.StartListening(StageEvent.ReturnSelectStage, ToggleMainMenuUI);
    }

    private void OnDestroy()
    {
        EventManager<StageEvent>.StopListening(StageEvent.ReturnSelectStage, ToggleMainMenuUI);
    }

    private void ToggleMainMenuUI()
    {
        bool isActive = gameObject.activeSelf;
        
        gameObject.SetActive(!isActive);
    }
}
