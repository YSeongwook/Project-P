using DataStruct;
using EnumTypes;
using EventLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    private Button Btn_ItemSlot;
    private ItemData data;
    private int Count;

    [SerializeField] private ItemID id;

    private void Awake()
    {
        Btn_ItemSlot = GetComponent<Button>();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<StageEvent>.StartListening< Dictionary<ItemData, int>  > (StageEvent.LoadInventoryItem, SetItemDataUpdate);
        EventManager<InventoryItemEvent>.StartListening(InventoryItemEvent.SetEnableButton, SetButtonActive);
    }

    private void RemoveEvent()
    {
        EventManager<StageEvent>.StopListening< Dictionary<ItemData, int>  > (StageEvent.LoadInventoryItem, SetItemDataUpdate);
        EventManager<InventoryItemEvent>.StopListening(InventoryItemEvent.SetEnableButton, SetButtonActive);
    }

    public void SetItemDataUpdate(Dictionary<ItemData, int> itemData)
    {
        ItemData data = default;

        foreach(var item in itemData)
        {
            if(item.Key.ItemID == id.ToString())
            {
                data = item.Key;
                break;
            }
        }

        this.data = data;
        this.Count = itemData[data];

        SetButtonActive();
    }

    // 버튼 활성화
    private void SetButtonActive()
    {
        if (Count <= 0) Btn_ItemSlot.interactable = false;
        else Btn_ItemSlot.interactable = true;
    }

    public void OnClick_UseItem()
    {
        // 아이템 사용
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.UseItem, data.ItemID);
    }
}
