using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStruct;
using EventLibrary;
using EnumTypes;

public class UI_TicketStoreList : MonoBehaviour
{
    [SerializeField] private GameObject[] ticketList;
    private Dictionary<string, TicketData> _ticketDataDictionary;

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnCreateTicketSlot, CreateTicketSlot);
    }

    private void RemoveEvent()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnCreateTicketSlot, CreateTicketSlot);
    }


    private void CreateTicketSlot()
    {
        _ticketDataDictionary = DataManager.Instance.GetTicketInfoDatas();
        int count = 0;
        foreach (var ticketslot in _ticketDataDictionary.Values)
        {
            TicketPakageSlot ticket = ticketList[count++].GetComponent<TicketPakageSlot>();
            ticket.SetTicketInfo(ticketslot);
        }
    }
}
