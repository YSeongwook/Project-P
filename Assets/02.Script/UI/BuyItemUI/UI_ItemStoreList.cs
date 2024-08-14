using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemStoreList : MonoBehaviour
{ 
    [FoldoutGroup("Item Shop List")] [SerializeField] private GameObject itemSlotPrefab;

    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;
    private float _widthValue;
    private Dictionary<int, ItemData> _itemDataDictionary;

    private float _cellHeight;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _widthValue = _rectTransform.rect.width;
        _cellHeight = _gridLayoutGroup.cellSize.y + _gridLayoutGroup.spacing.y;

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

    private void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        canvas.gameObject.SetActive(false);
    }

    // 상점 아이템 List 나열
    private void CreateItemSlot()
    {
        _itemDataDictionary = DataManager.Instance.GetItemInfoDatas();

        float count = 0; // rectTransform.rect.height;
        foreach (var itemData in _itemDataDictionary.Values)
        {
            count++;
            GameObject itemSlot = Instantiate(itemSlotPrefab, this.transform);
            Item_Basic itemSlotData = itemSlot.GetComponent<Item_Basic>();
            itemSlotData.SetItemInfo(itemData);
        }

        _rectTransform.sizeDelta = new Vector2(_widthValue, count * _cellHeight);
    }
}
