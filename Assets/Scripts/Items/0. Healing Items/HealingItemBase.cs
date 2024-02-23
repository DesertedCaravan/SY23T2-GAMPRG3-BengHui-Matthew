using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Inventory/Healing Item")]
public class HealingItemBase : ItemBase
{
    public enum AllyTarget
    {
        OneAlly,
        AllAllies,
        PHandler
    }

    [SerializeField] private AllyTarget allyTarget;

    [Header("Vitality")]
    [SerializeField] private int vitalityGain;
    [SerializeField] private bool maxVitality;

    [Header("Stamina")]
    [SerializeField] private int staminaGain;
    [SerializeField] private bool maxStamina;

    [Header("Reason")]
    [SerializeField] private int reasonGain;
    [SerializeField] private bool maxReason;

    [Header("Allow Revival")]
    [SerializeField] private bool halfRev;
    [SerializeField] private bool maxRev;

    public AllyTarget ChosenAllyTarget => allyTarget;
    public int VitalityGain => vitalityGain;
    public bool MaxVitality => maxVitality;
    public int StaminaGain => staminaGain;
    public bool MaxStamina => maxStamina;
    public int ReasonGain => reasonGain;
    public bool MaxReason => maxReason;
    public bool HalfRev => halfRev;
    public bool MaxRev => maxRev;

    public override void OnValidate()
    {
        base.OnValidate();

        if (halfRev)
        {
            maxRev = false;
        }
        
        if (maxRev)
        {
            halfRev = false;
        }
    }
}
