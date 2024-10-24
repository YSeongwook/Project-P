using DataStruct;
using EnumTypes;
using EventLibrary;
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

    [SerializeField] private Button btn_BuyGold;
    private GoldPackageData buyPackageData;

    private void Start()
    {
        gameObject.SetActive(false);

        btn_BuyGold.onClick.AddListener(BuyItem_Gold);
    }
    
    public void SetPackageInfo(GoldPackageData packageData)
    {
        textPriceErc.text = packageData.ERCPrice.ToString();
        textPriceGold.text = packageData.GiveGold.ToString();
        
        if (packageData.Image != null) imagePackageIcon.sprite = packageData.Image;
        else  DebugLogger.LogWarning("PackageIcon is null!");
        
        if (packageData.PackageID == "G1001") rectTransform.sizeDelta = new Vector2(206, 153);
        else if (packageData.PackageID == "G1002") rectTransform.sizeDelta = new Vector2(217, 155);
        else if(packageData.PackageID == "G1003") rectTransform.sizeDelta = new Vector2(230, 177);
        else if (packageData.PackageID == "G1004") rectTransform.sizeDelta = new Vector2(269, 207);
        else if (packageData.PackageID == "G1005") rectTransform.sizeDelta = new Vector2(340, 270);
        shineParticle.SetActive(packageData.PackageID == "G1005");
        buyPackageData = packageData;
    }

    public void PopUpOn()
    {
        this.gameObject.SetActive(true);
    }

    public void PopUpOff()
    {
        this.gameObject.SetActive(false);
    }
    
    //아이템 구매
    public void BuyItem_Gold()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickGoldBuyButton, buyPackageData);
    }
}
