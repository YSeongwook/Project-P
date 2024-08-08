using EnumTypes;
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

    private float GoldValue;    // Player가 가지고 있는 Gold의 갯수
    private float ERCValue;     // Player가 가지고 있는 ERC의 갯수

    protected new void Awake()
    {
        base.Awake();

        ReadPlayerCapital();

        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<UIEvents>.StartListening<ItemData, float>(UIEvents.OnClickStart, BuyItem_Gold);
    }

    private void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening<ItemData, float>(UIEvents.OnClickStart, BuyItem_Gold);
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
    public void BuyItem_Gold(ItemData itemInfo, float Count)
    {
        if (GoldValue < itemInfo.GoldPrice * Count)
        {
            BuyItem_ERC(itemInfo, Count);
        }
        else
            GoldValue -= itemInfo.GoldPrice * Count;

        UpdateUIText();
    }

    //아이템 구매 - ERC (Gold가 구매 가격보다 적으면)
    private void BuyItem_ERC(ItemData itemInfo, float Count)
    {
        if (ERCValue < itemInfo.ERCPrice * Count)
        {
            DebugLogger.Log("금액이 부족합니다.");
            return;
        }
        else
            ERCValue -= itemInfo.ERCPrice * Count;
    }
}
