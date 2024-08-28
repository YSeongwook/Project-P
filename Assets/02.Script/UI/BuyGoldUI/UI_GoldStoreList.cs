using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoldStoreList : MonoBehaviour
{
    [FoldoutGroup("Gold Shop List")] [SerializeField]
    private GameObject itemSlotPrefab;

    private Canvas _canvas;
    private RectTransform _rectTransform;
    private GridLayoutGroup _gridLayoutGroup;

    private Dictionary<string, GoldPackageData> _goldPackageDataDictionary;

    private float _widthValue;
    private float _cellHeight;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _widthValue = _rectTransform.rect.width;
        _cellHeight = _gridLayoutGroup.cellSize.y + _gridLayoutGroup.spacing.y;

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        _rectTransform.sizeDelta = new Vector2(_widthValue, 10);
        
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
        _goldPackageDataDictionary = DataManager.Instance.GetGoldPackageDatas();

        float count = 0;
        foreach (var PackageData in _goldPackageDataDictionary.Values)
        {
            count++;
            GameObject PackageSlot = Instantiate(itemSlotPrefab, this.transform);
            GoldPackageSlot goldPackageSlot = PackageSlot.GetComponent<GoldPackageSlot>();
            goldPackageSlot.SetPackageInfo(PackageData);
        }

        _rectTransform.sizeDelta = new Vector2(_widthValue, count * _cellHeight);
    }

    private void UI_PopUp()
    {
        _canvas.gameObject.SetActive(true);
    }

    private void UI_Exit()
    {
        _canvas.gameObject.SetActive(false);
    }
}
