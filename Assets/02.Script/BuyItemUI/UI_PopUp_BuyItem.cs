using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopUp_BuyItem : MonoBehaviour
{
    private ItemData _data;     //구매할 아이템의 정보
    private float BuyItemCount; //구매할 아이템의 갯수

    [SerializeField] private Image ItemIcon;    //구매할 아이템의 아이콘 표시 UI
    [SerializeField] private TMP_Text Text_ItemPrice;   //구매할 아이템의 가격 표시 UI
    [SerializeField] private TMP_Text Text_BuyItemCount;    //구매할 아이템의 갯수 표시 UI

    private void Awake()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnClickChangeBuyItemCount, UpdateBuyItemText);
        EventManager<UIEvents>.StartListening<ItemData>(UIEvents.OnClickBuyItem, SetBuyItem);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickChangeBuyItemCount, UpdateBuyItemText);
        EventManager<UIEvents>.StopListening<ItemData>(UIEvents.OnClickBuyItem, SetBuyItem);
    }

    private void OnEnable()
    {
        BuyItemCount = 1f;
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickChangeBuyItemCount);
    }

    //구매할 아이템 초기화
    private void SetBuyItem(ItemData item)
    {
        _data = item;
    }

    //구매 갯수 증가
    public void Plus_BuyItemCount()
    {
        BuyItemCount = Mathf.Clamp(BuyItemCount + 1, 0, 99);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickChangeBuyItemCount);
    }

    //구매 갯수 감소
    public void Minus_BuyItemCount()
    {
        BuyItemCount = Mathf.Clamp(BuyItemCount - 1, 0, 99);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickChangeBuyItemCount);
    }

    //구매할 아이템 정보 표시 UI Update
    private void UpdateBuyItemText()
    {
        Text_ItemPrice.text = (_data.GoldPrice * BuyItemCount).ToString();
        Text_BuyItemCount.text = BuyItemCount.ToString();
    }

    //아이템 구매
    public void BuyItem_Gold()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickStart, _data, BuyItemCount);
    }
}
