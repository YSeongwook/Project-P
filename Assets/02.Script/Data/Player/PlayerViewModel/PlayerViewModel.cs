using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DataStruct;
using UnityEngine;

public class PlayerViewModel
{
    private int tickets;
    public int GameTickets 
    {
        get { return tickets; }
        set
        {
            if(tickets == value) return;
            tickets = value;
            OnPropertyChanged(nameof(GameTickets));
        }
    }

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

    private int currentChapter;
    public int CurrentChapter
    {
        get { return currentChapter; }
        set
        {
            if (currentChapter == value) return;
            currentChapter = value;
            OnPropertyChanged(nameof(CurrentChapter));
        }
    }

    private int currentStage;
    public int CurrentStage
    {
        get { return currentStage; }
        set
        {
            if (currentStage == value) return;
            currentStage = value;
            OnPropertyChanged(nameof(CurrentStage));
        }
    }

    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();
    public Dictionary<ItemData, int> PlayerInventory
    {
        get { return inventory; }
        set
        {
            if (inventory.SequenceEqual(value)) 
                return;

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
