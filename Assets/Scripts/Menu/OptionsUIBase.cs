using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUIBase : MonoBehaviour
{
    [Header("Base Menu Option UI")]
    [SerializeField] protected MenuUI menuUI;

    [Header("Option Selection Text")]
    [SerializeField] protected DialogueText optionSelectDialogue;

    protected bool _menuOptionActiveTrigger; // control _menuOptionActive
    protected bool _menuOptionActive;
    protected bool _optionSelected;

    public bool SetMenuOptionActive { set { _menuOptionActive = value; } } // for menu Option UIs
    public bool SetOptionSelected { set { _optionSelected = value; } } // for Dialogue Box Manager

    protected virtual void Start()
    {
        _menuOptionActiveTrigger = false;
        _menuOptionActive = false;
        _optionSelected = false;
    }

    public void CheckResponseEvents(DialogueText dialogueText)
    {
        // find DialogueEvent components attached to this Game Object and make sure that it matches
        foreach (DialogueEvent dialogueEvents in GetComponents<DialogueEvent>()) // Old Version: if(TryGetComponent(out DialogueEvent dialogueEvents))
        {
            if (dialogueEvents.DialogueText == dialogueText)
            {
                DialogueBoxManager.instance.AddResponseEvents(dialogueEvents.Events);
                break;
            }
        }
    }

    protected virtual void CloseMenuOptionPanel()
    {
        menuUI.OptionOpenState = false; // allow menu to become interactable again

        menuUI.MenuOptionsPanelToggle.SetActive(true); // Open the menu option panel
    }
}
