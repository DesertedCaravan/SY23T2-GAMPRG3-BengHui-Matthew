using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buff Item", menuName = "Inventory/Buff Item")]
public class BuffItemBase : ItemBase
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

    public enum Target
    {
        OneAlly,
        AllyRow,
        PHandler,
        OneEnemy,
        EnemyRow,
        EHandler,
        All
    }

    [SerializeField] private BattleStat battleStat;
    [SerializeField] private Target target;
    [SerializeField] private int statStageChange;
    [SerializeField] private bool statEffect;

    public BattleStat ChosenBattleStat => battleStat;
    public Target ChosenTarget => target;
    public int StatStageChange => statStageChange;
    public bool StatEffect => statEffect;

    public override void OnValidate()
    {
        base.OnValidate();

        if (statStageChange == 0)
        {
            statStageChange = 1;
        }
    }
}
