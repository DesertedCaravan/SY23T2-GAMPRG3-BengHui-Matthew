using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CritterUI : MonoBehaviour
{
    [Header("Critter Main Info")]
    [SerializeField] private GameObject critterInfo;
    [SerializeField] private Image critterIcon;
    [SerializeField] private TextMeshProUGUI critterName;

    [Header("Critter Level Info")]
    [SerializeField] private TextMeshProUGUI critterLevel;
    [SerializeField] private TextMeshProUGUI expTotal;
    [SerializeField] private TextMeshProUGUI expNext;

    [Header("Critter Health Stats")]
    [SerializeField] private TextMeshProUGUI vitalityStat;
    [SerializeField] private TextMeshProUGUI staminaStat;
    [SerializeField] private TextMeshProUGUI reasonStat;

    [SerializeField] private Slider vitalitySlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider reasonSlider;

    [Header("Critter Combat Stats")]
    [SerializeField] private TextMeshProUGUI strengthStat;
    [SerializeField] private TextMeshProUGUI toughnessStat;
    [SerializeField] private TextMeshProUGUI agilityStat;
    [SerializeField] private TextMeshProUGUI powerStat;
    [SerializeField] private TextMeshProUGUI luckStat;
    [SerializeField] private TextMeshProUGUI evasionStat;

    public GameObject CritterInfo => critterInfo;

    public void SetData(CritterData data)
    {
        if (data.Sprite != null)
        {
            critterIcon.sprite = data.Sprite;
        }

        critterName.text = data.Name;

        critterLevel.text = "L" + data.Level.ToString();
        expTotal.text = "Exp Points: " + data.ExpPoints.ToString();
        expNext.text = "To Next LV: " + data.CalculateExpNeeded().ToString();

        vitalityStat.text = data.CurrentVitality.ToString() + "/" + data.Vitality.ToString();
        staminaStat.text = data.CurrentStamina.ToString() + "/" + data.Stamina.ToString();
        reasonStat.text = data.CurrentReason.ToString() + "/" + data.Reason.ToString();

        vitalitySlider.maxValue = data.Vitality;
        vitalitySlider.value = data.CurrentVitality;

        staminaSlider.maxValue = data.Stamina;
        staminaSlider.value = data.CurrentStamina;

        reasonSlider.maxValue = data.Reason;
        reasonSlider.value = data.CurrentReason;

        strengthStat.text = data.Strength.ToString();
        toughnessStat.text = data.Toughness.ToString();
        agilityStat.text = data.Agility.ToString();
        powerStat.text = data.Power.ToString();
        luckStat.text = data.Luck.ToString();
        evasionStat.text = data.Evasion.ToString();
    }
}
