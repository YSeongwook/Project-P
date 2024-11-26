using System;
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
    }

    private void OnEnable()
    {
        AddEvents();
    }

    private void OnDisable()
    {
        RemoveEvents();
    }

    private void Start()
    {
        // 미니게임 리스트에 있는 오브젝트들을 비활성화
        foreach(var game in miniGameList) 
        {
            game.gameObject.SetActive(false);
        }
    }

    private void AddEvents()
    {
        EventManager<MiniGame>.StartListening<int, int>(MiniGame.ActiveMiniGame, ActiveMiniGameCanvas);
        EventManager<MiniGame>.StartListening(MiniGame.DisActiveMiniGame, DisActiveMiniGameCanvas);
    }

    private void RemoveEvents()
    {
        EventManager<MiniGame>.StopListening<int, int>(MiniGame.ActiveMiniGame, ActiveMiniGameCanvas);
        EventManager<MiniGame>.StopListening(MiniGame.DisActiveMiniGame, DisActiveMiniGameCanvas);
    }

    private void ActiveMiniGameCanvas(int chapter, int stage)
    {
        // 스테이지가 10의 배수이거나 30일 때 실행
        if (chapter > 1 && (stage % 10 == 0 || stage == 30))
        {
            // index 계산: (챕터 - 2) * 3 + (stage / 10 - 1)
            int index = (chapter - 2) * 3 + (stage / 10 - 1);

            // 유효한 인덱스인지 확인 후 미니게임 활성화
            if (index >= 0 && index < miniGameList.Count)
            {
                miniGameList[index].gameObject.SetActive(true);
            }
        }
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
