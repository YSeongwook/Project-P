using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : Singleton<MiniGameManager>
{
    [FoldoutGroup("Mini Game Canvas List")]
    [SerializeField] private List<Canvas> miniGameList;

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
        // 미니게임 리스트에 있는 오브젝트들을 비활성화
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
        int index = 0;
        if (chapter > 1 && stage % 10 == 0)
        {
            index = chapter - 2 + (stage % 10);
        }

        if(index < miniGameList.Count - 1) miniGameList[index].gameObject.SetActive(true);
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
