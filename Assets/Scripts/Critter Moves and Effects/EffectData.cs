using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Data", menuName = "Move Creation/Effect Data")]
public class EffectData : ScriptableObject
{
    public enum EffectTarget
    {
        Self,
        AllyRow,
        PHandler,
        Target,
        EnemyRow,
        EHandler,
        AllOthers,
        All
    }

    public enum EffectType
    {
        StatStageChange,
        StatusCondition,
        RegainHealth,
        SwitchRule
    }

    public enum EffectStat
    {
        None,
        Strength,
        Toughness,
        Agility,
        Power,
        Luck,
        Evasion,
        Accuracy,
    }

    public enum EffectStatus
    {
        None,
        Bleeding, // Anima
        Venomed, // Bio
        Ill, // Half Toughness
        Weakened, // Half Strength
        Afraid // Half Power
    }

    public enum EffectHealth
    {
        None,
        Vitality,
        Stamina,
        Reason
    }

    public enum EffectRule
    {
        None
    }

    [SerializeField] private EffectTarget target;
    private int _target;

    [SerializeField] private EffectType type;
    private int _type;

    [Header("Stat Stage Change")]
    [SerializeField] private EffectStat stat;
    private int _stat;

    [SerializeField] private int _statStageChange;

    [Header("Status Condition")]
    [SerializeField] private EffectStatus status;
    private int _status;

    [Header("Switch Rule")]
    [SerializeField] private EffectRule rule;
    private int _rule;

    [Header("Regain Health")]
    [SerializeField] private EffectHealth health;
    private int _health;

    [SerializeField] private int _healthChange;

    [Header("Effect Data")]
    [SerializeField] private int _chance;

    [SerializeField] private int _duration;

    public int Target { get { return _target; } }
    public int Type { get { return _type; } }
    public int Stat { get { return _stat; } }
    public int Health { get { return _health; } }

    public int ChangeStat { get { return _stat; } }
    public int StatStageChange { get { return _statStageChange; } }
    public int HealthChange { get { return _healthChange; } }

    public int ChangeStatus { get { return _status; } }
    public int ChangeRule { get { return _rule; } }

    public int Chance { get { return _chance; } }
    public int Duration { get { return _duration; } }

    public void OnValidate()
    {
        _target = (int)target;
        _type = (int)type;
        _stat = (int)stat;
        _status = (int)status;
        _rule = (int)rule;
        _health = (int)health;

        if (_chance < 0)
        {
            _chance = 0;
        }
        else if (_chance > 100)
        {
            _chance = 100;
        }

        if (stat == EffectStat.None)
        {
            _statStageChange = 0;
        }
        else if (_statStageChange == 0)
        {
            _statStageChange = 1;
        }

        if (_duration < 1)
        {
            _duration = 1;
        }
    }
}