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

    public void SetCritterSlotData(CritterSlot slot)
    {
        if (slot.BaseData.Sprite != null)
        {
            critterIcon.sprite = slot.BaseData.Sprite;
        }

        critterName.text = slot.Name;

        critterLevel.text = "LV" + slot.Level.ToString();
        expTotal.text = "Exp Points: " + slot.ExpPoints.ToString();
        expNext.text = "To Next LV: " + slot.CalculateExpNeeded().ToString();

        vitalityStat.text = slot.CurrentVitality.ToString() + "/" + slot.Vitality.ToString();
        staminaStat.text = slot.CurrentStamina.ToString() + "/" + slot.Stamina.ToString();
        reasonStat.text = slot.CurrentReason.ToString() + "/" + slot.Reason.ToString();

        vitalitySlider.maxValue = slot.Vitality;
        vitalitySlider.value = slot.CurrentVitality;

        staminaSlider.maxValue = slot.Stamina;
        staminaSlider.value = slot.CurrentStamina;

        reasonSlider.maxValue = slot.Reason;
        reasonSlider.value = slot.CurrentReason;

        strengthStat.text = slot.Strength.ToString();
        toughnessStat.text = slot.Toughness.ToString();
        agilityStat.text = slot.Agility.ToString();
        powerStat.text = slot.Power.ToString();
        luckStat.text = slot.Luck.ToString();
        evasionStat.text = slot.Evasion.ToString();
    }
}
