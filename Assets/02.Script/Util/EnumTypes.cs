using UnityEngine;

namespace EnumTypes
{
    public enum UIEvents
    {
        OnClickSignInGoogle,
        OnClickStart,
        OnClickUseTicket,
        OnClickItemBuyButton,
        OnClickGoldBuyButton,
        OnClickChangeBuyItemCount,
        OnClickEnableItemBuyPopup,
        OnClickEnableGoldBuyPopup,
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
    }

    public enum GoldEvent
    {
        OnGetGold,
        OnUseERC,
        OnGetERC
    }

    public enum StageEvent
    {
        StartStage,
        ResetTileGrid,
        SetTileGrid,
        UseTurn
    }
    
    public enum PuzzleEvent
    {
        StartClearAnimation,
    }

    public class EnumTypes : MonoBehaviour
    {
    }
}