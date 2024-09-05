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
        EventManager<StageEvent>.StartListening(StageEvent.StageClear, UnlockNextStage);  // 스테이지 클리어 후 다음 스테이지 해금
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening<int, int>(UIEvents.CreateStageButton, SetUpStages);
        EventManager<StageEvent>.StopListening(StageEvent.StageClear, UnlockNextStage);
    }

    // 스테이지 클리어 후 다음 스테이지 해금
    private void UnlockNextStage()
    {
        int currentChapter = PlayerInformation.Instance.GetPlayerCurrentChapter();
        int currentStage = PlayerInformation.Instance.GetPlayerCurrentStage();

        int maxStages = currentChapter == 1 ? 5 : 20;

        // 다음 스테이지가 존재하는지 확인
        if (currentStage < maxStages)
        {
            // 다음 스테이지가 존재하면 해금
            PlayerInformation.Instance.UnlockStage(currentChapter, currentStage + 1);
        }
        else if (currentChapter < 4)
        {
            // 마지막 스테이지가 끝났으면 다음 챕터의 첫 스테이지 해금
            PlayerInformation.Instance.UnlockStage(currentChapter + 1, 1);
        }

        // 스테이지 버튼 업데이트
        SetUpStages(currentChapter, maxStages);
    }

    public void MoveToNextStage()
    {
        int currentChapter = PlayerInformation.Instance.GetPlayerCurrentChapter();
        int currentStage = PlayerInformation.Instance.GetPlayerCurrentStage();

        // 다음 스테이지 번호 계산
        int nextStage = currentStage + 1;
        int maxStages = currentChapter == 1 ? 5 : 20;

        if (nextStage > maxStages)
        {
            currentChapter++;
            nextStage = 1;
            if (currentChapter > 4)  // 마지막 챕터를 넘어섰다면 더 이상 스테이지가 없음
            {
                DebugLogger.Log("더 이상 스테이지가 없습니다.");
                return;
            }
        }

        // 입장권이 충분한지 확인
        if (PlayerInformation.Instance.PlayerViewModel.GameTickets <= 0)
        {
            DebugLogger.Log("입장권이 부족합니다.");
            return;
        }

        // 스테이지 이동
        PlayerInformation.Instance.UpdatePlayerStage(currentChapter, nextStage);  // 플레이어의 현재 스테이지 갱신
        EventManager<DataEvents>.TriggerEvent(DataEvents.SelectStage, currentChapter, nextStage);
    }

    private void SetUpStages(int chapter, int stageCount)
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        stages = new GameObject[stageCount];
        List<RectTransform> itemRects = new List<RectTransform>();

        for (int i = 0; i < stageCount; i++)
        {
            stages[i] = Instantiate(stagePrefab, contentTransform);
            var stage = stages[i].GetComponent<Stage>();
            if (stage == null) continue;
            stage.name = $"Stage{i + 1}";

            stage.SetStageNumber(chapter, i + 1);
            int playerChapter = PlayerInformation.Instance.GetPlayerCurrentChapter();
            int playerStage = PlayerInformation.Instance.GetPlayerCurrentStage();
            
            // 스테이지 해금 여부 확인
            bool buttonActive = (playerChapter > chapter) || (playerChapter == chapter && playerStage >= i + 1);
            stage.ButtonActivate(buttonActive);

            TMP_Text stageText = stages[i].GetComponentInChildren<TMP_Text>();
            if (stageText != null)
            {
                stageText.text = (i + 1).ToString();
            }

            itemRects.Add(stages[i].GetComponent<RectTransform>());
        }

        objectSelector.SetUpItems(itemRects);
    }
}
