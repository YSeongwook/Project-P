using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class UI_ItemStoreList : MonoBehaviour
{
    [SerializeField] private GameObject[] itemList;
    private Dictionary<string, ItemData> _itemDataDictionary;

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
        EventManager<UIEvents>.StartListening(UIEvents.OnCreateItemSlot, CreateItemSlot);
    }

    private void RemoveEvent()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnCreateItemSlot, CreateItemSlot);
    }

    // 상점 아이템 List 나열
    private void CreateItemSlot()
    {
        _itemDataDictionary = DataManager.Instance.GetItemInfoDatas();
        int count = 0;
        foreach (var itemslot in _itemDataDictionary.Values)
        {
            Item_Basic item = itemList[count++].GetComponent<Item_Basic>();
            item.SetItemInfo(itemslot);
        }
    }
}
