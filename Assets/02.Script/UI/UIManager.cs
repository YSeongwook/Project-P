using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [FoldoutGroup("Player Currency Canvas")] [SerializeField] private Canvas playerCurrencyCanvas;
    [FoldoutGroup("Main Menu Canvas")] [SerializeField] private Canvas mainMenuCanvas;
    [FoldoutGroup("Stage Canvas")] [SerializeField] private Canvas stageCanvas;
    
    protected override void Awake()
    {
        base.Awake();

        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    // 이벤트 리스너 등록
    private void AddEvents()
    {
        EventManager<StageEvent>.StartListening(StageEvent.EnterStage, EnterStage);
    }
    
    // 이벤트 리스너 제거
    private void RemoveEvents()
    {
        EventManager<StageEvent>.StopListening(StageEvent.EnterStage, EnterStage);
    }

    // 스테이지 입장 시 메인 메뉴, 플레이어 재화 UI 비활성화
    private void EnterStage()
    {
        playerCurrencyCanvas.enabled = false;
        mainMenuCanvas.enabled = false;
        stageCanvas.enabled = true;
    }
}