using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    // 스테이지 결과 패널
    [FoldoutGroup("Stage Clear Panel")] [SerializeField] private GameObject stageClearPanel; // 스테이지 클리어 패널
    [FoldoutGroup("Stage Clear Panel")] [SerializeField] private Button lobbyButton; // 로비 귀환 버튼
    [FoldoutGroup("Stage Clear Panel")] [SerializeField] private Button nextButton; // 다음 스테이지 버튼
    
    [FoldoutGroup("Stage Fail Panel")] [SerializeField] private GameObject stageFailPanel; // 스테이지 실패 패널
    [FoldoutGroup("Stage Fail Panel")] [SerializeField] private Button exitButton; // 나가기 버튼
    [FoldoutGroup("Stage Fail Panel")] [SerializeField] private Button restartButton; // 다시하기 버튼
    
    // 스테이지 메뉴 패널
    [FoldoutGroup("Stage Menu Panel")] [SerializeField] private TMP_Text limitCountText; // 제한 횟수 텍스트

    private Canvas _canvas;
    private int _limitCount;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        
        AddEvents();
        AddButtonEvents();
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
        RemoveButtonEvents();
    }

    // 이벤트 리스너 등록
    private void AddEvents()
    {
        EventManager<StageEvent>.StartListening<int>(StageEvent.StartStage, UpdateLimitCount);
        EventManager<StageEvent>.StartListening(StageEvent.UseTurn, DecreaseLimitCount);
        EventManager<StageEvent>.StartListening<bool>(StageEvent.StageClear, EnableStageClearPanel);
        EventManager<StageEvent>.StartListening<bool>(StageEvent.StageFail, EnableStageFailPanel);
        EventManager<StageEvent>.StartListening(StageEvent.RecoveryLimitCount, IncreaseLimitCount);
    }

    // 이벤트 리스너 해제
    private void RemoveEvents()
    {
        EventManager<StageEvent>.StopListening<int>(StageEvent.StartStage, UpdateLimitCount);
        EventManager<StageEvent>.StopListening(StageEvent.UseTurn, DecreaseLimitCount);
        EventManager<StageEvent>.StopListening<bool>(StageEvent.StageClear, EnableStageClearPanel);
        EventManager<StageEvent>.StopListening<bool>(StageEvent.StageFail, EnableStageFailPanel);
        EventManager<StageEvent>.StopListening(StageEvent.RecoveryLimitCount, IncreaseLimitCount);
    }

    // 버튼 이벤트 리스너 등록
    private void AddButtonEvents()
    {
        lobbyButton.onClick.AddListener(OnClickExitButton);
        nextButton.onClick.AddListener(OnClickNextButton);
        exitButton.onClick.AddListener(OnClickExitButton);
        restartButton.onClick.AddListener(OnClickRestartButton);
    }

    // 버튼 이벤트 리스너 해제
    private void RemoveButtonEvents()
    {
        lobbyButton.onClick.RemoveListener(OnClickExitButton);
        nextButton.onClick.RemoveListener(OnClickNextButton);
        exitButton.onClick.RemoveListener(OnClickExitButton);
        restartButton.onClick.RemoveListener(OnClickRestartButton);
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

        DebugLogger.Log(_limitCount);

        EventManager<DataEvents>.TriggerEvent(DataEvents.DecreaseLimitCount, _limitCount);
    }

    // 스테이지 선택 화면으로 돌아가기
    private void ReturnSelectStage()
    {
        // 스테이지 성공, 실패 패널, 스테이지 UI 비활성화
        stageClearPanel.SetActive(false);
        stageFailPanel.SetActive(false);
        _canvas.enabled = false;
    }

    // 스테이지 클리어 패널 활성화
    private void EnableStageClearPanel(bool enable)
    {
        stageClearPanel.SetActive(enable);
    }
    
    // 스테이지 실패 패널 활성화
    private void EnableStageFailPanel(bool enable)
    {
        stageFailPanel.SetActive(enable);
    }
    
    // 나가기 버튼 클릭 시 스테이지 선택 화면으로 전환
    public void OnClickExitButton()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.ReturnSelectStage);
        
        ReturnSelectStage();
    }

    // 스테이지 클리어 후 다음 버튼 클릭 시 다음 스테이지로 전환
    public void OnClickNextButton()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickNextButton);
    }

    public void OnClickRestartButton()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickRestartButton);
    }
}