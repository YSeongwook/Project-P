using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TicketID
{
    T1001,
    T1002,
    T1003
}

public class TicketPakageSlot : MonoBehaviour
{
    [FoldoutGroup("Shop UI")][SerializeField] TMP_Text ticketCount; // 상점 티켓 갯수이름 UI 표시
    [FoldoutGroup("Shop UI")][SerializeField] TMP_Text textGoldPrice; // 상점 티켓 골드 가격 UI 표시
    [FoldoutGroup("Shop UI")][SerializeField] Image ticketIcon;
    [SerializeField] private TicketID id;

    private TicketData _ticketInfo; //티켓의 정보 

    private void Awake()
    {
        //EventManager<UIEvents>.StartListening<TicketData>(UIEvents.OnCreateTicketSlot, SetTicketInfo);
    }
    private void OnDestroy()
    {
        //EventManager<UIEvents>.StopListening<TicketData>(UIEvents.OnCreateTicketSlot, SetTicketInfo);
    }


    public void SetTicketInfo(TicketData ticketdata)
    {
        _ticketInfo = ticketdata;
        ticketCount.text = _ticketInfo.TicketCount;
        textGoldPrice.text = _ticketInfo.GoldPrice.ToString();
        if (_ticketInfo.Image != null)
        {
            ticketIcon.sprite = _ticketInfo.Image;
        }
        else
        {
            Debug.LogWarning("PackageIcon is null!");
        }
    }

    public void BuyTicket_Gold()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickEnableTicketBuyPopup);
        EventManager<DataEvents>.TriggerEvent<TicketData>(DataEvents.OnTicketPopupDataLoad , _ticketInfo);
    }
}

