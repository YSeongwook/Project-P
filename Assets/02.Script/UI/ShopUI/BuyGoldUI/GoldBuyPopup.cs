using DataStruct;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoldBuyPopup : MonoBehaviour
{
    [SerializeField] private Image imagePackageIcon; // 패키지 아이콘 이미지
    [SerializeField] private RectTransform rectTransform; //패키지 아이콘 크기
    [SerializeField] private TMP_Text textPriceErc; // ERC 가격 텍스트
    [SerializeField] private TMP_Text textPriceGold; // 골드 가격 텍스트
    [SerializeField] private GameObject shineParticle; // 특별한 아이디의 패키지에 대한 파티클 효과

    private void Start()
    {
        gameObject.SetActive(false);
    }
    
    public void SetPackageInfo(GoldPackageData pakagedata)
    {

        textPriceErc.text = pakagedata.ERCPrice.ToString();
        textPriceGold.text = pakagedata.GiveGold.ToString();
        
        if (pakagedata.Image != null)
        {
            imagePackageIcon.sprite = pakagedata.Image;
        }
        else
        {
            Debug.LogWarning("PackageIcon is null!");
        }
        
        if (pakagedata.PackageID == "G1001") rectTransform.sizeDelta = new Vector2(206, 153);
        else if (pakagedata.PackageID == "G1002") rectTransform.sizeDelta = new Vector2(217, 155);
        else if(pakagedata.PackageID == "G1003") rectTransform.sizeDelta = new Vector2(230, 177);
        else if (pakagedata.PackageID == "G1004") rectTransform.sizeDelta = new Vector2(269, 207);
        else if (pakagedata.PackageID == "G1005") rectTransform.sizeDelta = new Vector2(340, 270);
        shineParticle.SetActive(pakagedata.PackageID == "G1005");
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
