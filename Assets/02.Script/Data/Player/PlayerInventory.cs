using System.ComponentModel;
using DataStruct;
using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInventory : Singleton<PlayerInventory>
{
    public PlayerViewModel PlayerViewModel { get; private set; }

    [SerializeField] private int tempPlayerId;
    
    private Inventory _playerInventory;

    protected new void Awake()
    {
        base.Awake();

        if (PlayerViewModel == null)
        {
            PlayerViewModel = new PlayerViewModel();
            PlayerViewModel.PropertyChanged += OnPropertyChanged;
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
            PlayerViewModel.PropertyChanged -= OnPropertyChanged;
            PlayerViewModel = null;
        }

        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<DataEvents>.StartListening(DataEvents.OnUserInventoryLoad, SetPlayerInventory);
        EventManager<DataEvents>.StartListening<float>(DataEvents.PlayerGoldChanged, PlayerGoldChanged);
        EventManager<DataEvents>.StartListening<float>(DataEvents.PlayerERCChanged, PlayerERCChanged);
        EventManager<DataEvents>.StartListening<ItemData, int>(DataEvents.PlayerItemListChanged, PlayerItemListChanged);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening(DataEvents.OnUserInventoryLoad, SetPlayerInventory);
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
                _playerInventory.ERC = PlayerViewModel.PlayerERC;
                break;
            case nameof(PlayerViewModel.PlayerGold):
                _playerInventory.Gold = PlayerViewModel.PlayerGold;
                break;
            case nameof(PlayerViewModel.PlayerInventory):
                _playerInventory.ItemList = PlayerViewModel.PlayerInventory;
                break;
        }

        // Player UI 반영
        EventManager<UIEvents>.TriggerEvent(UIEvents.GetPlayerInventoryResources, 
            PlayerViewModel.PlayerGold, PlayerViewModel.PlayerERC);
        
        // Player Inventory Data XML로 Save
        EventManager<DataEvents>.TriggerEvent(DataEvents.OnUserInventorySave, _playerInventory);
    }

    //플레이어의 인벤토리 초기화
    private void SetPlayerInventory()
    {
        var playerInvenData = DataManager.Instance.GetPlayerInventoryDatas();
        if (playerInvenData == null || !playerInvenData.ContainsKey(tempPlayerId)) return;

        _playerInventory = playerInvenData[tempPlayerId];
        PlayerViewModel.RequestPlayerGoldChanged(_playerInventory.Gold);
        PlayerViewModel.RequestPlayerERCChanged(_playerInventory.ERC);
        foreach (var data in _playerInventory.ItemList)
        {
            PlayerViewModel.RequestPlayerItemListChanged(data.Key, data.Value);
        }
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
