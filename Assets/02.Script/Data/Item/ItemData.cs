using System.Collections.Generic;

namespace DataStruct
{
    public struct ItemData
    {
        public string ItemID;
        public string Name;
        public string Description;
        public float GoldPrice;
        public float ERCPrice;
        public string Image;
    }

    public struct GoldPackageData
    {
        public string PackageID;
        public string Name;
        public string Description;
        public float ERCPrice;
        public float GiveGold;
        public string Image;
    }

    public struct PlayerInfo
    {
        public string PlayerID;
        public float Gold;
        public float ERC;
        public int TicketCount;
        public Dictionary<ItemData, int> ItemList;
        public string CurrentChapter;
        public string CurrentStage;
    }
}