using System.IO;

public enum ItemType
{

}

public struct ItemData
{
    public string ItemID;
    public string Name;
    public string Description;
    public ItemType Type;
    public float GoldPrice;
    public float ERCPrice;
    public string Image;
}
