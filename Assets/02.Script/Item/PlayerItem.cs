using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private ItemID id;
    
    private Button _btnItemSlot;
    private ItemData _data;
    private int _count;

    // 현재 활성화된 아이템 유형 관리: 1번은 제한 증가, 2번은 역회전, 3번은 힌트
    private static ItemID? activeItemType = null;

    private void Awake()
    {
        _btnItemSlot = GetComponent<Button>();
        _btnItemSlot.onClick.AddListener(OnClick_UseItem);
    }

    private void OnEnable()
    {
        AddEvent();
    }

    private void OnDisable()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<StageEvent>.StartListening<Dictionary<ItemData, int>>(StageEvent.LoadInventoryItem, SetItemDataUpdate);
        EventManager<InventoryItemEvent>.StartListening(InventoryItemEvent.SetEnableButton, SetButtonActive);
        EventManager<InventoryItemEvent>.StartListening(InventoryItemEvent.CallbackPlayerResourceUI, DeactivateCurrentItem);
    }

    private void RemoveEvent()
    {
        EventManager<StageEvent>.StopListening<Dictionary<ItemData, int>>(StageEvent.LoadInventoryItem, SetItemDataUpdate);
        EventManager<InventoryItemEvent>.StopListening(InventoryItemEvent.SetEnableButton, SetButtonActive);
        EventManager<InventoryItemEvent>.StopListening(InventoryItemEvent.CallbackPlayerResourceUI, DeactivateCurrentItem);
    }

    private void SetItemDataUpdate(Dictionary<ItemData, int> itemData)
    {
        ItemData data = default;

        foreach (var item in itemData)
        {
            if (item.Key.ItemID == id.ToString())
            {
                data = item.Key;
                break;
            }
        }

        this._data = data;
        this._count = itemData[data];

        SetButtonActive();
    }

    private void SetButtonActive()
    {
        _btnItemSlot.interactable = _count > 0;
    }

    public void OnClick_UseItem()
    {
        if (_count < 1) return;

        // 1번 아이템은 제한 없이 사용 가능
        if (id == ItemID.I1001)
        {
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.UseItem, this._data.ItemID);
            _count--;
            return;
        }

        // 다른 아이템이 이미 활성화된 경우, 현재 아이템을 활성화하지 않음
        if (activeItemType != null && activeItemType != id) return;

        // 현재 아이템을 활성화 상태로 설정
        activeItemType = id;
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.UseItem, this._data.ItemID);
        _count--;

        // 현재 활성화된 아이템에 따라 이벤트 전달
        if (id == ItemID.I1002)
        {
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, true);
        }
        else if (id == ItemID.I1003)
        {
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetHint, true);
        }
    }

    // 아이템 사용 후 상태 초기화
    private static void DeactivateCurrentItem()
    {
        if (activeItemType == null) return;

        EventManager<InventoryItemEvent>.TriggerEvent(activeItemType == ItemID.I1002 ? InventoryItemEvent.SetReverseRotate : InventoryItemEvent.SetHint, false);
        activeItemType = null;
    }
}
