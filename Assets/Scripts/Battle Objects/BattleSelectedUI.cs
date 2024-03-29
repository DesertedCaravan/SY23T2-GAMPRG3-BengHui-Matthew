using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSelectedUI : MonoBehaviour
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

    [SerializeField] private GameObject itemSprite;

    [SerializeField] private TextMeshProUGUI moveDamageCategory;
    [SerializeField] private TextMeshProUGUI moveType;
    [SerializeField] private TextMeshProUGUI moveLimb;

    [SerializeField] private GameObject divider1;

    [SerializeField] private TextMeshProUGUI moveTargetName;
    [SerializeField] private TextMeshProUGUI moveDamageName;
    [SerializeField] private TextMeshProUGUI moveAccuracyName;
    [SerializeField] private TextMeshProUGUI moveSaveName;

    [SerializeField] private TextMeshProUGUI moveTarget;
    [SerializeField] private TextMeshProUGUI moveDamage;
    [SerializeField] private TextMeshProUGUI moveAccuracy;
    [SerializeField] private TextMeshProUGUI moveSave;

    [SerializeField] private GameObject divider2;

    [SerializeField] private TextMeshProUGUI moveDescription;
    public void SetMoveData(MoveData move)
    {
        itemSprite.SetActive(false);

        moveDamageCategory.text = move.Name;

        MoveDamageCategory category = (MoveDamageCategory)move.DamageCategory;

        moveDamageCategory.text = $"{category}";

        moveType.text = SetType(move.Type);
        moveLimb.text = SetLimb(move.Limb);

        divider1.SetActive(true);

        moveTargetName.text = "Target:";
        moveDamageName.text = "Damage:";
        moveAccuracyName.text = "Accuracy:";
        moveSaveName.text = "Save:";

        MoveTarget target = (MoveTarget)move.Target;
        MoveSave save = (MoveSave)move.Save;

        moveTarget.text = $"{target}";
        moveDamage.text = $"{move.Damage}%";
        moveAccuracy.text = $"{move.Accuracy}%";
        moveSave.text = $"{save}";

        divider2.SetActive(true);

        moveDescription.text = move.Info;
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

    public void SetStockData(ItemBase item)
    {
        ClearData();

        itemSprite.SetActive(true);

        moveDescription.text = item.Description;
    }

    public void ClearData()
    {
        itemSprite.SetActive(false);

        moveDamageCategory.text = "";

        moveType.text = "";
        moveLimb.text = "";

        divider1.SetActive(false);

        moveTargetName.text = "";
        moveDamageName.text = "";
        moveAccuracyName.text = "";
        moveSaveName.text = "";

        moveTarget.text = "";
        moveDamage.text = "";
        moveAccuracy.text = "";
        moveSave.text = "";

        divider2.SetActive(false);

        moveDescription.text = "";
    }
}
