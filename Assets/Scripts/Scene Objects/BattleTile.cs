using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleTile : InteractableObject
{
    [Header("Encounter Data")]
    [SerializeField] private List<EncounterData> encounterList;

    protected override void OnCollided(GameObject collidedObject)
    {
        EncounterCheck();
    }

    protected virtual void EncounterCheck()
    {
        // OnInteract();
    }

    protected override void OnInteract() // So that it can be inherited
    {
        if (!_interacting && _interactedCheck == false)
        {
            _interacting = true;
            _interactedCheck = true;

            CheckResponseEvents(interactDialogue);

            dialogueBoxManager.StartDialogue(this, interactDialogue);

            OnFirstInteract();
        }
        else if (!_interacting && _interactedCheck == true)
        {
            _interacting = true;

            CheckResponseEvents(interactedDialogue);

            dialogueBoxManager.StartDialogue(this, interactedDialogue);

            OnLaterInteract();
        }
    }

    public override void CheckResponseEvents(DialogueText dialogueText)
    {
        // find DialogueEvent components attached to this Game Object and make sure that it matches
        foreach (DialogueEvent dialogueEvents in GetComponents<DialogueEvent>()) // Old Version: if(TryGetComponent(out DialogueEvent dialogueEvents))
        {
            if (dialogueEvents.DialogueText == dialogueText)
            {
                dialogueBoxManager.AddResponseEvents(dialogueEvents.Events);
                break;
            }
        }
    }

    protected override void OnFirstInteract()
    {
        Debug.Log("INTERACT WITH " + name);
    }

    public override void OnEndInteract() // Occurs only after the dialogue has concluded
    {
        Debug.Log("INTERACT WITH " + name);
    }

    protected override void OnLaterInteract()
    {
        Debug.Log("ALREADY INTERACTED WITH " + name);
    }
}

[Serializable] // Allows this to be displayed in the inspector (requires using System)
public class EncounterData
{
    [SerializeField] private CritterData critter;
    [SerializeField] private int _percentage;

    public CritterData Critter { get { return critter; } }
    public int Percentage { get { return _percentage; } }
}