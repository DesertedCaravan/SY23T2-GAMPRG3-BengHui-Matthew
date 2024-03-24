using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : CollidableObject
{
    [Header("Dialogue Box Manager Updater")]
    [SerializeField] protected DialogueBoxManager dialogueBoxManager;

    [Header("Dialogue Text")]
    [SerializeField] protected DialogueText interactDialogue;
    [SerializeField] protected DialogueText interactedDialogue;

    // Old Version
    // public string[] interactTextBoxes;
    // public string[] interactedTextBoxes;

    protected bool _interacting = false;
    protected bool _interactedCheck = false;

    protected override void Start()
    {
        base.Start();

        dialogueBoxManager = FindObjectOfType<DialogueBoxManager>(); // looks for the first one
    }

    protected override void OnCollided(GameObject collidedObject)
    {
        // base.OnCollided(collidedObject);

        // PlayerController playerController = collidedObject.GetComponent<PlayerController>();

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && PlayerController.instance.MovementState) // Key must be pressed down (not held) in order to call the function
        {
            OnInteract();
        }
    }

    protected virtual void OnInteract() // So that it can be inherited
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

    public virtual void CheckResponseEvents(DialogueText dialogueText)
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

    protected virtual void OnFirstInteract()
    {
        Debug.Log("INTERACT WITH " + name);
    }

    public virtual void OnEndInteract() // Occurs only after the dialogue has concluded
    {
        Debug.Log("INTERACT WITH " + name);
    }

    protected virtual void OnLaterInteract()
    {
        Debug.Log("ALREADY INTERACTED WITH " + name);
    }

    public virtual void AllowInteraction()
    {
        _interacting = false;
    }

    public virtual void ResetDialogue()
    {
        _interactedCheck = false;

        Debug.Log("DIALOGUE RESET");
    }
}
