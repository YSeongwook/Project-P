using UnityEngine;

public class ShopUI : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Debuglog()
    {
        Debug.Log("눌림");
    }
    private void Awake()
    {
        AddEvents();
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
