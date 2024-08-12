using EnumTypes;
using DataStruct;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGold : Singleton<PlayerGold>
{
    [FoldoutGroup("Player Gold UI")][SerializeField] private TMP_Text GoldPrice;    // 골드 UI 표시
    [FoldoutGroup("Player Gold UI")][SerializeField] private TMP_Text ERCPrice;     // ERC UI 표시

    [FoldoutGroup("Payment UI")][SerializeField] private GameObject PaymentPopup;

    public Canvas canvas { get; private set; }

    private float GoldValue;    // Player가 가지고 있는 Gold의 갯수
    private float ERCValue;     // Player가 가지고 있는 ERC의 갯수

    protected new void Awake()
    {
        base.Awake();

        canvas = GetComponentInParent<Canvas>();

        ReadPlayerCapital();

        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<UIEvents>.StartListening<ItemData, float>(UIEvents.OnClickItemBuyButton, BuyItem_Gold);
        EventManager<UIEvents>.StartListening<GoldPackageData>(UIEvents.OnClickGoldBuyButton, BuyItem_ERC);
        EventManager<GoldEvent>.StartListening<float>(GoldEvent.OnGetGold, GetGold);
    }

    private void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening<ItemData, float>(UIEvents.OnClickItemBuyButton, BuyItem_Gold);
        EventManager<UIEvents>.StartListening<GoldPackageData>(UIEvents.OnClickGoldBuyButton, BuyItem_ERC);
        EventManager<GoldEvent>.StopListening<float>(GoldEvent.OnGetGold, GetGold);
    }

    //DataManager에서 Data를 읽어오기
    private void ReadPlayerCapital()
    {
        //디버그용
        GoldValue = 10000;
        ERCValue = 1000;

        UpdateUIText();
    }

    // Player 자원 UI 업데이트
    private void UpdateUIText()
    {
        GoldPrice.text = GoldValue.ToString();
        ERCPrice.text = ERCValue.ToString();
    }

    // 아이템 구매 - 골드 (기본)
    private void BuyItem_Gold(ItemData itemInfo, float Count)
    {
        if (GoldValue < itemInfo.GoldPrice * Count)
        {
            //BuyItem_ERC(itemInfo, Count);
            //EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickGoldBuyButton, itemInfo, Count);

            // 팝업 창 등장
            EventManager<UIEvents>.TriggerEvent(UIEvents.GoldStorePopup);
        }
        else
        {
            GoldValue -= itemInfo.GoldPrice * Count;
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, true);
        }

        UpdateUIText();
    }

    //아이템 구매 - ERC (Gold가 구매 가격보다 적으면)
    private void BuyItem_ERC(GoldPackageData itemInfo)
    {
        if (ERCValue < itemInfo.ERCPrice)
        {
            DebugLogger.Log("금액이 부족합니다.");

            //금액 부족 Popup On
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, false);

            return;
        }
        else
        {
            ERCValue -= itemInfo.ERCPrice;

            //계산 완료 PopUp On 
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, true);
        }


        UpdateUIText();
    }

    private void GetGold(float getGold)
    {
        GoldValue += getGold;

        UpdateUIText();
    }
}
