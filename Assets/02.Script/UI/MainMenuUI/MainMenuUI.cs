using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [FoldoutGroup("Panel")] [SerializeField] private GameObject chapterPanel; // 스테이지 선택 패널
    [FoldoutGroup("Panel")] [SerializeField] private GameObject shopPanel; // 상점 패널
    
    [FoldoutGroup("Main Under Buttons")] [SerializeField] private Button socialButton; // 소셜 버튼
    [FoldoutGroup("Main Under Buttons")] [SerializeField] private Button chapterButton; // 챕터 버튼
    [FoldoutGroup("Main Under Buttons")] [SerializeField] private Button shopButton; // 상점 버튼
    
    private void Awake()
    {
        AddButtonEvents();
    }

    private void OnDestroy()
    {
        RemoveButtonEvents();
    }

    private void AddButtonEvents()
    {
        chapterButton.onClick.AddListener(OnClickChapterButton);
        shopButton.onClick.AddListener(OnClickShopButton);
    }
    
    private void RemoveButtonEvents()
    {
        chapterButton.onClick.RemoveListener(OnClickChapterButton);
        shopButton.onClick.RemoveListener(OnClickShopButton);
    }

    private void ToggleMainMenuUI()
    {
        bool isActive = gameObject.activeSelf;
        
        gameObject.SetActive(!isActive);
    }

    private void ToggleSelectStagePanel()
    {
        bool isActive = chapterPanel.activeSelf;
        
        chapterPanel.SetActive(!isActive);
    }

    private void ToggleShopPanel()
    {
        bool isActive = shopPanel.activeSelf;
        
        shopPanel.SetActive(!isActive);
    }

    private void OnClickChapterButton()
    {
        ToggleSelectStagePanel();
    }

    private void OnClickShopButton()
    {
        ToggleShopPanel();
    }

    public void DebugClickButton()
    {
        DebugLogger.Log("버튼 눌림");
    }
}
