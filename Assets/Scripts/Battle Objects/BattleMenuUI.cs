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

    [Header("Turn Order")]
    [SerializeField] private GameObject[] turnMarkers; // Player side only
    [SerializeField] private GameObject[] turnNumbers; // All critter slots

    public GameObject[] GetStatPanels { get { return StatPanels; } }

    public void ActivateSprite(CritterSlot slot, int pos)
    {
        SpritePanels[pos].gameObject.SetActive(true);
        SpritePanels[pos].GetComponent<Image>().sprite = slot.BaseData.Sprite;

        if (pos >= 0 && pos <= 7)
        {
            StatPanels[pos].GetComponent<BattleStatusHolder>().SetStartingStatusData(slot);
        }
    }

    public void CloseSprite(int pos)
    {
        SpritePanels[pos].gameObject.SetActive(false);
        StatPanels[pos].gameObject.SetActive(false);
    }

    public void UnmarkTurnMarkers()
    {
        foreach (GameObject marker in turnMarkers)
        {
            marker.SetActive(false);
        }

        turnMarkers[0].SetActive(true);
    }

    public void DisplayTurnMarker(int ally)
    {
        turnMarkers[ally].SetActive(true);
    }

    public void HideTurnMarker(int ally)
    {
        turnMarkers[ally].SetActive(false);
    }

    public void UnmarkTurnNumbers()
    {
        foreach (GameObject number in turnNumbers)
        {
            number.SetActive(false);
        }
    }

    public void MarkTurnNumber(int number, int order)
    {
        turnNumbers[number].SetActive(true);
        turnNumbers[number].GetComponent<TextMeshProUGUI>().text = order.ToString();
    }
}