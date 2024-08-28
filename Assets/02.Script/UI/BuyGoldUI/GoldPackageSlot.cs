using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldPackageSlot : MonoBehaviour
{
    [FoldoutGroup("Gold Shop UI")][SerializeField] private Image imagePackageIcon;
    [FoldoutGroup("Gold Shop UI")][SerializeField] private TMP_Text textPriceErc;

    private GoldPackageData _packageInfo;

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnClickEnableGoldBuyPopup, PopUpOn);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickGoldBuyExit, PopUpOff);
    }
    private void RemoveEvent()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnClickEnableGoldBuyPopup, PopUpOn);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickGoldBuyExit, PopUpOff);
    }

    //패키지 정보 초기화
    public void SetPackageInfo(GoldPackageData packageData)
    {
        _packageInfo = packageData;
        textPriceErc.text = _packageInfo.ERCPrice.ToString();
    }

    //구매 창 PopUp On
    private void PopUpOn()
    {
        this.gameObject.SetActive(true);
    }

    //구매 창 PopUp Off
    private void PopUpOff()
    {
        this.gameObject.SetActive(false);
    }

    //골드 구매
    public void BuyGold_ERC()
    {
        //ERC 감소
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickGoldBuyButton, _packageInfo);
        // ERC 코인 소비 코드 전송

        //골드 획득
        EventManager<GoldEvent>.TriggerEvent(GoldEvent.OnGetGold, _packageInfo.GiveGold);

        //골드 상점 닫기
        //EventManager<UIEvents>.TriggerEvent(UIEvents.GoldStoreExit);
    }
}
