using DataStruct;
using EnumTypes;
using EventLibrary;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItem : MonoBehaviour
{
    private ItemData data;
    private int Count;

    [SerializeField] private ItemID id;

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
        EventManager<StageEvent>.StartListening< Dictionary<ItemData, int>  > (StageEvent.LoadInventoryItem, SetItemDataUpdate);
    }

    private void RemoveEvent()
    {
        EventManager<StageEvent>.StopListening< Dictionary<ItemData, int>  > (StageEvent.LoadInventoryItem, SetItemDataUpdate);
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
    }

    public void OnClick_UseItem()
    {
        // Test 용
        DebugLogger.Log(data.Name);

        // 아이템 사용
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.UseItem, data.ItemID);
    }
}
