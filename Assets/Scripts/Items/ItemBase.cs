using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemBase : ScriptableObject
{
    public enum ItemType
    {
        Heal, // 0
        Buff, // 1
        Perma, // 2
        Equip, // 3
        Key // 4
    }

    [SerializeField] private ItemType itemType;
    private int _itemValue;

    [SerializeField] private string nameText;
    [SerializeField] [TextArea] private string descriptionText;
    [SerializeField] private Sprite icon;

    public int ChosenItemType => _itemValue;
    public string Name => nameText;
    public string Description => descriptionText;
    public Sprite Icon => icon;

    public virtual void OnValidate()
    {
        _itemValue = (int)itemType;
    }
}

/*
 * 0. Healing Items
 * Mixed Nuts - A handful of assorted nuts. You don't recognize some of them. Restores 5 Vitality when eaten.
 * Cheese - A piece of cheese. Either from the cows or the goats. Restores 10 Vitality when eaten.
 * Drumstick - A roasted chicken leg. The bone looks bigger than usual. Restores 25 Vitality when eaten.
 * Sausage - A sausage. The butcher says it's made from pork. Restores 50 Vitality when eaten.
 * 
 * 1. Buff Items
 * Strong Draught - A traditional brew brimming with vigor. Boosts the Strength of one ally.
 * 
 * 2. Perma Items
 * S Pill - A strange red pill. It feels warm to the touch. Permanently increases the Strength of one ally by 1.
*/