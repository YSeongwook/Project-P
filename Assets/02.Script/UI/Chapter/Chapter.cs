using DataStruct;
using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter : MonoBehaviour
{
    [SerializeField] private Button Btn_ChapterMove;
    [SerializeField] private int _chapter;
    [SerializeField] private int _maxStageCount;

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

        for(int i=1; i<= _maxStageCount ; i++)
        {
            string mapID = $"M{_chapter}{i.ToString("000")}";
            var tableData = DataManager.Instance.GetTileMapTable(mapID);
            if (tableData.FileName == null) continue;
            string fileName = tableData.FileName;
            EventManager<DataEvents>.TriggerEvent(DataEvents.LoadThisChapterTileList, fileName);
        }

        EventManager<UIEvents>.TriggerEvent(UIEvents.CreateStageButton, _chapter, _maxStageCount);
    }
}
