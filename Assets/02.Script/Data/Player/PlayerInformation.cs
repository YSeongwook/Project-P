using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DataStruct;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class PlayerInformation : Singleton<PlayerInformation>
{
    public PlayerViewModel PlayerViewModel { get; private set; }

    [SerializeField] private string tempPlayerId;
    
    private PlayerInfo _playerInfo;
    //private string PlayerCurrentChapter { get { return _playerInfo.CurrentChapter; } }
    //private string PlayerCurrentStage { get { return _playerInfo.CurrentStage; } }

    protected new void Awake()
    {
        base.Awake();

        if (PlayerViewModel == null)
        {
            PlayerViewModel = new PlayerViewModel();
            PlayerViewModel.PropertyChanged += OnPropertyChanged;
            PlayerViewModel.RegisterPlayerGameTicketsCountChanged(true);
            PlayerViewModel.RegisterPlayerGoldChanged(true);
            PlayerViewModel.RegisterPlayerERCChanged(true);
            PlayerViewModel.RegisterPlayerItemListChanged(true);
            PlayerViewModel.RegisterPlayerCurrentChapterChanged(true);
            PlayerViewModel.RegisterPlayerCurrentStageChanged(true);
        }

        AddEvents();
    }

    private void OnDestroy()
    {
        if (PlayerViewModel != null)
        {
            PlayerViewModel.RegisterPlayerCurrentStageChanged(false);
            PlayerViewModel.RegisterPlayerCurrentChapterChanged(false);
            PlayerViewModel.RegisterPlayerItemListChanged(false);
            PlayerViewModel.RegisterPlayerGoldChanged(false);
            PlayerViewModel.RegisterPlayerERCChanged(false);
            PlayerViewModel.RegisterPlayerGameTicketsCountChanged(false);
            PlayerViewModel.PropertyChanged -= OnPropertyChanged;
            PlayerViewModel = null;
        }

        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<DataEvents>.StartListening(DataEvents.OnUserInformationLoad, SetPlayerData);
        EventManager<DataEvents>.StartListening<int>(DataEvents.PlayerTicketCountChanged, PlayerTicketCountChanged);
        EventManager<DataEvents>.StartListening<float>(DataEvents.PlayerGoldChanged, PlayerGoldChanged);
        EventManager<DataEvents>.StartListening<float>(DataEvents.PlayerERCChanged, PlayerERCChanged);
        EventManager<DataEvents>.StartListening<ItemData, int>(DataEvents.PlayerItemListChanged, PlayerItemListChanged);
        EventManager<DataEvents>.StartListening<int>(DataEvents.PlayerCurrentChapterChanged, PlayerCurrentChapterChanged);
        EventManager<DataEvents>.StartListening<int>(DataEvents.PlayerCurrentStageChanged, PlayerCurrentStageChanged);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening(DataEvents.OnUserInformationLoad, SetPlayerData);
        EventManager<DataEvents>.StopListening<int>(DataEvents.PlayerTicketCountChanged, PlayerTicketCountChanged);
        EventManager<DataEvents>.StopListening<float>(DataEvents.PlayerGoldChanged, PlayerGoldChanged);
        EventManager<DataEvents>.StopListening<float>(DataEvents.PlayerERCChanged, PlayerERCChanged);
        EventManager<DataEvents>.StopListening<ItemData, int>(DataEvents.PlayerItemListChanged, PlayerItemListChanged);
        EventManager<DataEvents>.StopListening<int>(DataEvents.PlayerCurrentChapterChanged, PlayerCurrentChapterChanged);
        EventManager<DataEvents>.StopListening<int>(DataEvents.PlayerCurrentStageChanged, PlayerCurrentStageChanged);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(PlayerViewModel.PlayerERC):
                //코인 삭제
                _playerInfo.ERC = PlayerViewModel.PlayerERC;
                break;
            case nameof(PlayerViewModel.PlayerGold):
                _playerInfo.Gold = PlayerViewModel.PlayerGold;
                break;
            case nameof(PlayerViewModel.PlayerInventory):
                _playerInfo.ItemList = PlayerViewModel.PlayerInventory;
                break;
            case nameof(PlayerViewModel.CurrentChapter):
                _playerInfo.CurrentChapter = PlayerViewModel.CurrentChapter.ToString();
                break;
            case nameof(PlayerViewModel.CurrentStage):
                _playerInfo.CurrentStage = PlayerViewModel.CurrentStage.ToString();
                break;
        }

        // Player UI 반영
        EventManager<UIEvents>.TriggerEvent(UIEvents.GetPlayerInventoryResources, 
            PlayerViewModel.GameTickets, PlayerViewModel.PlayerGold, PlayerViewModel.PlayerERC);

        // DB에 업데이트할 데이터
        UpdateDBData();

        PlayerDataSave();
    }

    public void UpdateDBData()
    {
        // StringBuilder 객체 생성
        StringBuilder sb = new StringBuilder();

        // 현재 UTC 시간을 가져옴
        DateTime utcNow = DateTime.UtcNow;

        // 한국 시간대 (UTC+9)로 변환
        TimeZoneInfo kstZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
        DateTime kstTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, kstZone);

        // 하트 갯수
        int Heart = 0;
        // Item1
        int Item1 = 0;
        // Item2
        int Item2 = 0;

        // 문자열 추가
        sb.Append("Assets");
        sb.Append("||");
        //sb.Append(DBDataManager.Instance.UserData["MemberID"]);
        sb.Append("1");
        sb.Append("||");
        //sb.Append(PlayerViewModel.PlayerGold.ToString());
        sb.Append("0");
        sb.Append("||");
        sb.Append(kstTime.ToString("HH:mm:ss"));    // HeartTime
        sb.Append("||");
        sb.Append($"{Heart}/{Item1}/{Item2}");    // ItemCount

        MySQLManager.Instance.UpdateDB(sb.ToString());
    }


    private void PlayerDataSave()
    {
        // Player Inventory Data XML로 Save
        EventManager<DataEvents>.TriggerEvent(DataEvents.OnUserInventorySave, _playerInfo);
    }

    //플레이어의 데이터 초기화
    private void SetPlayerData()
    {
        var playerInvenData = DataManager.Instance.GetPlayerInventoryDatas();
        if (playerInvenData == null || !playerInvenData.ContainsKey(tempPlayerId)) return;

        _playerInfo = playerInvenData[tempPlayerId];
        SetPlayerInventory(_playerInfo);

        EventManager<UIEvents>.TriggerEvent(UIEvents.OnEnableChapterMoveButton, _playerInfo.CurrentChapter);

        var playerItemList = new Dictionary<ItemData, int>();
        foreach(var item in _playerInfo.ItemList)
        {
            var itemData = DataManager.Instance.GetItemData(item.Key.ItemID);
            playerItemList.Add(itemData, item.Value);
        }

        // 인벤토리에 아이템 리스트 전달
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.GetInventoryItemList, playerItemList);
    }

    // 플레이어 자원 데이터 초기화
    private void SetPlayerInventory(PlayerInfo _playerInventory)
    {
        PlayerViewModel.RequestPlayerGameTicketCountChanged(_playerInventory.TicketCount);
        PlayerViewModel.RequestPlayerGoldChanged(_playerInventory.Gold);
        PlayerViewModel.RequestPlayerERCChanged(_playerInventory.ERC);
        PlayerViewModel.RequestPlayerCurrentChapterChanged(int.Parse(_playerInventory.CurrentChapter));
        PlayerViewModel.RequestPlayerCurrentStageChanged(int.Parse(_playerInventory.CurrentStage));

        foreach (var data in _playerInventory.ItemList)
        {
            PlayerViewModel.RequestPlayerItemListChanged(data.Key, data.Value);
        }
    }

    private void PlayerTicketCountChanged(int ticketCount)
    {
        PlayerViewModel.RequestPlayerGameTicketCountChanged(ticketCount);
    }

    private void PlayerGoldChanged(float gold)
    {
        PlayerViewModel.RequestPlayerGoldChanged(gold);
    }

    private void PlayerERCChanged(float erc)
    {
        PlayerViewModel.RequestPlayerERCChanged(erc);
    }

    private void PlayerItemListChanged(ItemData item, int count)
    {
        PlayerViewModel.RequestPlayerItemListChanged(item, count);
    }

    private void PlayerCurrentChapterChanged(int chapter)
    {
        PlayerViewModel.RequestPlayerCurrentChapterChanged(chapter);
    }

    private void PlayerCurrentStageChanged(int stage)
    {
        PlayerViewModel.RequestPlayerCurrentStageChanged(stage);
    }

    public int GetPlayerCurrentChapter()
    {
        return int.Parse(_playerInfo.CurrentChapter);
    }

    public int GetPlayerCurrentStage()
    {
        return int.Parse(_playerInfo.CurrentStage);
    }


}