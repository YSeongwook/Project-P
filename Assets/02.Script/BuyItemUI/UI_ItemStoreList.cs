using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ItemStoreList : MonoBehaviour
{
    [FoldoutGroup("Item Shop List")]
    [SerializeField] GameObject ItemSlotPrefab;

    private RectTransform rectTransform;
    private float WidthValue;
    private Dictionary<int, ItemData> ItemDataDictionary;

    private void Awake()
    {
        ItemDataDictionary = DataManager.Instance.GetItemInfoDatas();
        rectTransform = GetComponent<RectTransform>();
        WidthValue = rectTransform.rect.width;
    }

    private void Start()
    {
        rectTransform.sizeDelta = new Vector2(WidthValue, 10);

        foreach (var itemData in ItemDataDictionary.Values)
        {
            rectTransform.sizeDelta = new Vector2(WidthValue, rectTransform.rect.height + 155f);
            GameObject itemSlot = Instantiate(ItemSlotPrefab, this.transform);
            Item_Basic itemSlotData = itemSlot.GetComponent<Item_Basic>();
            itemSlotData.SetItemInfo(itemData);
        }
    }
}
