using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter2_2 : MonoBehaviour
{
    [SerializeField] private Transform fishs;
    [SerializeField] private TMP_Text Text_timer;
    [SerializeField] private float SetTimer;
    private float timer;

    private List<Fish> fishLists = new List<Fish>();
    private bool isGameClear;

    private Coroutine _miniGame;

    private void Awake()
    {
        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnDestroy()
    {
        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnEnable()
    {
        fishLists.Clear();
        isGameClear = false;
        timer = SetTimer;
        Text_timer.text = timer.ToString();

        SetCameraToFish();        
    }

    private void OnDisable()
    {
        if(_miniGame ==  null) return;
        StopCoroutine(_miniGame);
    }

    private void SetCameraToFish()
    {
        foreach(Transform child in fishs)
        {
            var fish = child.GetComponent<Fish>();
            if(fish == null) continue;

            fishLists.Add(fish);
        }
    }

    IEnumerator StartTimer()
    {
        while (true)
        {
            yield return null;
            timer -= Time.deltaTime;
            Text_timer.text = timer.ToString("0");

            isGameClear = CheckGameClear();

            if (isGameClear)
            {
                GameClear();
                yield break;
            }

            if (timer <= 0)
            {
                GameOver();
                yield break;
            }
        }
    }

    private void GameOver()
    {
        DebugLogger.Log($"Clear 실패");

        EventManager<MiniGame>.TriggerEvent(MiniGame.DisActiveMiniGame);

        EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail, true);
    }

    private void GameClear()
    {
        // 디버그 용
        DebugLogger.Log($"게임 클리어 : {isGameClear}");

        EventManager<MiniGame>.TriggerEvent(MiniGame.DisActiveMiniGame);

        EventManager<StageEvent>.TriggerEvent(StageEvent.MissionSuccess);

        // 맵 이미지 변경
    }

    private bool CheckGameClear()
    {
        bool isClear = true;
        foreach(var  fish in fishLists)
        {
            if (fish == null) continue;
            if (!fish.IsClearAble) 
                isClear = false;
        }

        return isClear;
    }

    private void SetGameStart(bool isGameStart)
    {
        if (!isGameStart) return;
        var canvas = transform.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        if (!canvas.gameObject.activeSelf) return;

        DebugLogger.Log(canvas.gameObject.name + " : " + !canvas.gameObject.activeSelf);

        _miniGame = StartCoroutine(StartTimer());
    }
}
