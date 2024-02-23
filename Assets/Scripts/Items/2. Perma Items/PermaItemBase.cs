using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perma Item", menuName = "Inventory/Perma Item")]
public class PermaItemBase : ItemBase
{
    public enum BattleStat
    {
        Strength,
        Toughness,
        Agility,
        Power,
        Luck,
        Evasion
    }

    [SerializeField] private BattleStat battleStat;
    [SerializeField] private int statGain;

    public BattleStat ChosenBattleStat => battleStat;
    public int StatGain => statGain;

    public override void OnValidate()
    {
        base.OnValidate();

        if (statGain == 0)
        {
            statGain = 1;
        }
    }
}
