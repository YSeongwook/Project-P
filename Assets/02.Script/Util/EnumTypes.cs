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
        OnClickItemBuyButton,
        OnClickGoldBuyButton,
        OnClickChangeBuyItemCount,
        OnClickEnableItemBuyPopup,
        OnClickEnableGoldBuyPopup,
        OnClickItemBuyExit,
        OnClickGoldBuyExit,
        OnCreateItemSlot,
        OnCreateGoldPackageSlot,
        GoldStorePopup,
        GoldStoreExit
    }

    public enum DataEvents
    {
        OnUserDataSave,
        OnUserDataLoad,
        OnUserDataReset,
        OnItemDataLoad,
        HeroCollectionUpdated,
        OnPaymentSuccessful
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