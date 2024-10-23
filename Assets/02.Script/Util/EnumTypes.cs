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
        OnClickEnableTicketBuyPopup,
        OnClickEnableGoldStore,
        OnClickGoldBuyExit,
        OnCreateItemSlot,
        OnCreateGoldPackageSlot,
        OnCreateTicketSlot,
        GetPlayerInventoryResources,
        GoldStorePopup,
        GoldStoreExit,
        OnClickGameStage,
        OnEnableChapterMoveButton,
        CreateStageButton,
        ChangeScrollViewCenter,
        MissionSuccessPopUp,
        GameMessagePopUp,
        OnClickNextButton,
        OnClickRestartButton,
        ActiveMiniGameUI,
    }

    public enum MiniGame
    {
        Catch,
        PoliceGameOver,
        FeedCountChanged,
        ActiveMiniGame,
        DisActiveMiniGame,
        StartMiniGame,
        MiniGameEnd,        
        SetStartTrigger,
    }

    public enum DataEvents
    {
        OnUserInformationLoad,
        OnUserInventorySave,
        OnItemPopupDataLoad,
        OnTicketPopupDataLoad,
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
        UpdateLobby,
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
        SetRandomRotateLinkTile,
        SortPathTileGrid,
        UseTurn,
        MissionSuccess,
        CheckMissionFail,
        ReturnSelectStage,
        RestartStage,
        RecoveryLimitCount,
        LoadInventoryItem,
        SetPlayerItemInventoryList,
        NextStage,
        LastStage,
        TutorialStage,
        GameEnd,
        SetMiniGame,
        CreateToken,
    }

    public enum VibrateEvents
    {
        ShortWeak,
        ShortStrong,
        LongWeak,
        LongStrong,
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
        AddItem,
        DecreaseItemCount,
        SetEnableButton,
        SetReverseRotate,
        SetHint,
        RecoveryTicketCountAfterGameClear,
    }

    public class EnumTypes : MonoBehaviour
    {
    }
}