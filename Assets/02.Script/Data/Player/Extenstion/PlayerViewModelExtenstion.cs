using DataStruct;
using EnumTypes;
using EventLibrary;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class PlayerViewModelExtenstion
{
    #region Player Tickets
    public static void RegisterPlayerGameTicketsCountChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<int>(DataEvents.MVVMChangedGameTicket, vm.OnResponsePlayerGameTicketCountChangedEvent);
        else EventManager<DataEvents>.StopListening<int>(DataEvents.MVVMChangedGameTicket, vm.OnResponsePlayerGameTicketCountChangedEvent);
    }

    public static void RequestPlayerGameTicketCountChanged(this PlayerViewModel vm, int ticketCount)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedGameTicket, ticketCount);
    }

    public static void OnResponsePlayerGameTicketCountChangedEvent(this PlayerViewModel vm, int ticketCount)
    {
        vm.GameTickets = ticketCount;
    }
    #endregion

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

    #region Player Current Chapter
    public static void RegisterPlayerCurrentChapterChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<int>(DataEvents.MVVMChangedCurrentChapter, vm.OnResponsePlayerCurrentChapterChangedEvent);
        else EventManager<DataEvents>.StopListening<int>(DataEvents.MVVMChangedCurrentChapter, vm.OnResponsePlayerCurrentChapterChangedEvent);
    }

    public static void RequestPlayerCurrentChapterChanged(this PlayerViewModel vm, int chapter)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedCurrentChapter, chapter);
    }

    public static void OnResponsePlayerCurrentChapterChangedEvent(this PlayerViewModel vm, int chapter)
    {
        vm.CurrentChapter = chapter;
    }
    #endregion

    #region Player Current Stage
    public static void RegisterPlayerCurrentStageChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<int>(DataEvents.MVVMChangedCurrentStage, vm.OnResponsePlayerStageChapterChangedEvent);
        else EventManager<DataEvents>.StopListening<int>(DataEvents.MVVMChangedCurrentStage, vm.OnResponsePlayerStageChapterChangedEvent);
    }

    public static void RequestPlayerCurrentStageChanged(this PlayerViewModel vm, int stage)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedCurrentStage, stage);
    }

    public static void OnResponsePlayerStageChapterChangedEvent(this PlayerViewModel vm, int stage)
    {
        vm.CurrentStage = stage;
        // 서버 통신
    }
    #endregion

    #region Player Item Inventory
    public static void RegisterPlayerItemListChanged(this PlayerViewModel vm, bool isRegister)
    {
        if (isRegister) EventManager<DataEvents>.StartListening<ItemData, int>(DataEvents.MVVMChangedInventoryItemDictionary, vm.OnResponsePlayerItemListChangedEvent);
        else EventManager<DataEvents>.StopListening<ItemData, int>(DataEvents.MVVMChangedInventoryItemDictionary, vm.OnResponsePlayerItemListChangedEvent);
    }

    public static void RequestPlayerItemListChanged(this PlayerViewModel vm, ItemData item, int count)
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.MVVMChangedInventoryItemDictionary, item, count);
    }

    public static void OnResponsePlayerItemListChangedEvent(this PlayerViewModel vm, ItemData item, int count)
    {
        var updatedInventory = new Dictionary<ItemData, int>(vm.PlayerInventory);

        if (count <= 0)
        {
            updatedInventory.Remove(item);
        }
        else
        {
            if (updatedInventory.ContainsKey(item))
            {
                updatedInventory[item] = count;
            }
            else
            {
                updatedInventory.Add(item, count);
            }
        }

        vm.PlayerInventory = updatedInventory; // 변경된 사본을 할당하여 OnPropertyChanged를 호출합니다.
    }
    #endregion
}
