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
    [FoldoutGroup("Gold shop UI")][SerializeField] private TMP_Text textPriceGold;
    [FoldoutGroup("Gold shop UI")][SerializeField] private GameObject ShineParticle;
    [SerializeField] private GameObject goldBuyPopup; // 팝업 프리팹

    private GoldPackageData _packageInfo;

    private void Awake()
    {
        // 추가: 슬롯 클릭 이벤트 등록
        GetComponent<Button>().onClick.AddListener(OnSlotClick);
    }

    // 패키지 정보 초기화
    public void SetPackageInfo(GoldPackageData packageData)
    {
        _packageInfo = packageData;
        UpdateUI();
    }

    private void UpdateUI()
    {
        textPriceErc.text = _packageInfo.ERCPrice.ToString();
        textPriceGold.text = _packageInfo.GiveGold.ToString();

        if (_packageInfo.Image != null)
        {
            imagePackageIcon.sprite = _packageInfo.Image;
        }
        else
        {
            Debug.LogWarning("PackageIcon is null!");
        }

        ShineParticle.SetActive(_packageInfo.PackageID == "G1005");
    }

    // 슬롯 클릭 시 팝업 열기
    private void OnSlotClick()
    {
        if (goldBuyPopup != null)
        {
            GoldBuyPopup popup = goldBuyPopup.GetComponent<GoldBuyPopup>();

            if (popup != null)
            {
                popup.SetPackageInfo(_packageInfo);
                goldBuyPopup.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Gold Buy Popup is not assigned.");
        }
    }
}
