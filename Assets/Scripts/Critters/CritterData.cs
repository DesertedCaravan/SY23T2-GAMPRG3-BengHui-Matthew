using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[CreateAssetMenu(fileName = "New Critter Data", menuName ="Critter Creation/Critter Data")]
public class CritterData : ScriptableObject
{
    public enum GrowthRate
    {
        Slow, // 4
        Medium, // 5
        Fast // 6
    }
    public enum Type
    {
        Anima,
        Vega,
        Bio,
        Psycho,
        Umbra,
        Thauma,
        Haunt,
        Gaunt,
        Auto,
        Giga,
        Thalassa,
        Inferna,
        Hyper,
        Celes,
        Luna,
        None
    }
    
    public enum Limb
    {
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

    [Header("Critter Info")]
    [SerializeField] private string _speciesName;
    [SerializeField] private Sprite _sprite;

    [Header("Critter Category")]
    [SerializeField] private Type type1;
    private int _type1;
    [SerializeField] private Type type2;
    private int _type2;
    [SerializeField] private GrowthRate _growthRate;
    private int _growthValue;

    [Header("Base Health Stats")]
    [SerializeField] private int _baseVitality;
    [SerializeField] private int _baseStamina;
    [SerializeField] private int _baseReason;

    [Header("Base Battle Stats")]
    [SerializeField] private int _baseStrength;
    [SerializeField] private int _baseToughness;
    [SerializeField] private int _baseAgility;
    [SerializeField] private int _basePower;
    [SerializeField] private int _baseLuck;
    [SerializeField] private int _baseEvasion;

    [Header("Move Data")]
    [SerializeField] private List<Limb> limbs;
    private List<int> limbValues;
    [SerializeField] private List<LevelMoves> levelMoves;

    // Public Pointers

    public string SpeciesName => _speciesName;
    public Sprite Sprite => _sprite;

    public int Type1 => _type1;
    public int Type2 => _type2;
    public int GrowthValue => _growthValue;

    public int BaseVitality => _baseVitality;
    public int BaseStamina => _baseStamina;
    public int BaseReason => _baseReason;

    public int BaseStrength => _baseStrength;
    public int BaseToughness => _baseToughness;
    public int BaseAgility => _baseAgility;
    public int BasePower => _basePower;
    public int BaseLuck => _baseLuck;
    public int BaseEvasion => _baseEvasion;

    public List<int> LimbValues => limbValues;
    public List<LevelMoves> LevelMoves => levelMoves; // must consider limbs

    public void OnValidate()
    {
        _type1 = (int)type1;
        _type2 = (int)type2;

        limbValues = new List<int>();

        for (int i = 0; i < limbs.Count; i++)
        {
            limbValues.Add((int)limbs[i]);
        }

        if (_growthRate == GrowthRate.Slow)
        {
            _growthValue = 6;
        }
        else if (_growthRate == GrowthRate.Medium)
        {
            _growthValue = 5;
        }
        else if (_growthRate == GrowthRate.Fast)
        {
            _growthValue = 4;
        }
    }
}

[Serializable] // Allows this to be displayed in the inspector (requires using System)
public class LevelMoves
{
    [SerializeField] private int level;
    [SerializeField] private MoveData move;

    public int GetLevel { get { return level; } }
    public MoveData GetMove { get { return move; } }
}