using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class StageUI : MonoBehaviour
{
    // 스테이지 결과 패널
    [FoldoutGroup("Stage Result Panel")] [SerializeField] private GameObject stageClearPanel; // 스테이지 클리어 패널
    [FoldoutGroup("Stage Result Panel")] [SerializeField] private GameObject stageFailPanel; // 스테이지 실패 패널
    
    // 스테이지 메뉴 패널
    [FoldoutGroup("Stage Menu Panel")] [SerializeField] private TMP_Text limitCountText; // 제한 횟수 텍스트
    
    private int _limitCount;

    private void Awake()
    {
        AddEvents();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _limitCount = 0;
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<StageEvent>.StartListening<int>(StageEvent.StartStage, UpdateLimitCount);
        EventManager<StageEvent>.StartListening(StageEvent.UseTurn, DecreaseLimitCount);
        EventManager<StageEvent>.StartListening(StageEvent.StageClear, EnableStageClearPanel);
        EventManager<StageEvent>.StartListening(StageEvent.StageFail, EnableStageFailPanel);
    }

    private void RemoveEvents()
    {
        EventManager<StageEvent>.StopListening<int>(StageEvent.StartStage, UpdateLimitCount);
        EventManager<StageEvent>.StopListening(StageEvent.UseTurn, DecreaseLimitCount);
        EventManager<StageEvent>.StopListening(StageEvent.StageClear, EnableStageClearPanel);
        EventManager<StageEvent>.StopListening(StageEvent.StageFail, EnableStageFailPanel);
        EventManager<StageEvent>.StopListening(StageEvent.RecoveryLimitCount, IncreaseLimitCount);
    }

    // 제한 횟수 UI 업데이트
    private void UpdateLimitCount(int limitCount)
    {
        _limitCount = limitCount;
        limitCountText.text = $"{_limitCount}";
    }

    // 타일 클릭 시 제한 횟수 감소
    private void DecreaseLimitCount()
    {
        _limitCount -= 1;
        limitCountText.text = $"{_limitCount}";

        EventManager<DataEvents>.TriggerEvent(DataEvents.DecreaseLimitCount, _limitCount);
    }

    private void IncreaseLimitCount()
    {
        _limitCount += 5;
        limitCountText.text = $"{_limitCount}";

        EventManager<DataEvents>.TriggerEvent(DataEvents.DecreaseLimitCount, _limitCount);
    }
    
    // 다시하기 버튼 클릭
    public void OnClickReplayBtn()
    {
        // missionFailPanel.SetActive(false);
        // Todo: 재도전 이벤트 발생
        // OpenNewStage(_currentChapter, _currentStage);
    }

    // 메인 UI로 돌아가기
    private void ReturnSelectStage()
    {
        // 스테이지 성공, 실패 패널, 스테이지 UI 비활성화
        stageClearPanel.SetActive(false);
        stageFailPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    // 스테이지 클리어 패널 활성화
    private void EnableStageClearPanel()
    {
        stageClearPanel.SetActive(true);
    }
    
    // 스테이지 실패 패널 활성화
    private void EnableStageFailPanel()
    {
        stageFailPanel.SetActive(true);
    }
    
    // 스테이지 클리어 후 메뉴로 돌아가기 버튼 클릭 메서드
    public void OnClickReturnSelectStageButton()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.ReturnSelectStage);
        
        ReturnSelectStage();
    }

    public void OnClickExitButton()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.RestartStage);
    }
}