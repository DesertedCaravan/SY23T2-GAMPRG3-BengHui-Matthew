using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleMenuUI : MonoBehaviour
{
    [Header("Ally/Enemy Row")]
    [SerializeField] private GameObject[] SpritePanels;
    [SerializeField] private GameObject[] StatPanels;

    public GameObject[] GetStatPanels { get { return StatPanels; } }

    public void ActivateSprite(CritterSlot slot, int pos)
    {
        SpritePanels[pos].gameObject.SetActive(true);
        SpritePanels[pos].GetComponent<Image>().sprite = slot.BaseData.Sprite;

        if (pos >= 0 && pos <= 3)
        {
            StatPanels[pos].GetComponent<BattleStatHolder>().SetData(slot);
            StatPanels[pos].GetComponent<AllyUI>().SetAllyData();
        }
        else if (pos >= 4 && pos <= 7)
        {
            StatPanels[pos].GetComponent<BattleStatHolder>().SetData(slot);
            StatPanels[pos].GetComponent<EnemyUI>().SetEnemyData();
        }
    }

    public void CloseSprite(int pos)
    {
        SpritePanels[pos].gameObject.SetActive(false);
        StatPanels[pos].gameObject.SetActive(false);
    }

    public void UpdateSprite(int pos)
    {
    }
}