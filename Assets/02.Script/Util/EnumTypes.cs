using UnityEngine;

namespace EnumTypes
{
    public enum UIEvents
    {
        OnClickStart,
        OnClickUseTicket,
        OnClickItemBuyButton,
        OnClickGoldBuyButton,
        OnClickChangeBuyItemCount,
        OnClickEnableItemBuyPopup,
        OnClickEnableGoldBuyPopup,
        OnClickEnableGoldStore,
        OnClickGoldBuyExit,
        OnCreateItemSlot,
        OnCreateGoldPackageSlot,
        GetPlayerInventoryResources,
        GoldStorePopup,
        GoldStoreExit,
        OnClickGameStage,
        OnEnableChapterMoveButton,
        CreateStageButton,
        ChangeScrollViewCenter,
        MissionSuccessPopUp,
        GameMessagePopUp,
    }

    public enum MiniGame
    {
        Catch,
        PoliceGameOver,
    }

    public enum DataEvents
    {
        OnUserInformationLoad,
        OnUserInventorySave,
        OnItemDataLoad,
        OnPaymentSuccessful,
        MVVMChangedGameTicket,
        MVVMChangedGold,
        MVVMChangedERC,
        MVVMChangedInventoryItemDictionary,
        MVVMChangedCurrentChapter,
        MVVMChangedCurrentStage,
        PlayerTicketCountChanged,
        PlayerGoldChanged,
        PlayerERCChanged,
        PlayerItemListChanged,
        PlayerCurrentChapterChanged,
        PlayerCurrentStageChanged,
        UpdateCurrentChapterAndStage,
        PlayerOpenStageInfo,
        LoadThisChapterTileList,
        ResetChapterTileList,
        SelectStage,
        CheckAnswer,
        SetTileGrid,
        DecreaseLimitCount,
        SavePlayerData,
    }

    public enum GoldEvent
    {
        OnGetGold,
        OnUseERC,
        OnGetERC
    }

    public enum StageEvent
    {
        EnterStage,
        StartStage,
        StageClear,
        StageFail,
        ResetTileGrid,
        SetPathTileGridAdd,
        SetPathEndPoint,
        SortPathTileGrid,
        UseTurn,
        MissionSuccess,
        CheckMissionFail,
        ReturnSelectStage,
        RestartStage,
        RecoveryLimitCount,
        LoadInventoryItem,
        SetPlayerItemInventoryList
    }
    
    public enum PuzzleEvent
    {
        StartClearAnimation,
        Rotation,
    }

    public enum GimmickEvent
    {
        GetGimmickShape,
    }

    public enum InventoryItemEvent
    {
        GetInventoryItemList,
        UseItem,
    }

    public class EnumTypes : MonoBehaviour
    {
    }
}