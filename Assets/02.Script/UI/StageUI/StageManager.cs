using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private DynamicObjectSelector objectSelector;
    //[SerializeField] private GameObject MainGameUI;
    //[SerializeField] private GameObject GameLobbyUI;

    private GameObject[] stages;

    private void Start()
    {
        SetUpStages(1);        
    }

   public void SetUpStages(int chapter)
    {
        int stageCount = GetStageCountForChapter(chapter);

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        stages = new GameObject[stageCount];
        List<RectTransform> itemRects = new List<RectTransform>(); // RectTransform을 저장할 리스트

        for (int i = 0; i < stageCount; i++)
        {
            stages[i] = Instantiate(stagePrefab, contentTransform);
            Transform childTransform = stages[i].transform.Find("Text_StageCountTitle");
            TextMeshProUGUI stageText = childTransform.GetComponent<TextMeshProUGUI>();
            if (stageText != null)
            {
                stageText.text = (i + 1).ToString();
            }
            // 생성된 스테이지의 RectTransform을 리스트에 추가
            itemRects.Add(stages[i].GetComponent<RectTransform>());
        }

        // DynamicObjectSelector에 리스트 전달
        objectSelector.SetUpItems(itemRects);
    }

    public void OnClickChapter1()
    {
        SetUpStages(1);
    }

    public void OnClickChapter2()
    {
        SetUpStages(2);
    }

    public void OnClickChapter3()
    {
        SetUpStages(3);
    }

    public void OnClickChapter4()
    {
        SetUpStages(4);
    }

    //public void OnClickChapterCanvas()
    //{
    //    MainGameUI.SetActive(true);
    //    GameLobbyUI.SetActive(false);
    //}

    private int GetStageCountForChapter(int chapter)
    {
        switch (chapter)
        {
            case 1:
                return 20;
            case 2:
                return 15;
            case 3:
                return 7;
            default:
                return 7;
        }
    }
}
