using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter2_3 : MonoBehaviour
{
    [SerializeField] private List<Feed> _feedList;
    [SerializeField] private TMP_Text _timer;

    [SerializeField] private int _initTime;

    private Coroutine _gameCoroutine;

    private int _time;
    private int _enableFeedCount;

    private bool _isEnd;

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<MiniGame>.StartListening(MiniGame.FeedCountChanged, ActiveFeedCountChanged);
        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void RemoveEvent()
    {
        EventManager<MiniGame>.StopListening(MiniGame.FeedCountChanged, ActiveFeedCountChanged);
        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnEnable()
    {
        foreach (var item in _feedList)
        {
            item.gameObject.SetActive(true);
        }

        _enableFeedCount = _feedList.Count;

        _time = _initTime;
        _timer.text = _time.ToString();

        _isEnd = false;

        //_gameCoroutine = StartCoroutine(StartGame());
    }

    private void OnDisable()
    {
        if (_gameCoroutine == null) return;
        StopCoroutine(_gameCoroutine);
    }

    IEnumerator StartGame()
    {
        while (true)
        {
            if(_isEnd) yield break;

            yield return new WaitForSeconds(1f);

            if (_isEnd) yield break;

            _time--;
            _timer.text = $"{_time}";

            if(_time <= 0)
            {
                GameOver();
            }
        }
    }

    private void ActiveFeedCountChanged()
    {
        _enableFeedCount--;
        if(_enableFeedCount <= 0)
        {
            GameClear();
        }
    }

    private void GameOver()
    {
        //Text_CatchCount.text = "Clear 실패";
        _isEnd = true;

        StopCoroutine(_gameCoroutine);

        EventManager<MiniGame>.TriggerEvent(MiniGame.DisActiveMiniGame);

        EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail, true);
    }

    private void GameClear()
    {
        _isEnd = true;

        StopCoroutine(_gameCoroutine);

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

        _gameCoroutine = StartCoroutine(StartGame());
    }
}
