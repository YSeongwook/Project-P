using DataStruct;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldBuyPopup : MonoBehaviour
{
    [SerializeField] private Image imagePackageIcon; // 패키지 아이콘 이미지
    [SerializeField] private TMP_Text textPriceErc; // ERC 가격 텍스트
    [SerializeField] private TMP_Text textPriceGold; // 골드 가격 텍스트
    [SerializeField] private GameObject shineParticle; // 특별한 아이디의 패키지에 대한 파티클 효과

    private GoldPackageData _packageInfo;

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

        shineParticle.SetActive(_packageInfo.PackageID == "G1005");
        
    }

    public void PopUpOn()
    {
        this.gameObject.SetActive(true);
    }

    public void PopUpOff()
    {
        this.gameObject.SetActive(false);
    }
}
