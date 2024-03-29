using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Updater")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Inventory Lists")]
    // [SerializeField] private List<InventoryLists> lists;

    [SerializeField] private List<InventorySlots> healingSlots;
    [SerializeField] private List<InventorySlots> buffingSlots;
    [SerializeField] private List<InventorySlots> permaSlots;
    [SerializeField] private List<InventorySlots> toolSlots;
    [SerializeField] private List<InventorySlots> keySlots;

    // public List<InventoryLists> Lists => lists;
    public InventoryUI InventoryUI => inventoryUI;
    public List<InventorySlots> HealingSlots => healingSlots;
    public List<InventorySlots> BuffingSlots => buffingSlots;
    public List<InventorySlots> PermaSlots => permaSlots;
    public List<InventorySlots> ToolSlots => toolSlots;
    public List<InventorySlots> KeySlots => keySlots;

    /*
    public static InventoryManager GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<InventoryManager>();
    }
    */

    /*
    // Convert to Singleton
    public static InventoryManager instance = null; // public static means that it can be accessed

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }
    */

    public List<InventorySlots> SelectList(int listNumber)
    {
        if (listNumber == 0)
        {
            return HealingSlots;
        }
        else if (listNumber == 1)
        {
            return BuffingSlots;
        }
        else if (listNumber == 2)
        {
            return PermaSlots;

        }
        else if (listNumber == 3)
        {
            return ToolSlots;
        }
        else if (listNumber == 4)
        {
            return KeySlots;
        }

        return KeySlots;
    }

    public bool CheckForItem(ItemBase item)
    {
        for (int i = 0; i < SelectList(item.ChosenItemType).Count; i++)
        {
            if (item.Name == SelectList(item.ChosenItemType)[i].GetItem.Name)
            {
                if (SelectList(item.ChosenItemType)[i].GetAmount > 0) // there has to be at least one of the requested item
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void AddItem(ItemBase item, int amount)
    {
        bool itemCheck = false;

        for (int i = 0; i < SelectList(item.ChosenItemType).Count; i++)
        {
            if (item.Name == SelectList(item.ChosenItemType)[i].GetItem.Name)
            {
                itemCheck = true;

                SelectList(item.ChosenItemType)[i].AddAmount = amount;

                InventoryUI.UpdateListItems();

                return;
            }
        }

        if (itemCheck == false)
        {
            InventorySlots newSlot = new InventorySlots();
            newSlot.SetItem = item;
            newSlot.AddAmount = amount;

            SelectList(item.ChosenItemType).Add(newSlot);

            InventoryUI.UpdateListItems();
        }
    }

    public void RemoveItem(ItemBase item, int amount) // Check if there is enough item amount to remove
    {
        for (int i = 0; i < SelectList(item.ChosenItemType).Count; i++)
        {
            if (item.Name == SelectList(item.ChosenItemType)[i].GetItem.Name)
            {
                if (SelectList(item.ChosenItemType)[i].GetAmount >= amount) // only subtract if the amount in stock is more or equal to the amount requested
                {
                    SelectList(item.ChosenItemType)[i].SubtractAmount = amount;

                    InventoryUI.UpdateListItems();

                    return;
                }
            }
        }
    }
}

/*
[Serializable] // Allows this to be displayed in the inspector (requires using System)
public class InventoryLists
{
    [SerializeField] private List<InventorySlots> slots;
}
*/

[Serializable] // Allows this to be displayed in the inspector (requires using System)
public class InventorySlots
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int amount;

    public ItemBase GetItem { get { return item; } }
    public ItemBase SetItem { set { item = value; } }

    public int GetAmount { get { return amount; } }
    public int AddAmount
    {
        set
        { 
            amount += value;

            if (amount > 999)
            {
                amount = 999;
            }
        }
    }
    public int SubtractAmount { set { amount -= value; } }
}