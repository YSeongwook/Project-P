using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DataStruct;
using EnumTypes;
using EventLibrary;
using Newtonsoft.Json;
using UnityEngine;

public enum DataType
{
    Item,
    GoldPackage,
    PlayerInven,
    TileMap
}

public class DataManager : Singleton<DataManager>
{
    #region LoadData
    public Dictionary<string, ItemData> LoadedItemDataList { get; private set; }
    public Dictionary<string, GoldPackageData> LoadedGoldPackageDataList { get; private set; }
    public Dictionary<string, PlayerInfo> LoadedPlayerInventoryList { get; private set; }

    public Dictionary<string, List<Tile>> LoadedTileMapList { get; private set; } = new Dictionary<string, List<Tile>>();

    private Dictionary<DataType, TextAsset> textAssetDic = new Dictionary<DataType, TextAsset>();

    private void LoadFile()
    {
        textAssetDic.Add(DataType.Item, Resources.Load("Temp_ItemList") as TextAsset);
        textAssetDic.Add(DataType.GoldPackage, Resources.Load("Temp_PackageList") as TextAsset);
        textAssetDic.Add(DataType.PlayerInven, Resources.Load("Temp_PlayerInventory") as TextAsset);
    }

    private void ReadDataOnAwake()
    {
        ReadDatas(DataType.Item);
        ReadDatas(DataType.GoldPackage);
        ReadDatas(DataType.PlayerInven);
    }

    private void ReadDatas(DataType dataType)
    {
        var textAsset = textAssetDic[dataType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if (xmlAsset == null) return;

        switch (dataType)
        {
            case DataType.Item:
                FileType_ItemData(xmlAsset);
                break;
            case DataType.GoldPackage:
                FileType_GoldPackageData(xmlAsset);
                break;
            case DataType.PlayerInven:
                FileType_PlayerInventoryData(xmlAsset);
                break;
        }        
    }

    // 타일맵 읽어오기
    private void ReadTileMapData(string fileName)
    {
        string filePath = Path.Combine("C:/Download/TileMap", fileName + ".json");

        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            List<Tile> tileData = JsonConvert.DeserializeObject<List<Tile>>(json);

            if (LoadedTileMapList.ContainsKey(fileName))
            {
                LoadedTileMapList[fileName] = tileData;
            }
            else LoadedTileMapList.Add(fileName, tileData);
        }
        else
        {
            Debug.Log($"잘못된 파일 이름입니다. : {fileName}");
            return;
        }
    }

    private void ResetReadTileMapData()
    {
        LoadedTileMapList.Clear();
    }

    private void FileType_ItemData(XDocument xmlAsset)
    {
        LoadedItemDataList = new Dictionary<string, ItemData>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            ItemData item = new ItemData();
            item.ItemID = data.Attribute(nameof(item.ItemID)).Value;
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
        LoadedGoldPackageDataList = new Dictionary<string, GoldPackageData>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            GoldPackageData item = new GoldPackageData();
            item.PackageID = data.Attribute(nameof(item.PackageID)).Value;
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
        LoadedPlayerInventoryList = new Dictionary<string, PlayerInfo>();

        foreach (var data in xmlAsset.Descendants("data"))
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.PlayerID = data.Attribute(nameof(playerInfo.PlayerID)).Value;
            playerInfo.Gold = float.Parse(data.Attribute(nameof(playerInfo.Gold)).Value);
            playerInfo.ERC = float.Parse(data.Attribute(nameof(playerInfo.ERC)).Value);
            playerInfo.TicketCount = int.Parse(data.Attribute(nameof(playerInfo.TicketCount)).Value);
            playerInfo.CurrentChapter = data.Attribute(nameof(playerInfo.CurrentChapter)).Value;
            playerInfo.CurrentStage = data.Attribute(nameof(playerInfo.CurrentStage)).Value;

            string playerInventoryItemList = data.Attribute(nameof(playerInfo.ItemList)).Value;
            playerInfo.ItemList = ParseItemList(playerInventoryItemList);

            LoadedPlayerInventoryList.Add(playerInfo.PlayerID, playerInfo);
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
            string itemID = parts[0];
            int quantity = int.Parse(parts[1]);

            ItemData itemData = new ItemData { ItemID = itemID };

            itemList.Add(itemData, quantity);
        }

        return itemList;
    }
    #endregion

    #region SaveData
    //파일을 저장할 위치 지정

    private void SaveInventoryData(PlayerInfo inventory)
    {
        var textAsset = textAssetDic[DataType.PlayerInven];
        if (textAsset == null) return;

        XDocument xmlDoc = XDocument.Parse(textAsset.text);

        XElement dataElement = xmlDoc.Element("Temp_PlayerInventory")
                                        .Element("dataCategory")
                                        .Element("data");

        if(dataElement != null)
        {
            dataElement.SetAttributeValue("PlayerID", $"P{inventory.PlayerID}");
            dataElement.SetAttributeValue("Gold", inventory.Gold);
            dataElement.SetAttributeValue("ERC", inventory.ERC);
            dataElement.SetAttributeValue("ItemList", SerializeItemList(inventory.ItemList));
        }

        string filePath = Path.Combine(Application.dataPath, "Resources/Temp_PlayerInventory.xml");
        xmlDoc.Save(filePath);
    }
    //Resource 파일의 Data Update는 불가.

    private string SerializeItemList(Dictionary<ItemData, int> itemList)
    {
        List<string> items = new List<string>();

        foreach (var item in itemList)
        {
            string itemString = $"I{item.Key.ItemID}:{item.Value}";
            items.Add(itemString);
        }

        return "{" + string.Join(",", items) + "}";
    }
    #endregion

    public Dictionary<string, ItemData> GetItemInfoDatas()
    {
        return LoadedItemDataList;
    }

    public Dictionary<string, GoldPackageData> GetGoldPackageDatas()
    {
        return LoadedGoldPackageDataList;
    }

    public Dictionary<string, PlayerInfo> GetPlayerInventoryDatas()
    {
        return LoadedPlayerInventoryList;
    }

    public ItemData GetItemData(string itemID)
    {
        if (LoadedItemDataList.ContainsKey(itemID)) return LoadedItemDataList[itemID];
        else return default;
    }

    public List<Tile> GetPuzzleTileMap(string fileName)
    {
        if(LoadedTileMapList.ContainsKey(fileName)) return LoadedTileMapList[fileName];
        else return null;
    }

    protected new void Awake()
    {
        base.Awake();

        LoadFile();
        ReadDataOnAwake();

        EventManager<DataEvents>.StartListening<PlayerInfo>(DataEvents.OnUserInventorySave, SaveInventoryData);
        EventManager<DataEvents>.StartListening<string>(DataEvents.LoadThisChapterTileList, ReadTileMapData);
        EventManager<DataEvents>.StartListening(DataEvents.ResetChapterTileList, ResetReadTileMapData);
    }

    private void OnDestroy()
    {
        EventManager<DataEvents>.StopListening<PlayerInfo>(DataEvents.OnUserInventorySave, SaveInventoryData);
        EventManager<DataEvents>.StopListening<string>(DataEvents.LoadThisChapterTileList, ReadTileMapData);
        EventManager<DataEvents>.StopListening(DataEvents.ResetChapterTileList, ResetReadTileMapData);
    }

    private void Start()
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnCreateItemSlot);
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnCreateGoldPackageSlot);
        EventManager<DataEvents>.TriggerEvent(DataEvents.OnUserInformationLoad);
    }
}
