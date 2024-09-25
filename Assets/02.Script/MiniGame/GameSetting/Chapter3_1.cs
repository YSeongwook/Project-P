using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter3_1 : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_CatchCount;
    [SerializeField] private Transform Thiefs;
    private List<Thief> thiefList = new List<Thief>();

    [SerializeField] private int SetEndCount;
    private int CautchCount;

    private Coroutine gameCoroutine;
    private bool isEnd;
    private bool isGameStart;

    private void Awake()
    {
        EventManager<MiniGame>.StartListening(MiniGame.Catch, Cautch);
        EventManager<MiniGame>.StartListening(MiniGame.PoliceGameOver, GameOver);
        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnDestroy()
    {
        EventManager<MiniGame>.StopListening(MiniGame.Catch, Cautch);
        EventManager<MiniGame>.StopListening(MiniGame.PoliceGameOver, GameOver);
        EventManager<MiniGame>.StopListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnEnable()
    {
        thiefList.Clear();
        CautchCount = 0;
        isEnd = false;
        Text_CatchCount.text = $"{CautchCount}";

        SetThiefList();        
    }

    private void OnDisable()
    {
        isEnd = true;
    }

    private void SetThiefList()
    {
        foreach(Transform child in Thiefs)
        {
            var thief = child.GetComponent<Thief>();
            if (thief == null) continue;
            thiefList.Add(thief);
        }
    }


    IEnumerator StartTimer()
    {
        while (!isEnd)
        {
            if (isEnd) yield break;

            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

            if (isEnd) yield break;

            AppearThief();
        }        
    }

    private void AppearThief()
    {
        var index = Random.Range(0, thiefList.Count);

        var thief = thiefList[index].GetComponent<Thief>();
        thief.AppearThief();
    }

    private void Cautch()
    {
        CautchCount++;

        Text_CatchCount.text = $"{CautchCount}";

        if (CautchCount >= SetEndCount)
        {
            StopCoroutine(gameCoroutine);
            GameClear();
        }
    }

    private void GameOver()
    {
        //Text_CatchCount.text = "Clear 실패";
        isEnd = true;

        StopCoroutine(gameCoroutine);

        EventManager<MiniGame>.TriggerEvent(MiniGame.DisActiveMiniGame);

        EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail, true);
    }

    private void GameClear()
    {
        Text_CatchCount.text = "Clear";
        isEnd = true;
        // 디버그 용
        EventManager<MiniGame>.TriggerEvent(MiniGame.DisActiveMiniGame);

        EventManager<StageEvent>.TriggerEvent(StageEvent.MissionSuccess);
        // 맵 이미지 변경
    }

    private void SetGameStart(bool isGameStart)
    {
        if (!isGameStart) return;
        var canvas = transform.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        if (!canvas.gameObject.activeSelf) return;

        gameCoroutine = StartCoroutine(StartTimer());
    }
}
