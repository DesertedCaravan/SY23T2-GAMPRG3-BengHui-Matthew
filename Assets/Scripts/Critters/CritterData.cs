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
    [SerializeField] private string _name;
    [SerializeField] private string _speciesName;
    [SerializeField] private Sprite _sprite;

    [Header("Critter Category")]
    [SerializeField] private Type type1;
    private int _type1;
    [SerializeField] private Type type2;
    private int _type2;

    [Header("Level Stats")]
    [SerializeField] private int _level;
    [SerializeField] private int _expPoints;
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

    // Calculated Health Stats
    private int _vitality;
    private int _stamina;
    private int _reason;

    private int _currentVitality;
    private int _currentStamina;
    private int _currentReason;

    // Calculated Battle Stats
    private int _strength;
    private int _toughness;
    private int _agility;
    private int _power;
    private int _luck;
    private int _evasion;

    [Header("Move Data")]
    [SerializeField] private List<Limb> limbs;
    private List<int> limbValues;
    [SerializeField] private List<LevelMoves> levelMoves;

    [Header("Known Moves")]
    [SerializeField] private List<MoveData> setMoves; // max of 4
    [SerializeField] private List<MoveData> learnedMoves; // max of 4

    public string Name => _name;
    public string SpeciesName => _speciesName;
    public Sprite Sprite => _sprite;

    public int Type1 => _type1;
    public int Type2 => _type2;

    public int Level => _level;
    public int ExpPoints => _expPoints;
    public GrowthRate ExpGrowthRate => _growthRate;

    public int Vitality => _vitality;
    public int Stamina => _stamina;
    public int Reason => _reason;
    public int CurrentVitality => _currentVitality;
    public int CurrentStamina => _currentStamina;
    public int CurrentReason => _currentReason;

    public int Strength => _strength;
    public int Toughness => _toughness;
    public int Agility => _agility;
    public int Power => _power;
    public int Luck => _luck;
    public int Evasion => _evasion;

    public List<int> LimbValues => limbValues;
    public List<LevelMoves> LevelMoves => levelMoves; // must consider limbs
    public List<MoveData> SetMoves => setMoves; // must consider limbs
    public List<MoveData> LearnedMoves => learnedMoves; // must consider limbs

    public void OnValidate()
    {
        _type1 = (int)type1;
        _type2 = (int)type2;

        limbValues = new List<int>();

        for (int i = 0; i < limbs.Count; i++)
        {
            limbValues.Add((int)limbs[i]);
        }

        // Buggy when used
        /*
        foreach (LevelMoves levelmoves in LevelMoves)
        {
            MoveData moveCheck = levelmoves.GetMove;

            bool limbChecker = false;

            foreach (Limb limb in limbs)
            {
                if (moveCheck.Limb == (int)limb)
                {
                    limbChecker = true;
                }
            }

            if (limbChecker == false)
            {
                LevelMoves.Remove(levelmoves);
            }
        }
        */

        foreach (MoveData move in LearnedMoves)
        {
            bool limbChecker = false;

            foreach (Limb limb in limbs)
            {
                if (move.Limb == (int)limb)
                {
                    limbChecker = true;
                }
            }

            if (limbChecker == false)
            {
                LearnedMoves.Remove(move);
            }
        }

        setMoves.Clear();

        foreach (LevelMoves move in LevelMoves)
        {
            if (move.GetLevel <= Level)
            {
                SetMoves.Add(move.GetMove);
            }
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

        // Set Health Stats
        if (_baseVitality > 0)
        {
            _vitality = CalculateStat(_baseVitality);
            _currentVitality = _vitality;
        }

        if (_baseStamina > 0)
        {
            _stamina = CalculateStat(_baseStamina);
            _currentStamina = _stamina;
        }

        if (_baseReason > 0)
        {
            _reason = CalculateStat(_baseReason);
            _currentReason = _reason;
        }

        // Set Battle Stats
        if (_baseStrength > 0)
        {
            _strength = CalculateStat(_baseStrength);
        }

        if (_baseToughness > 0)
        {
            _toughness = CalculateStat(_baseToughness);
        }

        if (_baseAgility > 0)
        {
            _agility = CalculateStat(_baseAgility);
        }

        if (_basePower > 0)
        {
            _power = CalculateStat(_basePower);
        }

        if (_baseLuck > 0)
        {
            _luck = CalculateStat(_baseLuck);
        }

        if (_baseEvasion > 0)
        {
            _evasion = CalculateStat(_baseEvasion);
        }
    }

    #region Critter Level Functions

    public int CalculateExpCheckpoint(int level) // for calculating the Exp Checkpoint needed to reach next level
    {
        // Player Level Sequence Patterns

        // Slow Growth Rate: y = 6 * ((x * x) + 1)
        // Medium Growth Rate: y = 5 * ((x * x) + 1) or 10 + 15(x - 1) + 5(x - 1)(x - 2)
        // Fast Growth Rate: y = 4 * ((x * x) + 1)

        // Sample Table (S/M/F)
        // 1: 12/10/8 exp to reach lv 2
        // 2: 30/25/20 exp to reach lv 3
        // 3: 60/50/40 ...
        // 4: 102/85/68 ...
        // 5: 156/130/104 ...


        if (level > 0 && level < 25) // can't be level 0 or 25
        {
            return _growthValue * ((int)Mathf.Pow(level, 2) + 1); // 10 + (15 * (level - 1)) + (5 * (level - 1) * (level - 2));
        }
        else
        {
            return 0;
        }
    }

    public int CalculateCumulativeExpCheckpoint(int level) // for calculating the total Exp needed to reach next level
    {
        // Player Level Total Cumulative Patterns

        // Slow Cumulative: y = (6 / 6 ) * ((2 * x * x * x) + (3 * x * x) + (7 * x))
        // Medium Cumulative: y = (5 / 6) * ((2 * x * x * x) + (3 * x * x) + (7 * x))
        // Medium Cumulative: y = (4 / 6) * ((2 * x * x * x) + (3 * x * x) + (7 * x))

        // Sample Table (S/M/F)
        // 1: 12/10/8 total exp to reach lv 2
        // 2: 42/35/28 total exp to reach lv 3
        // 3: 102/85/68 ...
        // 4: 204/170/136 ...
        // 5: 360/300/240 ...

        if (level > 0 && level < 25) // can't be level 0 or 25
        {
            return _growthValue * ((2 * (int)Mathf.Pow(level, 3)) + (3 * (int)Mathf.Pow(level, 2)) + (7 * level)) / 6;
        }
        else
        {
            return 0;
        }
    }

    public int CalculateExpNeeded() // for calculating the remaining Exp needed to reach the next level
    {
        return CalculateCumulativeExpCheckpoint(_level) - _expPoints;
    }

    public void AddExp(int exp)
    {
        _expPoints += exp;

        while (_expPoints >= CalculateCumulativeExpCheckpoint(_level)) // Keep levelling up until the current experience is lower than the exp checkpoint
        {
            LevelUp();
        }
    } // TBA

    public void LevelUp()
    {
        _level++;

        Debug.Log("Level Up! Reached Level " + _level + "!");
    } // TBA

    #endregion

    public int CalculateStat(int stat)
    {
        return (int)Mathf.Floor( (stat * Mathf.Pow(1.05f, Level)) + Level ); // each Stat increases by 1/20th its current amount rounded down.
    }

    #region Critter Limb Functions

    public bool CheckLimb(MoveData move)
    {
        foreach (Limb limb in limbs)
        {
            if (move.Limb == (int)limb)
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckLimbSlots()
    {
        if (LearnedMoves.Count > 4)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddLimb(MoveData move)
    {
        LearnedMoves.Add(move);
    }

    public void RemoveLimb(int move)
    {
        LearnedMoves.RemoveAt(move);
    }

    #endregion
}

[Serializable] // Allows this to be displayed in the inspector (requires using System)
public class LevelMoves
{
    [SerializeField] private int level;
    [SerializeField] private MoveData move;

    public int GetLevel { get { return level; } }
    public MoveData GetMove { get { return move; } }
}