using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerInventory : Singleton<PlayerInventory>
{
    public PlayerViewModel playerViewModel { get; private set; }
    private Inventory playerInventory;

    [SerializeField] private int Temp_PlayerId;

    protected new void Awake()
    {
        base.Awake();

        if (playerViewModel == null)
        {
            playerViewModel = new PlayerViewModel();
            playerViewModel.PropertyChanged += OnPropertyChanged;
            playerViewModel.RegisterPlayerGoldChanged(true);
            playerViewModel.RegisterPlayerERCChanged(true);
            playerViewModel.RegisterPlayerItemListChanged(true);
        }

        EventManager<DataEvents>.StartListening(DataEvents.OnUserInventoryLoad, SetPlayerInventory);
        EventManager<DataEvents>.StartListening<float>(DataEvents.PlayerGoldChanged, PlayerGoldChanged);
        EventManager<DataEvents>.StartListening<float>(DataEvents.PlayerERCChanged, PlayerERCChanged);
        EventManager<DataEvents>.StartListening<ItemData, int>(DataEvents.PlayerItemListChanged, PlayerItemListChanged);
    }

    private void OnDestroy()
    {
        if (playerViewModel != null)
        {
            playerViewModel.RegisterPlayerItemListChanged(false);
            playerViewModel.RegisterPlayerGoldChanged(false);
            playerViewModel.RegisterPlayerERCChanged(false);
            playerViewModel.PropertyChanged -= OnPropertyChanged;
            playerViewModel = null;
        }

        EventManager<DataEvents>.StopListening(DataEvents.OnUserInventoryLoad, SetPlayerInventory);
        EventManager<DataEvents>.StopListening<float>(DataEvents.PlayerGoldChanged, PlayerGoldChanged);
        EventManager<DataEvents>.StopListening<float>(DataEvents.PlayerERCChanged, PlayerERCChanged);
        EventManager<DataEvents>.StopListening<ItemData, int>(DataEvents.PlayerItemListChanged, PlayerItemListChanged);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(playerViewModel.PlayerERC):
                //코인 삭제

                playerInventory.ERC = playerViewModel.PlayerERC;
                break;
            case nameof(playerViewModel.PlayerGold):
                playerInventory.Gold = playerViewModel.PlayerGold;
                break;
            case nameof(playerViewModel.PlayerInventory):
                playerInventory.ItemList = playerViewModel.PlayerInventory;
                break;
        }

        // Player UI 반영
        EventManager<UIEvents>.TriggerEvent(UIEvents.GetPlayerInventoryResources,
                                                    playerViewModel.PlayerGold,
                                                    playerViewModel.PlayerERC);
        // Player Inventory Data XML로 Save
        EventManager<DataEvents>.TriggerEvent(DataEvents.OnUserInventorySave, playerInventory);
    }

    //플레이어의 인벤토리 초기화
    private void SetPlayerInventory()
    {
        var playerInvenData = DataManager.Instance.GetPlayerInventoryDatas();
        if (playerInvenData == null || !playerInvenData.ContainsKey(Temp_PlayerId)) return;

        playerInventory = playerInvenData[Temp_PlayerId];
        playerViewModel.RequestPlayerGoldChanged(playerInventory.Gold);
        playerViewModel.RequestPlayerERCChanged(playerInventory.ERC);
        foreach (var data in playerInventory.ItemList)
        {
            playerViewModel.RequestPlayerItemListChanged(data.Key, data.Value);
        }
    }

    private void PlayerGoldChanged(float gold)
    {
        playerViewModel.RequestPlayerGoldChanged(gold);
    }

    private void PlayerERCChanged(float ERC)
    {
        playerViewModel.RequestPlayerERCChanged(ERC);
    }

    private void PlayerItemListChanged(ItemData item, int count)
    {
        playerViewModel.RequestPlayerItemListChanged(item, count);
    }
}
