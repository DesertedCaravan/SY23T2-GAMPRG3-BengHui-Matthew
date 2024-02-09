using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class ItemBase : ScriptableObject
{
    enum ItemType
    {
        Healing,
        Equip
    }

    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;

    /*
    public void Test()
    {
        if (itemType == ItemType.Healing)
        {

        }
    }
    */
}
