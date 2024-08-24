using DataStruct;
using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Chapter : MonoBehaviour
{
    [SerializeField] private Button Btn_ChapterMove;
    [SerializeField] private int _chapter;
    [SerializeField] private int _maxStageCount;

    private void Awake()
    {
        EventManager<DataEvents>.StartListening<int, int>(DataEvents.UpdateCurrentChapterAndStage, ChangedPlayerChpaterAndStage);
    }

    private void OnDestroy()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.UpdateCurrentChapterAndStage, ChangedPlayerChpaterAndStage);
    }

    private void Start()
    {
        if(_chapter == 1)
        {
            OnClickChapterMove();
        }
    }

    //버튼 비활성화 및 활성화
    public void DeactivateButton(int PlayerOpenChapter)
    {
        bool active = PlayerOpenChapter >= _chapter;
        Btn_ChapterMove.interactable = active;
    }

    //버튼 누르면
    public void OnClickChapterMove()
    {
        // 스테이지 버튼을 생성하고
        // 해당 챕터의 스테이지 Tile Map들을 모두 파싱한다.

        // 챕터 타일 리스트 초기화
        EventManager<DataEvents>.TriggerEvent(DataEvents.ResetChapterTileList);

        for (int i = 1; i <= _maxStageCount; i++)
        {
            string mapID = $"M{_chapter}{i.ToString("000")}";
            var tableData = DataManager.Instance.GetTileMapTable(mapID);
            if (tableData.FileName == null) continue;
            string fileName = tableData.FileName;
            EventManager<DataEvents>.TriggerEvent(DataEvents.LoadThisChapterTileList, fileName);
        }

        EventManager<UIEvents>.TriggerEvent(UIEvents.CreateStageButton, _chapter, _maxStageCount);
    }

    // 플레이어 챕터 및 스테이지 해금
    private void ChangedPlayerChpaterAndStage(int currentChapter, int currentStage)
    {
        // 현재 플레이어의 챕터 및 스테이지 가져오기
        int chapter = PlayerInformation.Instance.GetPlayerCurrentChapter();
        int stage = PlayerInformation.Instance.GetPlayerCurrentStage();

        if (_chapter != chapter) return;
        if (currentChapter < chapter || currentStage < stage) return;

        int newChapter = chapter;   
        var newStage = stage + 1;

        if (newStage >= _maxStageCount)
        {
            newChapter = Mathf.Clamp(newChapter+1, 0, 5);
            newStage = 1;
        }

        // 증가 반영
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerCurrentChapterChanged, newChapter);
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerCurrentStageChanged, newStage);

        // 챕터 UI 반영
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnEnableChapterMoveButton, _chapter);
        // 스테이지 UI 반영
        EventManager<UIEvents>.TriggerEvent(UIEvents.CreateStageButton, _chapter, _maxStageCount);

        // 스테이지 UI - 해금된 스테이지로 이동

        DebugLogger.Log($"new Stage : {newStage}");

        DebugLogger.Log("Chapter : " + PlayerInformation.Instance.PlayerViewModel.CurrentChapter);
        DebugLogger.Log("Stage : " + PlayerInformation.Instance.PlayerViewModel.CurrentStage);
    }
}
