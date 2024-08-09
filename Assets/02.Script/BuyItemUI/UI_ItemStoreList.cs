using EnumTypes;
using DataStruct;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Reflection.Editor;

public class UI_ItemStoreList : MonoBehaviour
{
    [FoldoutGroup("Item Shop List")]
    [SerializeField] GameObject ItemSlotPrefab;

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;
    private float WidthValue;
    private Dictionary<int, ItemData> ItemDataDictionary;

    private float CellHeight;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        WidthValue = rectTransform.rect.width;
        CellHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;

        AddEvenet();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvenet()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnCreateItemSlot, CreateItemSlot);
    }

    private void RemoveEvent()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnCreateItemSlot, CreateItemSlot);
    }

    private void CreateItemSlot()
    {
        ItemDataDictionary = DataManager.Instance.GetItemInfoDatas();

        float Count = 0;//rectTransform.rect.height;
        foreach (var itemData in ItemDataDictionary.Values)
        {
            Count++;
            GameObject itemSlot = Instantiate(ItemSlotPrefab, this.transform);
            Item_Basic itemSlotData = itemSlot.GetComponent<Item_Basic>();
            itemSlotData.SetItemInfo(itemData);
        }

        rectTransform.sizeDelta = new Vector2(WidthValue, Count * CellHeight);
    }

}
