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
        PlayerTicketCountChanged,
        PlayerGoldChanged,
        PlayerERCChanged,
        PlayerItemListChanged,
        PlayerOpenStageInfo,
        LoadThisChapterTileList,
        ResetChapterTileList,
        SelectStage,
        CheckAnswer,
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
        UseTurn
    }
    
    public class EnumTypes : MonoBehaviour
    {
    }
}