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
    I1003
}

public enum PaymentMethod
{
    Gold,
    ERC
}

public class Item_Basic : MonoBehaviour
{
    [FoldoutGroup("Shop UI")][SerializeField] TMP_Text textName; // 상점 아이템 이름 UI 표시
    [FoldoutGroup("Shop UI")][SerializeField] TMP_Text textGoldPrice; // 상점 아이템 골드 가격 UI 표시
    [FoldoutGroup("Shop UI")][SerializeField] Image itemIcon;
    [SerializeField] private ItemID id;

    public ItemID ID
    {
        get { return this.id; }
        private set { this.id = value; }
    }

    private ItemData _itemInfo; // 아이템의 정보

    //아이템 정보 설정
    public void SetItemInfo(ItemData itemdata)
    {
        _itemInfo = itemdata;
        textName.text = _itemInfo.Name;
        textGoldPrice.text = _itemInfo.GoldPrice.ToString();
        if (_itemInfo.Image != null)
        {
            itemIcon.sprite = _itemInfo.Image;
        }
        else
        {
            Debug.LogWarning("PackageIcon is null!");
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
        //EventManager<UIEvents>.StartListening<ItemData>(UIEvents.OnCreateItemSlot, SetItemInfo);
    }

    private void RemoveEvents()
    {
        //EventManager<UIEvents>.StopListening<ItemData>(UIEvents.OnCreateItemSlot, SetItemInfo);
    }

    //BuyItemUIPopUp
    public void BuyItem_Gold()
    {
        EventManager<DataEvents>.TriggerEvent<ItemData>(DataEvents.OnItemPopupDataLoad, _itemInfo);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickEnableItemBuyPopup);
    }
}