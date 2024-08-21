using System.ComponentModel;
using DataStruct;
using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.Serialization;

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
        }

        AddEvents();
    }

    private void OnDestroy()
    {
        if (PlayerViewModel != null)
        {
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
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening(DataEvents.OnUserInformationLoad, SetPlayerData);
        EventManager<DataEvents>.StopListening<int>(DataEvents.PlayerTicketCountChanged, PlayerTicketCountChanged);
        EventManager<DataEvents>.StopListening<float>(DataEvents.PlayerGoldChanged, PlayerGoldChanged);
        EventManager<DataEvents>.StopListening<float>(DataEvents.PlayerERCChanged, PlayerERCChanged);
        EventManager<DataEvents>.StopListening<ItemData, int>(DataEvents.PlayerItemListChanged, PlayerItemListChanged);
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
        }

        // Player UI 반영
        EventManager<UIEvents>.TriggerEvent(UIEvents.GetPlayerInventoryResources, 
            PlayerViewModel.GameTickets, PlayerViewModel.PlayerGold, PlayerViewModel.PlayerERC);
        
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
    }

    // 플레이어 자원 데이터 초기화
    private void SetPlayerInventory(PlayerInfo _playerInventory)
    {
        PlayerViewModel.RequestPlayerGameTicketCountChanged(_playerInventory.TicketCount);
        PlayerViewModel.RequestPlayerGoldChanged(_playerInventory.Gold);
        PlayerViewModel.RequestPlayerERCChanged(_playerInventory.ERC);

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
}
