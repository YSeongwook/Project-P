using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//디버그용
public enum ItemID
{
    I1001,
    I1002,
    I1003,
    I1004,
    I1005,
}

public enum PaymentMethod
{
    Gold,
    ERC
}

public class Item_Basic : MonoBehaviour
{
    [FoldoutGroup("Shop UI")] [SerializeField] TMP_Text textName; // 상점 아이템 이름 UI 표시
    [FoldoutGroup("Shop UI")] [SerializeField] TMP_Text textGoldPrice; // 상점 아이템 골드 가격 UI 표시
    [FoldoutGroup("Shop UI")] [SerializeField] Image itemIcon;
    [SerializeField] private GameObject itemBuyPopup; // 팝업 프리팹

    private ItemData _itemInfo; // 아이템의 정보

    //아이템 정보 설정
    public void SetItemInfo(ItemData itemdata)
    {
        _itemInfo = itemdata;

        textName.text = _itemInfo.Name;
        textGoldPrice.text = _itemInfo.GoldPrice.ToString();
        if(_itemInfo.Image != null)
        {
            itemIcon.sprite = _itemInfo.Image;
        }
        else
        {
            Debug.LogWarning("PackageIcon is null!");
        }
    }

    public void OnSlotClick()
    {
        if (itemBuyPopup != null)
        {
            var popup = itemBuyPopup.GetComponent<UI_PopUp_BuyItem>();

            if (popup != null)
            {
                popup.SetItemInfo(_itemInfo);
                itemBuyPopup.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Gold Buy Popup is not assigned.");
        }
    }

    private void Awake()
    {
        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
    }

    private void RemoveEvents()
    {
    }

    //BuyItemUIPopUp
    public void BuyItem_Gold()
    {
        EventManager<DataEvents>.TriggerEvent<ItemData>(DataEvents.OnItemDataLoad, _itemInfo);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickEnableItemBuyPopup);
    }
}