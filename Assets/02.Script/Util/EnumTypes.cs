using UnityEngine;

namespace EnumTypes
{
    public enum PlayerState
    {
        Stand,
        Move,
        Run,
        Hold
    }

    public enum Layers
    {
        Default,
        TransparentFX,
        IgnoreRaycast,
        Reserved1,
        Water,
        UI,
        Reserved2,
        Reserved3,
        Player,
        Enemy,
    }

    public enum HeroEvents
    {
        LeaderAttackStarted,
        LeaderAttackStopped,
        LeaderDirectionChanged,
    }

    public enum FormationEvents
    {
        OnChangeLeaderMode,
        SetLeader,
    }

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
        HeroCollectionUpdated,
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

    public enum GoogleEvents
    {
        GPGSSignIn,
        ManualGPGSSignIn,
    }

    public enum FirebaseEvents
    {
        FirebaseInitialized,
        FirebaseDatabaseInitialized,
        FirebaseSignIn,
        EmailSignIn,
    }
    
    public class EnumTypes : MonoBehaviour
    {
    }
}