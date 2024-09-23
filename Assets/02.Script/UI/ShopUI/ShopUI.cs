using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject itemShopPanel;
    
    private void Awake()
    {
        AddEvents();
    }

    private void Start()
    {
        itemShopPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        
    }

    private void RemoveEvents()
    {
        
    }
}
