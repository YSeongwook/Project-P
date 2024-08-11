using DataStruct;
using EnumTypes;
using EventLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerViewModelExtenstion
{
    #region Player Gold
    public static void RegisterPlayerGoldChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<float>(DataEvents.MVVMChangedGold, vm.OnResponsePlayerGoldChangedEvent);
        else EventManager<DataEvents>.StopListening<float>(DataEvents.MVVMChangedGold, vm.OnResponsePlayerGoldChangedEvent);
    }

    public static void RequestPlayerGoldChanged(this PlayerViewModel vm, float gold)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedGold, gold);
    }

    public static void OnResponsePlayerGoldChangedEvent(this PlayerViewModel vm, float gold)
    {
        vm.PlayerGold = gold;
    }
    #endregion
    #region Player ERC
    public static void RegisterPlayerERCChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<float>(DataEvents.MVVMChangedERC, vm.OnResponsePlayerERCChangedEvent);
        else EventManager<DataEvents>.StopListening<float>(DataEvents.MVVMChangedERC, vm.OnResponsePlayerERCChangedEvent);
    }

    public static void RequestPlayerERCChanged(this PlayerViewModel vm, float ERC)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedERC, ERC);
    }

    public static void OnResponsePlayerERCChangedEvent(this PlayerViewModel vm, float ERC)
    {
        vm.PlayerERC = ERC;
    }
    #endregion
    #region Player Item Inventory
    public static void RegisterPlayerItemListChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<ItemData, int>(DataEvents.MVVMChangedERC, vm.OnResponsePlayerItemListChangedEvent);
        else EventManager<DataEvents>.StopListening<ItemData, int>(DataEvents.MVVMChangedERC, vm.OnResponsePlayerItemListChangedEvent);
    }

    public static void RequestPlayerItemListChanged(this PlayerViewModel vm, ItemData item, int count)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedERC, item, count);
    }

    public static void OnResponsePlayerItemListChangedEvent(this PlayerViewModel vm, ItemData item, int count)
    {
        if(vm.PlayerInventory.ContainsKey(item))
        {
            vm.PlayerInventory[item] = count;
            if (vm.PlayerInventory[item] <= 0) vm.PlayerInventory.Remove(item);
        }
        else
        {
            vm.PlayerInventory.Add(item, count);
        }
    }
    #endregion
}
