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

    private GameObject[] _stages;

    protected new void Awake()
    {
        base.Awake();
        EventManager<UIEvents>.StartListening<int, int>(UIEvents.CreateStageButton, SetUpStages);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening<int, int>(UIEvents.CreateStageButton, SetUpStages);
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

        _stages = new GameObject[stageCount];
        List<RectTransform> itemRects = new List<RectTransform>();

        for (int i = 0; i < stageCount; i++)
        {
            _stages[i] = Instantiate(stagePrefab, contentTransform);
            var stage = _stages[i].GetComponent<Stage>();
            if (stage == null) continue;
            stage.name = $"Stage {i + 1}";

            stage.SetStageNumber(chapter, i + 1);
            int playerChapter = PlayerInformation.Instance.GetPlayerCurrentChapter();
            int playerStage = PlayerInformation.Instance.GetPlayerCurrentStage();
            
            // 스테이지 해금 여부 확인
            bool buttonActive = (playerChapter > chapter) || (playerChapter == chapter && playerStage >= i + 1);
            stage.ButtonActivate(buttonActive);
            stage.SetLastStage(i == stageCount-1);

            // 미니 게임 스테이지 인지 확인
            stage.SetMiniGameStage((i+1) % 10 == 0);

            TMP_Text stageText = _stages[i].GetComponentInChildren<TMP_Text>();
            if (stageText != null)
            {
                stageText.text = (i + 1).ToString();
            }

            itemRects.Add(_stages[i].GetComponent<RectTransform>());
        }

        objectSelector.SetUpItems(itemRects);
    }
}
