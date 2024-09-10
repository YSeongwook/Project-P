using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemStoreList : MonoBehaviour
{
    [SerializeField] private GameObject[] itemList;
    private Dictionary<string, ItemData> _itemDataDictionary;


    private void Awake()
    {
        AddEvent();
    }

    private void Start()
    {
        GameObject.Find("Canvas_ItemShop").SetActive(false);
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
        foreach (var itemData in _itemDataDictionary.Values)
        {
            GameObject itemSlot = itemList[count++];
            Item_Basic itemSlotData = itemSlot.GetComponent<Item_Basic>();
            itemSlotData.SetItemInfo(itemData);
            //count++;
        }
    }
}
