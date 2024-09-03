using DataStruct;
using EnumTypes;
using EventLibrary;
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
        EventManager<StageEvent>.StartListening<ItemData, int>(StageEvent.LoadInventoryItem, SetItemDataUpdate);
    }

    private void RemoveEvent()
    {
        EventManager<StageEvent>.StopListening<ItemData, int>(StageEvent.LoadInventoryItem, SetItemDataUpdate);
    }

    public void SetItemDataUpdate(ItemData item, int count)
    {
        data = item;
        Count = count;
    }

    public void OnClick_UseItem()
    {
        // Test 용
        DebugLogger.Log(data.Name);
    }
}
