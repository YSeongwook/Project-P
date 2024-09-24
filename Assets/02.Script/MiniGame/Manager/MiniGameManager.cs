using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : Singleton<MiniGameManager>
{
    [FoldoutGroup("Mini Game Canvas List")]
    [SerializeField]
    private List<Canvas> miniGameList;

    protected new void Awake()
    {
        base.Awake();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        foreach(var game in miniGameList) 
        {
            game.gameObject.SetActive(false);
        }
    }

    private void AddEvent()
    {
        EventManager<MiniGame>.StartListening<int, int>(MiniGame.ActiveMiniGame, ActiveMiniGameCanvas);
        EventManager<MiniGame>.StartListening(MiniGame.DisActiveMiniGame, DisActiveMiniGameCanvas);
    }

    private void RemoveEvent()
    {
        EventManager<MiniGame>.StopListening<int, int>(MiniGame.ActiveMiniGame, ActiveMiniGameCanvas);
        EventManager<MiniGame>.StopListening(MiniGame.DisActiveMiniGame, DisActiveMiniGameCanvas);
    }

    private void ActiveMiniGameCanvas(int chapter, int stage)
    {
        //var index = (chapter - 2) * 3 + (stage / 10);
        // 디버깅 모드
        var index = (stage - 1);

        miniGameList[index].gameObject.SetActive(true);
    }

    private void DisActiveMiniGameCanvas()
    {
        foreach(var game in miniGameList)
        {
            game.gameObject.SetActive(false);
        }

        //EventManager<StageEvent>.TriggerEvent(StageEvent.SetMiniGame, true);
    }
}
