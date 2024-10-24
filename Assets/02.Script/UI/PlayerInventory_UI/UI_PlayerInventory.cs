using System.Collections;
using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UI_PlayerInventory : Singleton<UI_PlayerInventory>
{
    [FoldoutGroup("Player Ticket UI")][SerializeField] private TMP_Text gameTickets;
    [FoldoutGroup("Player Ticket UI")][SerializeField] private TMP_Text timer;

    [FoldoutGroup("Player Gold UI")] [SerializeField] private TMP_Text goldPrice; // 골드 UI 표시
    [FoldoutGroup("Player Gold UI")] [SerializeField] private TMP_Text ercPrice; // ERC UI 표시
    [FoldoutGroup("Payment UI")] [SerializeField] private GameObject paymentPopup;

    public Canvas Canvas { get; private set; }
    private GameObject _parent;

    private int _ticketCount; // Player가 가지고 있는 티겟
    private float _goldValue; // Player가 가지고 있는 Gold의 갯수
    private float _ercValue; // Player가 가지고 있는 ERC의 갯수

    // 임의로 지정한 티켓의 최대 갯수
    private const int TicketMaxCount = 99;
    private const int MaxTimer = 300;

    private int ticketTimer = MaxTimer;

    public Dictionary<ItemData, int> _itemInventory { get; private set; }

    protected new void Awake()
    {
        base.Awake();

        Canvas = GetComponentInParent<Canvas>();
        _parent = gameObject.transform.parent.gameObject;

        AddEvents();
    }
    private void Start()
    {
        StartCoroutine(StartRechargeTicket());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<UIEvents>.StartListening<int, float, float>(UIEvents.GetPlayerInventoryResources, ReadPlayerCapital);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickUseTicket, UseTicket);
        EventManager<UIEvents>.StartListening<ItemData, float>(UIEvents.OnClickItemBuyButton, BuyItem_Gold);
        EventManager<UIEvents>.StartListening<GoldPackageData>(UIEvents.OnClickGoldBuyButton, BuyItem_ERC);
        EventManager<GoldEvent>.StartListening<float>(GoldEvent.OnGetGold, GetGold);
        EventManager<InventoryItemEvent>.StartListening<Dictionary<ItemData, int>>(InventoryItemEvent.GetInventoryItemList, GetPlayerItemInventory);
        EventManager<InventoryItemEvent>.StartListening<string>(InventoryItemEvent.UseItem, UseItem);
        EventManager<InventoryItemEvent>.StartListening<string>(InventoryItemEvent.AddItem, AddItem);
        EventManager<StageEvent>.StartListening(StageEvent.SetPlayerItemInventoryList, SetGamePlayerItem);
        EventManager<InventoryItemEvent>.StartListening<string>(InventoryItemEvent.DecreaseItemCount, DecreaseItemCount);
        EventManager<InventoryItemEvent>.StartListening(InventoryItemEvent.RecoveryTicketCountAfterGameClear, RechargeGameTicket);
    }

    private void RemoveEvents()
    {
        EventManager<UIEvents>.StopListening<int, float, float>(UIEvents.GetPlayerInventoryResources, ReadPlayerCapital);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickUseTicket, UseTicket);
        EventManager<UIEvents>.StopListening<ItemData, float>(UIEvents.OnClickItemBuyButton, BuyItem_Gold);
        EventManager<UIEvents>.StartListening<GoldPackageData>(UIEvents.OnClickGoldBuyButton, BuyItem_ERC);
        EventManager<GoldEvent>.StopListening<float>(GoldEvent.OnGetGold, GetGold);
        EventManager<InventoryItemEvent>.StopListening<Dictionary<ItemData, int>>(InventoryItemEvent.GetInventoryItemList, GetPlayerItemInventory);
        EventManager<InventoryItemEvent>.StopListening<string>(InventoryItemEvent.UseItem, UseItem);
        EventManager<InventoryItemEvent>.StopListening<string>(InventoryItemEvent.AddItem, AddItem);
        EventManager<StageEvent>.StopListening(StageEvent.SetPlayerItemInventoryList, SetGamePlayerItem);
        EventManager<InventoryItemEvent>.StopListening<string>(InventoryItemEvent.DecreaseItemCount, DecreaseItemCount);
        EventManager<InventoryItemEvent>.StopListening(InventoryItemEvent.RecoveryTicketCountAfterGameClear, RechargeGameTicket);
    }

    //Gold와 ERC 초기화
    private void ReadPlayerCapital(int ticketCount,float gold, float erc)
    {
        //디버그용
        _ticketCount = ticketCount;
        _goldValue = gold;
        _ercValue = erc;

        UpdateUIText();
    }

    // Player 자원 UI 업데이트
    private void UpdateUIText()
    {
        goldPrice.text = UpdateNumberMeasured(_goldValue);
        ercPrice.text = UpdateNumberMeasured(_ercValue);
        gameTickets.text = UpdateNumberMeasured((float)_ticketCount);
    }

    // 숫자 단위 도입
    private string UpdateNumberMeasured(float Value)
    {
        if (Value < 1000)
        {
            return Value.ToString(); // 1000 미만의 값은 단위 없이 그대로 반환
        }

        string[] units = { "", "K", "M", "B", "T", "P", "E" }; // Kilo, Mega, Billion, Trillion, Peta, Exa 등
        int unitIndex = 0;
        double scaledValue = Value;

        while (scaledValue >= 1000 && unitIndex < units.Length - 1)
        {
            scaledValue /= 1000;
            unitIndex++;
        }

        return scaledValue.ToString("0.##") + units[unitIndex];
    }

    // 입장 티겟 구매
    private void UseTicket()
    {
        if(_ticketCount <= 0)
        {
            EventManager<DataEvents>.TriggerEvent(DataEvents.OnPaymentSuccessful, false);
            return;
        }

        _ticketCount = Mathf.Clamp(_ticketCount - 1, 0, TicketMaxCount);
        // Player Inventory View Model에 반영
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerTicketCountChanged, _ticketCount);

        UpdateUIText();
    }

    private IEnumerator StartRechargeTicket()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(1f);

            if (_ticketCount >= TicketMaxCount) 
            {
                timer.text = "0:00";
                continue;
            }

            ticketTimer = Mathf.Clamp(ticketTimer - 1, 0, MaxTimer);
            if(ticketTimer <= 0)
            {
                RechargeGameTicket();
                gameTickets.text = _ticketCount.ToString();
                ticketTimer = MaxTimer;
            }

            // 분:초 형식으로 변환하여 표시
            int minutes = ticketTimer / 60;
            int seconds = ticketTimer % 60;
            timer.text = string.Format("{0}:{1:D2}", minutes, seconds);
        }       
    } 

    // 게임 티켓 충전
    private void RechargeGameTicket()
    {
        _ticketCount = Mathf.Clamp(_ticketCount+1, 0, TicketMaxCount);
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerTicketCountChanged, _ticketCount);

        // AWS의 MySql과 통신
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

            // 소지 아이템 리스트 변경
            _itemInventory[itemInfo] += (int)Count;

            ItemData newItemData = new ItemData();
            newItemData.ItemID = itemInfo.ItemID;

            // Player Inventory View Model에 반영
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerGoldChanged, _goldValue);

            // Player Informaiton에 데이터 전달
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerItemListChanged, newItemData, _itemInventory[itemInfo]);

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

    private void TogglePlayerCurrencyUI()
    {
        bool isActive = _parent.activeSelf;
        
        _parent.SetActive(!isActive);
    }

    private void GetPlayerItemInventory(Dictionary<ItemData, int> itemList)
    {
        _itemInventory = itemList;
    }

    private void UseItem(string itemID)
    {
        // 아이템 사용
        ExecuteItemEffect(itemID);
    }

    private void AddItem(string itemID)
    {
        IncreaseItemCount(itemID);
    }

    // 아이템 감소
    private void DecreaseItemCount(string itemID)
    {
        ItemData useItem = default;

        foreach (var item in _itemInventory)
        {
            if (item.Key.ItemID == itemID)
            {
                useItem = item.Key;
                break;
            }
        }

        // 소유 아이템 개수 감소
        if (useItem.ItemID != default)
        {
            _itemInventory[useItem] -= 1;
        }
        else
        {
            DebugLogger.LogWarning("아이템을 소지하고 있지 않습니다.");
            return;
        }

        ItemData newItemData = new ItemData();
        newItemData.ItemID = useItem.ItemID;

        // Player Information에 데이터 전달
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerItemListChanged, newItemData, _itemInventory[useItem]);

        
        // 아이템 개수가 0이하가 되면 리스트에서 제거하는 것 같은데 제거하면 아이템 추가시 예외 처리를 해야함
        // 그래서 주석 처리
        /*
        if (_itemInventory[useItem] <= 0)
        {
            _itemInventory.Remove(useItem);
        }
        */
        
    }

    private void IncreaseItemCount(string itemID)
    {
        ItemData useItem = default;

        foreach (var item in _itemInventory)
        {
            if (item.Key.ItemID == itemID)
            {
                useItem = item.Key;
                break;
            }
        }

        // 소유 아이템 개수 증가
        if (useItem.ItemID != default)
        {
            _itemInventory[useItem]++;
        }

        ItemData newItemData = new ItemData();
        newItemData.ItemID = useItem.ItemID;

        // Player Information에 데이터 전달
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerItemListChanged, newItemData, _itemInventory[useItem]);
    }

    private void ExecuteItemEffect(string itemID)
    {
        switch (itemID)
        {
            case nameof(ItemID.I1001):
                // limitCount 증가
                DecreaseItemCount(itemID);
                EventManager<StageEvent>.TriggerEvent(StageEvent.RecoveryLimitCount);
                break;
            case nameof(ItemID.I1002):
                EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetHint, false);
                EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, true);
                break;
            case nameof(ItemID.I1003):
                EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, false);
                EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetHint, true);
                break;
        }
    }

    private void SetGamePlayerItem()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.LoadInventoryItem, _itemInventory);
    }
}