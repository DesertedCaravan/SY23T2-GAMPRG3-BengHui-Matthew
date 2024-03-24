using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Move Data", menuName = "Move Creation/Move Data")]
public class MoveData : ScriptableObject
{
    public enum MoveDamageCategory
    {
        Physical,
        Magical
    }

    public enum MoveType
    {
        Anima,
        Vega,
        Psycho,
        Umbra,
        Thauma,
        Haunt,
        Gaunt,
        Bio,
        Auto,
        Giga,
        Thalassa,
        Inferna,
        Hyper,
        Celes,
        Luna
    }

    public enum MoveLimb
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

    public enum MoveTarget
    {
        Self,
        OneAlly,
        AllyRow,
        PHandler,
        OneEnemy,
        EnemyRow,
        EHandler,
        AllOthers,
        All
    }

    public enum MoveSave
    {
        Strength,
        Toughness,
        Agility,
        Power,
        Luck,
        Evasion,
        None
    }

    [Header("Move Info")]
    [SerializeField] private string _name;
    [SerializeField] [TextArea] private string _info;

    [Header("Move Category")]
    [SerializeField] private MoveDamageCategory damageCategory;
    private int _damageCategoryType;

    [SerializeField] private MoveType type;
    private int _typeType;

    [SerializeField] private MoveLimb limb;
    private int _limbType;

    [Header("Move Cost")]
    [SerializeField] private int _staminaCost;
    [SerializeField] private int _reasonCost;

    [Header("Move Effect")]
    [SerializeField] private MoveTarget target;
    private int _targetType;

    [SerializeField] private int _damage;
    [SerializeField] private int _accuracy;
    [SerializeField] private MoveSave save;
    private int _saveType;

    [SerializeField] private List<EffectData> effects;

    public string Name => _name;
    public string Info => _info;

    public int DamageCategory => _damageCategoryType;
    public int Type => _typeType;
    public int Limb => _limbType;

    public int StaminaCost => _staminaCost;
    public int ReasonCost => _reasonCost;

    public int Target => _targetType;
    public int Damage => _damage;
    public int Accuracy => _accuracy;
    public int Save => _saveType;
    public List<EffectData> Effects => effects;

    public void OnValidate()
    {
        _typeType = (int)type;
        _limbType = (int)limb;

        _damageCategoryType = (int)damageCategory;

        _targetType = (int)target;
        _saveType = (int)save;
        
        if (_damage < 0)
        {
            _damage = 0;
        }

        if (_accuracy < 0)
        {
            _accuracy = 0;
        }
    }
}