using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    private int _chapter; 
    
    private Button _btnStage;
    private Image _lock;

    private void Awake()
    {
        _btnStage = GetComponent<Button>();
        _lock = transform.GetChild(0).GetComponent<Image>();
    }
    
    public void SetStageNumber(int chapter, int number)
    {
        _chapter = chapter;
        stageNumber = number;
    }
    
    // Todo: 스테이지 입장 메서드 생성해서 스테이지 클리어 후 다음 스테이지 입장 시 사용
    private void OpenStage()
    {
        if(PlayerInformation.Instance.PlayerViewModel.GameTickets <= 0)
        {
            DebugLogger.Log("티켓의 수가 부족합니다.");
            return;
        }

        EventManager<DataEvents>.TriggerEvent(DataEvents.SelectStage, this._chapter,stageNumber);

        // 플레이어 아이템 게임 캔버스에 적용
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetPlayerItemInventoryList);
    }

    public void OnClickStageButton()
    {
        OpenStage();
    }

    public void ButtonActivate(bool isEnable)
    {
        _btnStage.interactable = isEnable;
        _lock.enabled = !isEnable;
    }
}
