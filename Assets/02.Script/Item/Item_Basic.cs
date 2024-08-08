using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//디버그용
public enum ItemID
{
    I1001,
    I1002, I1003, I1004,
    I1005,
}

public enum PaymentMethod
{
    Gold,
    ERC
}

public class Item_Basic : MonoBehaviour
{
    //[SerializeField] private ItemID ItemID;
    //디버그 용
    [SerializeField] ItemID TempItemID;
    [SerializeField] string TempName;
    [SerializeField] float TempItemGoldPrice;
    [SerializeField] float TempItemERCPrice;

    [FoldoutGroup("Shop UI")][SerializeField] TMP_Text Text_Name;        // 상점 아이템 이름 UI 표시
    [FoldoutGroup("Shop UI")][SerializeField] TMP_Text Text_GoldPrice;   // 상점 아이템 골드 가격 UI 표시

    private ItemData ItemInfo;      // 아이템의 정보

    //아이템 정보 설정
    private void SetItemInfo()
    {
        //디버그용
        ItemInfo = new ItemData();

        ItemInfo.Name = TempName;
        ItemInfo.GoldPrice = TempItemGoldPrice;
        ItemInfo.ERCPrice = TempItemERCPrice;

        Text_Name.text = TempName;
        Text_GoldPrice.text = TempItemGoldPrice.ToString();
        //Text_ERCPrice.text = TempItemERCPrice.ToString();

        //Debug.Log(ItemID.I1001.ToString() == "I1001");
    }

    private void Awake()
    {
        SetItemInfo();
    }

    private void OnDestroy()
    {
        
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
        EventManager<UIEvents>.TriggerEvent<ItemData>(UIEvents.OnClickBuyItem, ItemInfo);
    }

    //public void BuyItem_ERC()
    //{
    //    EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickStart, ItemInfo, PaymentMethod.ERC);
    //}
}
