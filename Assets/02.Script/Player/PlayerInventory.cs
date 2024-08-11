using DataStruct;
using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private PlayerViewModel playerViewModel;
    private Inventory playerInventory;

    [SerializeField] private int Temp_PlayerId;

    private void Awake()
    {
        if(playerViewModel == null)
        {
            playerViewModel = new PlayerViewModel();
            playerViewModel.PropertyChanged += OnPropertyChanged;
            playerViewModel.RegisterPlayerGoldChanged(true);
            playerViewModel.RegisterPlayerERCChanged(true);
            playerViewModel.RegisterPlayerItemListChanged(true);
        }

        EventManager<DataEvents>.StartListening(DataEvents.OnUserInventoryLoad, SetPlayerInventory);
    }

    private void OnDestroy()
    {
        if(playerViewModel != null)
        {
            playerViewModel.RegisterPlayerItemListChanged(false);
            playerViewModel.RegisterPlayerGoldChanged(false);
            playerViewModel.RegisterPlayerERCChanged(false);
            playerViewModel.PropertyChanged -= OnPropertyChanged;
            playerViewModel = null;
        }

        EventManager<DataEvents>.StopListening(DataEvents.OnUserInventoryLoad, SetPlayerInventory);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {

    }

    //플레이어의 인벤토리 초기화
    private void SetPlayerInventory()
    {
        var playerInvenData = DataManager.Instance.GetPlayerInventoryDatas();
        if (playerInvenData == null || !playerInvenData.ContainsKey(Temp_PlayerId)) return;

        playerInventory = playerInvenData[Temp_PlayerId];
        playerViewModel.RequestPlayerGoldChanged(playerInventory.Gold);
        playerViewModel.RequestPlayerERCChanged(playerInventory.ERC);
        foreach(var data in playerInventory.ItemList)
        {
            playerViewModel.RequestPlayerItemListChanged(data.Key, data.Value);
        }
    }
}
