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

    private Canvas _canvas;
    private int _limitCount;

    private void Awake()
    {
        EventManager<StageEvent>.StartListening<int>(StageEvent.StartStage, UpdateLimitCount);
        EventManager<StageEvent>.StartListening(StageEvent.UseTurn, DecreaseLimitCount);
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
        EventManager<StageEvent>.StopListening<int>(StageEvent.StartStage, UpdateLimitCount);
        EventManager<StageEvent>.StopListening(StageEvent.UseTurn, DecreaseLimitCount);
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

        EventManager<DataEvents>.TriggerEvent(DataEvents.DecreaseLimitCount);
    }
    
    // 다시하기 버튼 클릭
    public void OnClickReplayBtn()
    {
        // missionFailPanel.SetActive(false);
        // Todo: 재도전 이벤트 발생
        // OpenNewStage(_currentChapter, _currentStage);
    }

    // 메인 UI로 돌아가기
    public void OnClickReturnSelectStage()
    {
        // UI 변화
        // missionFailPanel.SetActive(false);
        // missionSuccessPanel.SetActive(false);
        _canvas.enabled = false;
        // Todo: 스테이지 UI 비활성화 및 메인 메뉴 및 플레이어 재화 UI 활성화 이벤트 발생
        // mainMenuUI.enabled = true;
        // playerGoldUI.enabled = true;
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
}