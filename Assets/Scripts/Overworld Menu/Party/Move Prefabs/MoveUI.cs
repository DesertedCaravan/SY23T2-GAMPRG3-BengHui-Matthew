using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI staminaCostText;
    [SerializeField] private TextMeshProUGUI reasonCostText;

    public TextMeshProUGUI NameText => nameText;

    public void SetData(MoveData move)
    {
        nameText.text = move.Name;
        staminaCostText.text = $"{move.StaminaCost}";
        reasonCostText.text = $"{move.ReasonCost}";
    }
}
