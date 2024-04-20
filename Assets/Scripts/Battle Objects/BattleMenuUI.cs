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

    [Header("Turn")]
    [SerializeField] private GameObject turnNumber;

    [Header("Critter Order")]
    [SerializeField] private GameObject[] orderMarkers; // Player side only
    [SerializeField] private GameObject[] orderNumbers; // All critter slots

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

    public void SetTurnNumber(int turn)
    {
        turnNumber.GetComponent<TextMeshProUGUI>().text = "Turn " + turn.ToString();
    }

    public void UnmarkOrderMarkers()
    {
        foreach (GameObject marker in orderMarkers)
        {
            marker.SetActive(false);
        }

        orderMarkers[0].SetActive(true);
    }

    public void DisplayOrderMarker(int ally)
    {
        orderMarkers[ally].SetActive(true);
    }

    public void HideOrderMarker(int ally)
    {
        orderMarkers[ally].SetActive(false);
    }

    public void UnmarkOrderNumbers()
    {
        foreach (GameObject number in orderNumbers)
        {
            number.SetActive(false);
        }
    }

    public void MarkOrderNumber(int number, int order)
    {
        orderNumbers[number].SetActive(true);
        orderNumbers[number].GetComponent<TextMeshProUGUI>().text = order.ToString();
    }
}