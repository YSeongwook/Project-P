using System;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    private void Awake()
    {
        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void AddEvents()
    {
        
    }

    private void RemoveEvents()
    {
        
    }
}
