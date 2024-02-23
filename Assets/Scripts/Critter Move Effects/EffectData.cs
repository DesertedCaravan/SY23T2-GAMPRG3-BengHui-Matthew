using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect Data", menuName = "Move Creation/Effect Data")]
public class EffectData : ScriptableObject
{
    public enum EffectTarget
    {
        Self,
        AllAllyCritters,
        PHandler,
        TargetEnemy,
        AllEnemyCritters,
        EHandler,
        All
    }

    public enum EffectType
    {
        StatStageChange,
        StatusCondition,
        SwitchRule
    }

    public enum EffectStat
    {
        Strength,
        Toughness,
        Agility,
        Power,
        Luck,
        Evasion,
        Accuracy,
        None
    }

    public enum EffectStatus
    {
        None
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

    [Header("Effect Data")]
    [SerializeField] private int _chance;

    [SerializeField] private int _duration;

    public int Target { get { return _target; } }
    public int Type { get { return _type; } }
    public int Stat { get { return _stat; } }

    public int ChangeStat { get { return _stat; } }
    public int StatStage { get { return _statStageChange; } }
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