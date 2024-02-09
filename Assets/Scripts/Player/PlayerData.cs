using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName ="Player Creation/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player Info Data")]
    public string _playerName;

    public Vector3 currentPosition;

    [Header("Player Level Stats")]
    public int _level;
    public int _experience; // current exp
    public int _expCheckNextLevel; // exp checkpoint needed to reach next level
    public int _expLeftNextLevel; // exp left to reach next level

    [Header("Player Battle Stats")]
    public int _vitality;
    public int _maxVitality;

    public int _stamina;
    public int _maxStamina;

    public int _reason;
    public int _maxReason;

    public int _strength;
    public int _toughness;
    public int _agility;
    public int _power;
    public int _luck;
    public int _evasion;

    #region Player Level Functions
    public void CalculateExpCheckpoint()
    {
        // Player Level Sequence Pattern: y = 10 + 15(x - 1) + 5(x - 1)(x - 2)
        // 1: 10 exp to reach lv 2
        // 2: 25 exp to reach lv 3
        // 3: 50 ...
        // 4: 85 ...
        // ...

        if (_level > 0 && _level < 25)
        {
            _expCheckNextLevel = 10 + (15 * (_level - 1)) + (5 * (_level - 1) * (_level - 2));
        }
        else
        {
            _expCheckNextLevel = 0; // can't be level 0 or 25
        }
    }

    public void CalculateExpLeft()
    {
        _expLeftNextLevel = _experience - _expCheckNextLevel;
    }

    public void AddExp(int exp)
    {
        _experience += exp;

        while (_experience >= _expCheckNextLevel) // Keep levelling up until the current experience is lower than the exp checkpoint
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        _level++;

        Debug.Log("Level Up! Reached Level " + _level + "!");
    }

    #endregion
}