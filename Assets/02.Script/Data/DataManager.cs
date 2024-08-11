using EnumTypes;
using DataStruct;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using System.Xml.Serialization;

public enum DataType
{
    Item,
    GoldPackage,
    PlayerInven
}

public class DataManager : Singleton<DataManager>
{

    #region LoadData
    public Dictionary<int, ItemData> LoadedItemDataList { get; private set; }
    public Dictionary<int, GoldPackageData> LoadedGoldPackageDataList { get; private set; }
    public Dictionary<int, Inventory> LoadedPlayerInventoryList { get; private set; }

    private Dictionary<DataType, TextAsset> textAssetDic = new Dictionary<DataType, TextAsset>();

    private void LoadFile()
    {
        textAssetDic.Add(DataType.Item, Resources.Load("Temp_ItemList") as TextAsset);
        textAssetDic.Add(DataType.GoldPackage, Resources.Load("Temp_PackageList") as TextAsset);
        textAssetDic.Add(DataType.PlayerInven, Resources.Load("Temp_PlayerInventory") as TextAsset);
    }

    private void ReadDataOnAwake()
    {
        ReadItemData(DataType.Item);
        ReadGoldPackageData(DataType.GoldPackage);
        ReadPlayerInventory(DataType.PlayerInven);
    }

    private void ReadPlayerInventory(DataType dataType)
    {
        var textAsset = textAssetDic[dataType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if (xmlAsset == null) return;

        FileType_PlayerInventoryData(xmlAsset);
    }

    private void ReadItemData(DataType dataType)
    {
        var textAsset = textAssetDic[dataType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if (xmlAsset == null) return;

        FileType_ItemData(xmlAsset);
    }

    private void ReadGoldPackageData(DataType dataType)
    {
        var textAsset = textAssetDic[dataType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if (xmlAsset == null) return;

        FileType_GoldPackageData(xmlAsset);
    }

    private void FileType_ItemData(XDocument xmlAsset)
    {
        LoadedItemDataList = new Dictionary<int, ItemData>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            ItemData item = new ItemData();
            item.ItemID = int.Parse(data.Attribute(nameof(item.ItemID)).Value.Substring(1));
            item.Name = data.Attribute(nameof(item.Name)).Value;
            item.Description = data.Attribute(nameof(item.Description)).Value;
            item.GoldPrice = float.Parse(data.Attribute(nameof(item.GoldPrice)).Value);
            item.ERCPrice = float.Parse(data.Attribute(nameof(item.ERCPrice)).Value);
            item.Image = data.Attribute(nameof(item.Image)).Value;

            LoadedItemDataList.Add(item.ItemID, item);
        }
    }

    private void FileType_GoldPackageData(XDocument xmlAsset)
    {
        LoadedGoldPackageDataList = new Dictionary<int, GoldPackageData>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            GoldPackageData item = new GoldPackageData();
            item.PackageID = int.Parse(data.Attribute(nameof(item.PackageID)).Value.Substring(1));
            item.Name = data.Attribute(nameof(item.Name)).Value;
            item.Description = data.Attribute(nameof(item.Description)).Value;
            item.ERCPrice = float.Parse(data.Attribute(nameof(item.ERCPrice)).Value);
            item.GiveGold = float.Parse(data.Attribute(nameof(item.GiveGold)).Value);
            item.Image = data.Attribute(nameof(item.Image)).Value;

            LoadedGoldPackageDataList.Add(item.PackageID, item);
        }
    }

    private void FileType_PlayerInventoryData(XDocument xmlAsset)
    {
        LoadedPlayerInventoryList = new Dictionary<int, Inventory>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            Inventory playerInven = new Inventory();
            playerInven.PlayerID = int.Parse(data.Attribute(nameof(playerInven.PlayerID)).Value.Substring(1));
            playerInven.Gold = float.Parse(data.Attribute(nameof(playerInven.Gold)).Value);
            playerInven.ERC = float.Parse(data.Attribute(nameof(playerInven.ERC)).Value);

            string playerInventoryItemList = data.Attribute(nameof(playerInven.ItemList)).Value;
            playerInven.ItemList = ParseItemList(playerInventoryItemList);

            LoadedPlayerInventoryList.Add(playerInven.PlayerID, playerInven);
        }
    }

    //아이템 인벤토리 ID와 갯수로 나누기
    private Dictionary<ItemData, int> ParseItemList(string itemListString)
    {
        var itemList = new Dictionary<ItemData, int>();

        var items = itemListString.Trim('{', '}').Split(',');

        foreach (var item in items)
        {
            var parts = item.Split(':');
            int itemID = int.Parse(parts[0].Substring(1));
            int quantity = int.Parse(parts[1]);

            ItemData itemData = new ItemData { ItemID = itemID };

            itemList.Add(itemData, quantity);
        }

        return itemList;
    }
    #endregion

    #region SaveData
    //파일을 저장할 위치 지정

    //Resource 파일의 Data Update는 불가.

    #endregion

    public Dictionary<int, ItemData> GetItemInfoDatas()
    {
        return LoadedItemDataList;
    }

    public Dictionary<int, GoldPackageData> GetGoldPackageDatas()
    {
        return LoadedGoldPackageDataList;
    }

    public Dictionary<int, Inventory> GetPlayerInventoryDatas()
    {
        return LoadedPlayerInventoryList;
    }

    public ItemData GetItemData(int itemID)
    {
        if (LoadedItemDataList.ContainsKey(itemID)) return LoadedItemDataList[itemID];
        else return default;
    }

    protected new void Awake()
    {
        base.Awake();

        LoadFile();
        ReadDataOnAwake();
    }

    private void Start()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnCreateItemSlot);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnCreateGoldPackageSlot);
        EventManager<DataEvents>.TriggerEvent(DataEvents.OnUserInventoryLoad);
    }

}
