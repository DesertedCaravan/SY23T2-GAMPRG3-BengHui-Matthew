using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;

    public TextMeshProUGUI NameText => nameText;

    public void SetData(InventorySlots slot)
    {
        nameText.text = slot.GetItem.Name;
        amountText.text = $"x {slot.GetAmount}";
    }
}