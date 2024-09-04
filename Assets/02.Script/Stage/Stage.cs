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

    public void OnClickStageButton()
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

    public void ButtonActivate(bool isEnable)
    {
        _btnStage.interactable = isEnable;
        _lock.enabled = !isEnable;
    }
}
