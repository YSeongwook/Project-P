using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using TMPro;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private DynamicObjectSelector objectSelector;

    private GameObject[] stages;

    protected new void Awake()
    {
        base.Awake();

        EventManager<UIEvents>.StartListening<int, int>(UIEvents.CreateStageButton, SetUpStages);      
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening<int, int>(UIEvents.CreateStageButton, SetUpStages);
    }

    private void SetUpStages(int chapter, int stageCount)
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        stages = new GameObject[stageCount];
        List<RectTransform> itemRects = new List<RectTransform>(); // RectTransform을 저장할 리스트

        for (int i = 0; i < stageCount; i++)
        {
            stages[i] = Instantiate(stagePrefab, contentTransform);
            var stage = stages[i].GetComponent<Stage>();
            if(stage == null) continue;
            stage.name = $"Stage{i+1}";

            stage.SetStageNumber(chapter, i+1);
            int playerChapter = PlayerInformation.Instance.GetPlayerCurrentChapter();
            int playerStage = PlayerInformation.Instance.GetPlayerCurrentStage();
            bool buttonActive = playerChapter > chapter? true : playerStage >= i + 1;
            stage.ButtonActivate(buttonActive);
            TMP_Text stageText = stages[i].GetComponentInChildren<TMP_Text>();
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
}
