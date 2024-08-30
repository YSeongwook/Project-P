using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chapter3_1 : MonoBehaviour
{
    [SerializeField] private TMP_Text Text_CatchCount;
    [SerializeField] private Transform Thiefs;
    private List<Thief> thiefList = new List<Thief>();

    [SerializeField] private int SetEndCount;
    private int CautchCount;

    private Coroutine gameCoroutine;
    private bool isEnd;

    private void Awake()
    {
        EventManager<MiniGame>.StartListening(MiniGame.Catch, Cautch);
        EventManager<MiniGame>.StartListening(MiniGame.PoliceGameOver, GameOver);
    }

    private void OnDestroy()
    {
        EventManager<MiniGame>.StopListening(MiniGame.Catch, Cautch);
        EventManager<MiniGame>.StopListening(MiniGame.PoliceGameOver, GameOver);
    }

    private void OnEnable()
    {
        thiefList.Clear();
        CautchCount = 0;
        isEnd = false;

        SetThiefList();
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

    private void Start()
    {
        gameCoroutine = StartCoroutine(StartTimer());
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

        if (CautchCount >= SetEndCount)
        {
            GameClear();
            yield break;
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
            isEnd = true;
    }

    private void GameOver()
    {
        Text_CatchCount.text = "Clear 실패";
        isEnd = true;

        StopCoroutine(gameCoroutine);
    }

    private void GameClear()
    {
        Text_CatchCount.text = "Clear";
        isEnd = true;
        // 디버그 용
    }

}
