using System.Collections.Generic;
using DataStruct;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;

public class UI_GoldStoreList : MonoBehaviour
{
    [FoldoutGroup("Gold Shop List")] 
    [SerializeField] private GameObject[] goldPackage;
    [SerializeField] private GameObject goldShop;

    private Dictionary<string, GoldPackageData> _goldPackageDataDictionary;
    
    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        goldShop.SetActive(false);
    }

    private void AddEvent()
    {
        EventManager<UIEvents>.StartListening(UIEvents.OnCreateGoldPackageSlot, GoldPackageSlotInfo);
    }

    private void RemoveEvent()
    {
        EventManager<UIEvents>.StopListening(UIEvents.OnCreateGoldPackageSlot, GoldPackageSlotInfo);
    }

    private void GoldPackageSlotInfo()
    {
        _goldPackageDataDictionary = DataManager.Instance.GetGoldPackageDatas();

        int count = 0;
        foreach (var PackageData in _goldPackageDataDictionary.Values)
        {
            GameObject PakageSlot = goldPackage[count++];
            GoldPackageSlot goldPackageSlot = PakageSlot.GetComponent<GoldPackageSlot>();
            goldPackageSlot.SetPackageInfo(PackageData);
            // count++;
        }
    }
}
