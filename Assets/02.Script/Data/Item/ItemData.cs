using System.Collections.Generic;
using UnityEngine;

namespace DataStruct
{
    public struct ItemData
    {
        public string ItemID;
        public string Name;
        public string Description;
        public float GoldPrice;
        public float ERCPrice;
        public Sprite Image;
    }

    public struct GoldPackageData
    {
        public string PackageID;
        public string Name;
        public string Description;
        public float ERCPrice;
        public float GiveGold;
        public Sprite Image;
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

    public struct TicketData
    {
        public string TicketID;
        public string TicketCount;
        public float GoldPrice;
        public Sprite Image;
    }


    public struct StageGameMapInfoTable
    {
        public string MapID;
        public string FileName;
        public int LimitCount;
    }
}