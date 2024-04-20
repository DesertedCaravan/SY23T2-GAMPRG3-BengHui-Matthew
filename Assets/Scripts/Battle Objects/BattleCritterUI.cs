using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCritterUI : MonoBehaviour
{
    [Header("Battle Criter Main Info")]
    [SerializeField] private GameObject critterInfo;
    [SerializeField] private TextMeshProUGUI critterName;
    [SerializeField] private TextMeshProUGUI critterLevel;

    [Header("Battle Critter Health Stats")]
    [SerializeField] private TextMeshProUGUI vitalityStat;
    [SerializeField] private TextMeshProUGUI staminaStat;
    [SerializeField] private TextMeshProUGUI reasonStat;

    [SerializeField] private Slider vitalitySlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider reasonSlider;

    [Header("Check If Ally")]
    [SerializeField] private bool isAlly;
    public GameObject CritterInfo => critterInfo;

    public void SetupBattleCritterData(BattleStatusHolder statHolder)
    {
        critterName.text = statHolder.GetName;
        critterLevel.text = "LV" + statHolder.GetLevel.ToString();

        if (isAlly)
        {
            vitalityStat.text = statHolder.GetVitality.ToString() + "/" + statHolder.GetMaxVitality.ToString();
            staminaStat.text = statHolder.GetStamina.ToString() + "/" + statHolder.GetMaxStamina.ToString();
            reasonStat.text = statHolder.GetReason.ToString() + "/" + statHolder.GetMaxReason.ToString();
        }

        vitalitySlider.maxValue = statHolder.GetMaxVitality;
        vitalitySlider.value = statHolder.GetVitality;

        staminaSlider.maxValue = statHolder.GetMaxStamina;
        staminaSlider.value = statHolder.GetStamina;

        reasonSlider.maxValue = statHolder.GetMaxReason;
        reasonSlider.value = statHolder.GetReason;
    }

    public void SetVitalityData(BattleStatusHolder statHolder)
    {
        if (isAlly)
        {
            vitalityStat.text = statHolder.GetVitality.ToString() + "/" + statHolder.GetMaxVitality.ToString();
        }

        vitalitySlider.maxValue = statHolder.GetMaxVitality;
        vitalitySlider.value = statHolder.GetVitality;
    }

    public void SetStaminaData(BattleStatusHolder statHolder)
    {
        if (isAlly)
        {
            staminaStat.text = statHolder.GetStamina.ToString() + "/" + statHolder.GetMaxStamina.ToString();
        }

        staminaSlider.maxValue = statHolder.GetMaxStamina;
        staminaSlider.value = statHolder.GetStamina;
    }

    public void SetReasonData(BattleStatusHolder statHolder)
    {
        if (isAlly)
        {
            reasonStat.text = statHolder.GetReason.ToString() + "/" + statHolder.GetReason.ToString();
        }

        reasonSlider.maxValue = statHolder.GetMaxReason;
        reasonSlider.value = statHolder.GetReason;
    }
}