using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UI_PlayerInventory : Singleton<UI_PlayerInventory>
{ 
    [FoldoutGroup("Player Gold UI")] [SerializeField] private TMP_Text goldPrice; // 골드 UI 표시
    [FoldoutGroup("Player Gold UI")] [SerializeField] private TMP_Text ercPrice; // ERC UI 표시
    [FoldoutGroup("Payment UI")] [SerializeField] private GameObject paymentPopup;

    public Canvas Canvas { get; private set; }

    private float _goldValue; // Player가 가지고 있는 Gold의 갯수
    private float _ercValue; // Player가 가지고 있는 ERC의 갯수

    protected new void Awake()
    {
        base.Awake();

        Canvas = GetComponentInParent<Canvas>();

        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<UIEvents>.StartListening<float, float>(UIEvents.GetPlayerInventoryResources, ReadPlayerCapital);
        EventManager<UIEvents>.StartListening<ItemData, float>(UIEvents.OnClickItemBuyButton, BuyItem_Gold);
        EventManager<UIEvents>.StartListening<GoldPackageData>(UIEvents.OnClickGoldBuyButton, BuyItem_ERC);
        EventManager<GoldEvent>.StartListening<float>(GoldEvent.OnGetGold, GetGold);
    }

    private void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening<float, float>(UIEvents.GetPlayerInventoryResources, ReadPlayerCapital);
        EventManager<UIEvents>.StopListening<ItemData, float>(UIEvents.OnClickItemBuyButton, BuyItem_Gold);
        EventManager<UIEvents>.StartListening<GoldPackageData>(UIEvents.OnClickGoldBuyButton, BuyItem_ERC);
        EventManager<GoldEvent>.StopListening<float>(GoldEvent.OnGetGold, GetGold);
    }

    //Gold와 ERC 초기화
    private void ReadPlayerCapital(float gold, float erc)
    {
        //디버그용
        _goldValue = gold;
        _ercValue = erc;

        UpdateUIText();
    }

    // Player 자원 UI 업데이트
    private void UpdateUIText()
    {
        goldPrice.text = _goldValue.ToString();
        ercPrice.text = _ercValue.ToString();
    }

    // 아이템 구매 - 골드 (기본)
    private void BuyItem_Gold(ItemData itemInfo, float Count)
    {
        if (_goldValue < itemInfo.GoldPrice * Count)
        {
            // 팝업 창 등장
            EventManager<UIEvents>.TriggerEvent(UIEvents.GoldStorePopup);
        }
        else
        {
            _goldValue -= itemInfo.GoldPrice * Count;
            // Player Inventory View Model에 반영
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerGoldChanged, _goldValue);
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerItemListChanged, itemInfo, Count);

            // 구매 완료 Message 출력
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, true);
        }

        UpdateUIText();
    }

    //아이템 구매 - ERC (Gold가 구매 가격보다 적으면)
    private void BuyItem_ERC(GoldPackageData itemInfo)
    {
        if (_ercValue < itemInfo.ERCPrice)
        {
            DebugLogger.Log("금액이 부족합니다.");

            //금액 부족 Popup On
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, false);

            return;
        }
        else
        {
            _ercValue -= itemInfo.ERCPrice;
            // Player Inventory View Model에 반영
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerERCChanged, _ercValue);

            //계산 완료 PopUp On 
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, true);
        }


        UpdateUIText();
    }

    //골드 획득
    private void GetGold(float getGold)
    {
        _goldValue += getGold;

        UpdateUIText();
    }
}