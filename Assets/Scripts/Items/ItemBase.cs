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
        Tool, // 3
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
 * Ham - A slice of ham. Is this also made of pork? Restores 80 Vitality when eaten.
 * Steak - A piece of steak. What a rare delicacy. Restores 120 Vitality when eaten.
 * 
 * Crackers
 * Bread Roll
 * Doughnuts
 * Baked Potato
 * Apple Juice
 * Orange Juice
 * Tomato Juice
 * Cranberry Sauce
 * 
 * Sandwich
 * Potato Salad
 * Goulash
 * Chili
 * 
 * Tea
 * Hot Chocolate
 * Root Beer
 * Moonshine
 * 
 * Cream Puff
 * Chocolate Bar
 * Pie
 * Cake
 * 
 * Bleed Tonic
 * Venom Tonic
 * Ill Tonic
 * Weak Tonic
 * Afraid Tonic
 * 
 * 1. Buff Items
 * Strong Draught - A traditional brew brimming with vigor. Boosts the Strength of one ally.
 * Resilient Draught
 * Active Draught
 * Potent Draught
 * Helpful Draught
 * Helpful Draught
 * Clear Draught
 * 
 * Feeble Powder
 * Brittle Powder
 * Sluggish Powder
 * Draining Powder
 * Awful Powder
 * Confusing Powder
 * 
 * 2. Perma Items
 * 
 * V Capsule - A strange dark green capsule.
 * S Capsule - A strange dark orange capsule.
 * R Capsule - A strange dark purple capsule.
 * 
 * S Pill - A strange red pill. It feels warm to the touch. Permanently increases the Strength of one ally by 1.
 * T Pill - A strange blue pill.
 * A Pill - A strange yellow pill.
 * P Pill - A strange purple pill.
 * L Pill - A strange green pill.
 * E Pill - A strange orange pill.
 * 
 * Star Pieces
 * 
 * 
 * Cerebrum Tablet
 * Oculus Tablet
 * Feeler Tablet
 * Horn Tablet
 * Fang Tablet
 * Jaw Tablet
 * Claw Tablet
 * Arm Tablet
 * Torso Tablet
 * Leg Tablet
 * Tail Tablet
 * 
 * Tomes
 * 
 * 
 * 3. Tool Items
 * 
 * Simple Hedron
 * Advanced Hedron
 * Complex Hedron
 * Relic Hedron (Works best on certain types)
 * Ether Hedron (Works best on certain types)
 * Dire Hedron (Works best on certain types)
 * 
 * 4. Key Items
 * 
*/