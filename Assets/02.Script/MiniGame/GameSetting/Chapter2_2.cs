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

    private void OnEnable()
    {
        fishLists.Clear();
        isGameClear = false;
        timer = SetTimer;

        SetCameraToFish();
    }

    private void Start()
    {
        StartCoroutine(StartTimer());
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
    }

    private void GameClear()
    {
        // 디버그 용
        DebugLogger.Log($"게임 클리어 : {isGameClear}");
    }

    private bool CheckGameClear()
    {
        bool isClear = true;
        foreach(var  fish in fishLists)
        {
            if (fish == null) continue;
            if (!fish.isClearAble) 
                isClear = false;
        }

        return isClear;
    }
}
