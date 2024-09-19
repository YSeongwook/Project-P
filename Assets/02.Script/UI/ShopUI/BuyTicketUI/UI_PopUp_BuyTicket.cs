using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopUp_BuyTicket : MonoBehaviour
{
    [FoldoutGroup("UI Prefab")][SerializeField] private Image ticketIcon;    //구매할 티켓의 아이콘 표시 UI
    [FoldoutGroup("UI Prefab")][SerializeField] private TMP_Text textTicketPrice; //구매할 티켓의 가격 표시 UI 
    [FoldoutGroup("UI Prefab")][SerializeField] private TMP_Text textBuyTicketCount; //구매할 티켓의 갯수 표시 UI

    private TicketData _data;     //구매할 티켓의 정보

    private void Awake()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnClickEnableTicketBuyPopup, PopUpOn);
        EventManager<DataEvents>.StartListening<TicketData>(DataEvents.OnTicketPopupDataLoad, SetBuyItem);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickEnableTicketBuyPopup, PopUpOn);
        EventManager<DataEvents>.StopListening<TicketData>(DataEvents.OnTicketPopupDataLoad, SetBuyItem);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetItemInfo(TicketData ticket)
    {
        textTicketPrice.text = ticket.GoldPrice.ToString();
        textBuyTicketCount.text = ticket.TicketCount;
        if (ticket.Image != null)
        {
            ticketIcon.sprite = ticket.Image;
        }
        else
        {
            Debug.LogWarning("ticketImage is null!");
        }

    }

    private void PopUpOn()
    {
        gameObject.SetActive(true);
    }


    private void SetBuyItem(TicketData ticket)
    {
        _data = ticket;
        SetItemInfo(_data);
    }


}