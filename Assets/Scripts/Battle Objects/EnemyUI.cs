using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("Ememy Main Info")]
    [SerializeField] private GameObject enemyInfo;
    [SerializeField] private TextMeshProUGUI enemyName;
    [SerializeField] private TextMeshProUGUI enemyLevel;

    [Header("Ememy Health Stats")]

    [SerializeField] private Slider vitalitySlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Slider reasonSlider;

    [Header("Stat Holder")]
    [SerializeField] private BattleStatHolder statHolder;

    public GameObject EnemyInfo => enemyInfo;

    public void SetEnemyData()
    {
        enemyName.text = statHolder.GetName;
        enemyLevel.text = "LV" + statHolder.GetLevel.ToString();

        vitalitySlider.maxValue = statHolder.GetMaxVitality;
        vitalitySlider.value = statHolder.GetVitality;

        staminaSlider.maxValue = statHolder.GetMaxStamina;
        staminaSlider.value = statHolder.GetStamina;

        reasonSlider.maxValue = statHolder.GetMaxReason;
        reasonSlider.value = statHolder.GetReason;
    }
}
