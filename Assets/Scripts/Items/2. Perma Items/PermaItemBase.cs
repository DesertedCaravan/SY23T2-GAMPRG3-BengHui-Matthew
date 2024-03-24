using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perma Item", menuName = "Inventory/Perma Item")]
public class PermaItemBase : ItemBase
{
    public enum PermaType
    {
        StatGain,
        ExpGain,
        EvolveGain,
        LimbLearn,
        MoveLearn
    }

    public enum PermaStat
    {
        Vitality,
        Stamina,
        Reason,
        Strength,
        Toughness,
        Agility,
        Power,
        Luck,
        Evasion
    }

    public enum PermaLimb
    {
        None,
        Cerebrum,
        Oculus,
        Feeler,
        Horn,
        Fang,
        Jaw,
        Claw,
        Arm,
        Torso,
        Leg,
        Tail
    }

    [SerializeField] private PermaType permaType;
    private int _permaType;

    [Header("Stat Gain")]
    [SerializeField] private PermaStat permaStat;
    private int _permaStat;

    [SerializeField] private int _statGain;

    [Header("Exp Gain")]
    [SerializeField] private int _permaLevel;
    [SerializeField] private int _permaExp;

    [Header("Evolve Gain")]
    [SerializeField] private bool _permaEvolve;

    [Header("Limb Learn")]
    [SerializeField] private PermaLimb tabletLimb;
    private int _permaTabletLimb;

    [Header("Move Learn")]
    [SerializeField] private MoveData tomeMove;

    public int Type => _permaType;

    public int StatType => _permaStat;
    public int StatGain => _statGain;

    public int Level => _permaLevel;
    public int Exp => _permaExp;

    public bool Evolve => _permaEvolve;

    public int TabletLimb => _permaTabletLimb;

    public MoveData TomeMove => tomeMove;

    public override void OnValidate()
    {
        base.OnValidate();

        _permaType = (int)permaType;
        _permaStat = (int)permaStat;
        _permaTabletLimb = (int)tabletLimb;

        if (_statGain == 0)
        {
            _statGain = 1;
        }
    }
}
