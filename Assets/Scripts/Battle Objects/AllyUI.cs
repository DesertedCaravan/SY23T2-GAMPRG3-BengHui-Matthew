using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AllyUI : MonoBehaviour
{
    [Header("Ally Main Info")]
    [SerializeField] private GameObject allyInfo;
    [SerializeField] private TextMeshProUGUI allyName;
    [SerializeField] private TextMeshProUGUI allyLevel;

    [Header("Ally Health Stats")]
    [SerializeField] private TextMeshProUGUI vitalityStat;
    [SerializeField] private TextMeshProUGUI staminaStat;
    [SerializeField] private TextMeshProUGUI reasonStat;

    [SerializeField] private Slider vitalitySlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider reasonSlider;

    [Header("Stat Holder")]
    [SerializeField] private BattleStatHolder statHolder;

    public GameObject AllyInfo => allyInfo;

    public void SetAllyData()
    {
        allyName.text = statHolder.GetName;
        allyLevel.text = "LV" + statHolder.GetLevel.ToString();

        vitalityStat.text = statHolder.GetVitality.ToString() + "/" + statHolder.GetMaxVitality.ToString();
        staminaStat.text = statHolder.GetStamina.ToString() + "/" + statHolder.GetMaxStamina.ToString();
        reasonStat.text = statHolder.GetReason.ToString() + "/" + statHolder.GetReason.ToString();

        vitalitySlider.maxValue = statHolder.GetMaxVitality;
        vitalitySlider.value = statHolder.GetVitality;

        staminaSlider.maxValue = statHolder.GetMaxStamina;
        staminaSlider.value = statHolder.GetStamina;

        reasonSlider.maxValue = statHolder.GetMaxReason;
        reasonSlider.value = statHolder.GetReason;
    }
}