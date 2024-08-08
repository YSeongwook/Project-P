using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum DataType
{
    Item,
    GoldPackage
}

public class DataManager : Singleton<DataManager>
{

    #region LoadData
    public Dictionary<int, ItemData> LoadedItemDataList { get; private set; }

    private Dictionary<DataType, TextAsset> textAssetDic = new Dictionary<DataType, TextAsset>();

    void LoadFile()
    {
        textAssetDic.Add(DataType.Item, Resources.Load("Temp_ItemList") as TextAsset);
    }

    private void ReadDataOnAwake()
    {
        ReadItemData(DataType.Item);
    }

    private void ReadItemData(DataType dataType)
    {
        var textAsset = textAssetDic[dataType];
        if (textAsset == null) return;

        XDocument xmlAsset = XDocument.Parse(textAsset.text);
        if (xmlAsset == null) return;

        FileType_MonsterData(xmlAsset);
    }

    private void FileType_MonsterData(XDocument xmlAsset)
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
    #endregion

    public Dictionary<int, ItemData> GetItemInfoDatas()
    {
        return LoadedItemDataList;
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

}
