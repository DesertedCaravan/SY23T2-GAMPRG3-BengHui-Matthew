using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatHolder : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private int _level;

    [SerializeField] private int _vitality;
    [SerializeField] private int _stamina;
    [SerializeField] private int _reason;

    [SerializeField] private int _maxVitality;
    [SerializeField] private int _maxStamina;
    [SerializeField] private int _maxReason;

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

    public void SetData(CritterSlot slot)
    {
        _name = slot.Name;
        _level = slot.Level;

        _vitality = slot.CurrentVitality;
        _stamina = slot.CurrentStamina;
        _reason = slot.CurrentReason;

        _maxVitality = slot.Vitality;
        _maxStamina = slot.Stamina;
        _maxReason = slot.Reason;
    }
}
