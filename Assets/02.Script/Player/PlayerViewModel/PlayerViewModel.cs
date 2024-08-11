using DataStruct;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerViewModel
{
    private float gold;
    public float PlayerGold
    {
        get { return gold; }
        set
        {
            if (gold == value) return;
            gold = value;
            OnPropertyChanged(nameof(PlayerGold));
        }
    }

    private float ERC;
    public float PlayerERC
    {
        get { return ERC; }
        set
        {
            if (ERC == value) return;
            ERC = value;
            OnPropertyChanged(nameof(PlayerERC));
        }
    }

    private Dictionary<ItemData, int> inventory;
    public Dictionary<ItemData, int> PlayerInventory
    {
        get { return inventory; }
        set
        {
            if (inventory == value) return;
            inventory = value;
            OnPropertyChanged(nameof(PlayerInventory));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    } 
}
