using System.IO;

namespace DataStruct
{
    public struct ItemData
    {
        public int ItemID;
        public string Name;
        public string Description;
        public float GoldPrice;
        public float ERCPrice;
        public string Image;
    }

    public struct GoldPackageData
    {
        public int PackageID;
        public string Name;
        public string Description;
        public float ERCPrice;
        public float GiveGold;
        public string Image;
    }
}