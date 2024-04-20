using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Critter Slot", menuName = "Critter Creation/Critter Slot")]
public class CritterSlot : ScriptableObject
{
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

    [Header("Base Data")]
    [SerializeField] private CritterData _data;

    [Header("Modifiable Data")]
    [SerializeField] private string _name;
    [SerializeField] private int _level;
    [SerializeField] private int _expPoints;

    [Header("Health Stats")]
    // Calculated Health Stats
    [SerializeField] private int _vitality;
    [SerializeField] private int _stamina;
    [SerializeField] private int _reason;

    [SerializeField] private int _currentVitality;
    [SerializeField] private int _currentStamina;
    [SerializeField] private int _currentReason;

    [Header("Battle Stats")]
    // Calculated Battle Stats
    [SerializeField] private int _strength;
    [SerializeField] private int _toughness;
    [SerializeField] private int _agility;
    [SerializeField] private int _power;
    [SerializeField] private int _luck;
    [SerializeField] private int _evasion;

    [Header("Known Limbs")]
    [SerializeField] private List<Limb> setLimbs;
    private List<int> _setLimbValues;

    [SerializeField] private List<Limb> learnedLimbs;
    private List<int> _learnedLimbValues;

    [Header("Known Moves")]
    [SerializeField] private List<MoveData> setMoves;
    [SerializeField] private List<MoveData> learnedMoves;

    // Public Pointers
    public CritterData BaseData => _data;
    public string Name => _name;
    public int Level => _level;
    public int ExpPoints => _expPoints;

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

    public List<int> SetLimbValues => _setLimbValues;
    public List<int> LearnedLimbValues => _learnedLimbValues;

    public List<MoveData> SetMoves => setMoves; // must consider limbs
    public List<MoveData> LearnedMoves => learnedMoves; // must consider limbs

    public void OnValidate()
    {
        if (_data != null) // Ensure that there is something in the _data variable first
        {
            _vitality = CalculateStat(_data.BaseVitality);
            _stamina = CalculateStat(_data.BaseStamina);
            _reason = CalculateStat(_data.BaseReason);

            _currentVitality = _vitality;
            _currentStamina = _stamina;
            _currentReason = _reason;

            _strength = CalculateStat(_data.BaseStrength);
            _toughness = CalculateStat(_data.BaseToughness);
            _agility = CalculateStat(_data.BaseAgility);
            _power = CalculateStat(_data.BasePower);
            _luck = CalculateStat(_data.BaseLuck);
            _evasion = CalculateStat(_data.BaseEvasion);

            ///////////////////////////

            _setLimbValues = new List<int>();

            // Copy Limbs from Data to Slot
            if (_data.LimbValues != null) // Bug Fix
            {
                for (int i = 0; i < _data.LimbValues.Count; i++)
                {
                    _setLimbValues.Add(_data.LimbValues[i]);
                }
            }

            // Fill up setLimbs List with corresponding limbs
            setLimbs = new List<Limb>();

            foreach (int limbEnum in _setLimbValues)
            {
                setLimbs.Add((Limb)limbEnum);
            }

            ///////////////////////////

            // Fill up learnedLimbValues List with corresponding integers
            _learnedLimbValues = new List<int>();

            for (int i = 0; i < learnedLimbs.Count; i++)
            {
                _learnedLimbValues.Add((int)learnedLimbs[i]);
            }

            ///////////////////////////

            // Fill up setMoves List with corresponding moves
            setMoves = new List<MoveData>();

            foreach (LevelMoves move in _data.LevelMoves)
            {
                if (move.GetLevel <= Level)
                {
                    SetMoves.Add(move.GetMove);
                }
            }

            ///////////////////////////

            // Check and make sure LearnedMoves reflects the known Limbs
            foreach (MoveData move in LearnedMoves)
            {
                bool limbChecker = false;

                foreach (Limb limb in setLimbs)
                {
                    if (move.Limb == (int)limb)
                    {
                        limbChecker = true;
                    }
                }

                foreach (Limb limb in learnedLimbs)
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
            return _data.GrowthValue * ((int)Mathf.Pow(level, 2) + 1); // 10 + (15 * (level - 1)) + (5 * (level - 1) * (level - 2));
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
            return _data.GrowthValue * ((2 * (int)Mathf.Pow(level, 3)) + (3 * (int)Mathf.Pow(level, 2)) + (7 * level)) / 6;
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
        if (stat > 0)
        {
            return (int)Mathf.Floor((stat * Mathf.Pow(1.05f, Level)) + Level); // each Stat increases by 1/20th its current amount rounded down.
        }
        else
        {
            return 0;
        }
    }

    #region Critter Limb Functions

    public bool CheckLimb(MoveData move)
    {
        foreach (Limb limb in setLimbs)
        {
            if (move.Limb == (int)limb)
            {
                return true;
            }
        }

        foreach (Limb limb in learnedLimbs)
        {
            if (move.Limb == (int)limb)
            {
                return true;
            }
        }

        return false;
    }

    public void AddMove(MoveData move)
    {
        LearnedMoves.Add(move);
    }

    public void RemoveMove(int move)
    {
        LearnedMoves.RemoveAt(move);
    }

    public int GetStat(int stat)
    {
        if (stat == 1)
        {
            return Strength;
        }
        else if (stat == 2)
        {
            return Toughness;
        }
        else if (stat == 3)
        {
            return Agility;
        }
        else if (stat == 4)
        {
            return Power;
        }
        else if (stat == 5)
        {
            return Luck;
        }
        else if (stat == 6)
        {
            return Evasion;
        }
        else
        {
            return 0;
        }
    }

    #endregion
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