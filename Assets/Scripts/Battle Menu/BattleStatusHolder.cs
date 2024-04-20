using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleStatusHolder : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private int _level;

    [Header("Battle Health Stats")]
    [SerializeField] private int _vitality;
    [SerializeField] private int _stamina;
    [SerializeField] private int _reason;

    [SerializeField] private int _maxVitality;
    [SerializeField] private int _maxStamina;
    [SerializeField] private int _maxReason;

    [Header("Stat Modifiers")]
    [SerializeField] private List<TempStatModifiers> statModList;

    // Based on EffectData class
    // 1 - Strength
    // 2 - Toughness
    // 3 - Agility
    // 4 - Power
    // 5 - Luck
    // 6 - Evasion
    // 7 - Accuracy

    [Header("Battle Critter UI Updater")]
    [SerializeField] private BattleCritterUI critterUI;

    public string GetName { get { return _name; } }
    public string SetName { set { _name = value; } }

    public int GetLevel { get { return _level; } }
    public int SetLevel { set { _level = value; } }

    public int GetVitality { get { return _vitality; } }
    public int SetVitality { set { _vitality = value; } }

    public int GetStamina { get { return _stamina; } }
    public int SetStamina { set { _stamina = value; } }

    public int GetReason { get { return _reason; } }
    public int SetReason { set { _reason = value; } }

    public int GetMaxVitality { get { return _maxVitality; } }
    public int SetMaxVitality { set { _maxVitality = value; } }

    public int GetMaxStamina { get { return _maxStamina; } }
    public int SetMaxStamina { set { _maxStamina = value; } }

    public int GetMaxReason { get { return _maxReason; } }
    public int SetMaxReason { set { _maxReason = value; } }

    public void SetStartingStatusData(CritterSlot slot)
    {
        _name = slot.Name;
        _level = slot.Level;

        _vitality = slot.CurrentVitality;
        _stamina = slot.CurrentStamina;
        _reason = slot.CurrentReason;

        _maxVitality = slot.Vitality;
        _maxStamina = slot.Stamina;
        _maxReason = slot.Reason;

        critterUI.SetupBattleCritterData(this);
    }

    public void SetVitalityStatus(int change)
    {
        _vitality = _vitality + change;

        _vitality = Mathf.Clamp(_vitality, 0, _maxVitality);

        critterUI.SetVitalityData(this);
    }

    public void SetStaminaStatus(int change)
    {
        _stamina = _stamina + change;

        _stamina = Mathf.Clamp(_stamina, 0, _maxStamina);

        critterUI.SetStaminaData(this);
    }

    public void SetReasonStatus(int change)
    {
        _reason = _reason + change;

        _reason = Mathf.Clamp(_reason, 0, _maxReason);

        critterUI.SetReasonData(this);
    }

    public void AddEffect(EffectData effect)
    {
        TempStatModifiers newMod = new TempStatModifiers();

        newMod.SetEffectData = effect;
        newMod.SetStatModDuration = effect.Duration;

        statModList.Add(newMod);
    }

    public void ReduceAllEffectDuration()
    {
        foreach(TempStatModifiers statMod in statModList)
        {
            int duration = statMod.GetStatModDuration;
            duration--;

            statMod.SetStatModDuration = duration;
        }

        // Transfer Method
        List<TempStatModifiers> tempList = new List<TempStatModifiers>();

        foreach (TempStatModifiers statMod in statModList)
        {
            if (statMod.GetStatModDuration > 0)
            {
                tempList.Add(statMod);
            }
        }

        statModList = new List<TempStatModifiers>();

        foreach (TempStatModifiers statMod in tempList)
        {
            statModList.Add(statMod);
        }
    } 

    public float CheckStatMod(int stat) // for Stat Stage Change only
    {
        float _statModHolder = 0;

        if (statModList.Count == 0)
        {
            return 1.0f;
        }

        for (int i = 0; i < statModList.Count; i++)
        {
            if (statModList[i].GetEffectData.Type == 0 && statModList[i].GetEffectData.Stat == stat)
            {
                Debug.Log("Type detected!!!!!!!");
                _statModHolder += statModList[i].GetEffectData.StatStageChange;
            }
        }

        _statModHolder = Mathf.Clamp(_statModHolder, -6, 6);

        if (_statModHolder == 0)
        {
            return 1.0f;
        }
        else if (_statModHolder > 0)
        {
            return 0.5f + (_statModHolder * 0.5f);

            // 1: 1.5
            // 2: 2.0
            // 3: 2.5
            // 4: 3.0
            // 5: 3.5
            // 6: 4.0
        }
        else if (_statModHolder < 0)
        {
            return 2.0f / (2.0f - _statModHolder);

            // -1: 2/3
            // -2: 2/4
            // -3: 2/5
            // -4: 2/6
            // -5: 2/7
            // -6: 2/8
        }
        else
        {
            return 1.0f;
        }
    }
}

[Serializable]
public class TempStatModifiers
{
    [SerializeField] private EffectData _effectData;
    [SerializeField] private int _statModDuration;

    public EffectData GetEffectData { get { return _effectData; } }
    public EffectData SetEffectData { set { _effectData = value; } }
    public int GetStatModDuration { get { return _statModDuration; } }
    public int SetStatModDuration { set { _statModDuration = value; } }

    public void ReduceStatModDuration()
    {
        _statModDuration--;
    }
}