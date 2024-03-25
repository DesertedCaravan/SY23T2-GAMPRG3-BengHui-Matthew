using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SummaryUI : MonoBehaviour
{
    public enum Type
    {
        ANIMA,
        VEGA,
        BIO,
        PSYCHO,
        UMBRA,
        THAUMA,
        HAUNT,
        GAUNT,
        AUTO,
        GIGA,
        THALASSA,
        INFERNA,
        HYPER,
        CELES,
        LUNA,
        NONE
    }

    public enum MoveLimb
    {
        CEREBRUM,
        OCULUS,
        FEELER,
        HORN,
        FANG,
        JAW,
        CLAW,
        ARM,
        TORSO,
        LEG,
        TAIL
    }

    public enum MoveDamageCategory
    {
        Physical,
        Magical
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

    [Header("Critter Main Info")]
    [SerializeField] private Image critterIcon;
    [SerializeField] private TextMeshProUGUI critterName;
    [SerializeField] private TextMeshProUGUI critterSpeciesName;

    [SerializeField] private TextMeshProUGUI critterType1;
    [SerializeField] private TextMeshProUGUI critterType2;

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

    [Header("Critter Limbs")]
    [SerializeField] private List<Image> limbsList;

    [Header("Critter Moves")]
    [SerializeField] private GameObject moveListContent;
    [SerializeField] private MoveUI movePrefab;

    [SerializeField] private List<MoveData> movesListHolder;
    [SerializeField] private List<MoveUI> movesUIList;

    // [SerializeField] private TextMeshProUGUI moveName;
    [SerializeField] private TextMeshProUGUI moveType;
    [SerializeField] private TextMeshProUGUI moveLimb;
    [SerializeField] private TextMeshProUGUI moveDamageCategory;

    [SerializeField] private TextMeshProUGUI moveTargetName;
    [SerializeField] private TextMeshProUGUI moveSaveName;
    [SerializeField] private TextMeshProUGUI moveDamageName;
    [SerializeField] private TextMeshProUGUI moveAccuracyName;

    [SerializeField] private TextMeshProUGUI moveTarget;
    [SerializeField] private TextMeshProUGUI moveSave;
    [SerializeField] private TextMeshProUGUI moveDamage;
    [SerializeField] private TextMeshProUGUI moveAccuracy;

    [SerializeField] private TextMeshProUGUI moveDescription;

    public List<MoveData> MovesList => movesListHolder;
    public List<MoveUI> MovesUIList => movesUIList;

    public void SetCritterSlotData(CritterSlot slot)
    {
        if (slot.BaseData.Sprite != null)
        {
            critterIcon.sprite = slot.BaseData.Sprite;
        }

        critterName.text = slot.Name;
        critterSpeciesName.text = slot.BaseData.SpeciesName;

        critterType1.text = SetType(slot.BaseData.Type1);
        critterType2.text = SetType(slot.BaseData.Type2);

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

        foreach (Image limb in limbsList)
        {
            limb.color = new Color32(63, 63, 63, 100);
        }

        for (int i = 0; i < limbsList.Count; i++)
        {
            foreach (int limbValue in slot.SetLimbValues)
            {
                if (i == limbValue)
                {
                    limbsList[i].color = new Color32(255, 255, 225, 100);
                }
            }

            foreach (int limbValue in slot.LearnedLimbValues)
            {
                if (i == limbValue)
                {
                    limbsList[i].color = new Color32(255, 255, 225, 100);
                }
            }
        }

        movesListHolder = new List<MoveData>();

        for (int i = 0; i < slot.SetMoves.Count; i++)
        {
            movesListHolder.Add(slot.SetMoves[i]);
        }

        for (int i = 0; i < slot.LearnedMoves.Count; i++)
        {
            movesListHolder.Add(slot.LearnedMoves[i]);
        }
        
        // Clear all exisiting child Game Objects in the Move List Scrollview Content
        foreach (Transform child in moveListContent.transform)
        {
            Destroy(child.gameObject);
        }

        movesUIList = new List<MoveUI>(); // Make new temporary list // Alternative is movesUIList.Clear();

        foreach (var move in movesListHolder)
        {
            var moveDisplay = Instantiate(movePrefab, moveListContent.transform); // Instantiate Item Prefab
            moveDisplay.SetData(move); // Call SetData function from Instantiated Item Prefab, then change it's data by reading from the InventoryManager

            movesUIList.Add(moveDisplay); // Add to movesUI List
        }
    }

    public string SetType(int data)
    {
        Type type = (Type)data;
        return $"{type}";
    }

    public string SetLimb(int data)
    {
        MoveLimb limb = (MoveLimb)data;
        return $"{limb}";
    }


    public void SetMoveData(int data)
    {
        if (movesUIList.Count > 0)
        {
            // moveName.text = movesListHolder[data].Name;
            moveDamageCategory.text = movesListHolder[data].Name;

            MoveDamageCategory category = (MoveDamageCategory)movesListHolder[data].DamageCategory;

            moveDamageCategory.text = $"{category}";

            moveType.text = SetType(movesListHolder[data].Type);
            moveLimb.text = SetLimb(movesListHolder[data].Limb);

            moveTargetName.text = "Target:";
            moveSaveName.text = "Save:";
            moveDamageName.text = "Damage:";
            moveAccuracyName.text = "Accuracy:";

            MoveTarget target = (MoveTarget)movesListHolder[data].Target;
            MoveSave save = (MoveSave)movesListHolder[data].Save;

            moveTarget.text = $"{target}";
            moveSave.text = $"{save}";

            moveDamage.text = $"{movesListHolder[data].Damage}%";
            moveAccuracy.text = $"{movesListHolder[data].Accuracy}%";

            moveDescription.text = movesListHolder[data].Info;
        }
        else
        {
            // moveName.text = "";
            moveDamageCategory.text = "";

            moveType.text = "";
            moveLimb.text = "";

            moveTargetName.text = "";
            moveSaveName.text = "";
            moveDamageName.text = "";
            moveAccuracyName.text = "";

            moveTarget.text = "";
            moveSave.text = "";

            moveDamage.text = "";
            moveAccuracy.text = "";

            moveDescription.text = "";
        }
    }
}
