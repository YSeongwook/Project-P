using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using Sirenix.Reflection.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoldStoreList : MonoBehaviour
{
    [FoldoutGroup("Gold Shop List")]
    [SerializeField] GameObject ItemSlotPrefab;

    private Canvas canvas;
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;

    private Dictionary<int, GoldPackageData> GoldPackageDataDictionary;

    private float WidthValue;
    private float CellHeight;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        WidthValue = rectTransform.rect.width;
        CellHeight = gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y;

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        rectTransform.sizeDelta = new Vector2(WidthValue, 10);
        canvas.gameObject.SetActive(false);
    }

    private void AddEvent()
    {
        EventManager<UIEvents>.StartListening(UIEvents.GoldStorePopup, UI_PopUp);
        EventManager<UIEvents>.StartListening(UIEvents.GoldStoreExit, UI_Exit);
        EventManager<UIEvents>.StartListening(UIEvents.OnCreateGoldPackageSlot, CreateGoldPackageSlot);
    }

    private void RemoveEvent()
    {
        EventManager<UIEvents>.StopListening(UIEvents.GoldStorePopup, UI_PopUp);
        EventManager<UIEvents>.StopListening(UIEvents.GoldStoreExit, UI_Exit);
        EventManager<UIEvents>.StopListening(UIEvents.OnCreateGoldPackageSlot, CreateGoldPackageSlot);
    }

    private void CreateGoldPackageSlot()
    {
        GoldPackageDataDictionary = DataManager.Instance.GetGoldPackageDatas();

        float Count = 0;
        foreach (var PackageData in GoldPackageDataDictionary.Values)
        {
            Count++;
            GameObject PackageSlot = Instantiate(ItemSlotPrefab, this.transform);
            GoldPackageSlot goldPackageSlot = PackageSlot.GetComponent<GoldPackageSlot>();
            goldPackageSlot.SetPackageInfo(PackageData);
        }

        rectTransform.sizeDelta = new Vector2(WidthValue, Count * CellHeight);
    }

    private void UI_PopUp()
    {
        canvas.gameObject.SetActive(true);
    }

    private void UI_Exit()
    {
        canvas.gameObject.SetActive(false);
    }
}
