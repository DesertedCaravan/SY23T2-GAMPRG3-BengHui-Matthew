using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : InteractableObject
{
    [Header("With Dialogue Text")]
    [SerializeField] private DialogueText interactWithDialogue;

    [Header("Sprite Changer")]
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite usedSprite;

    [Header("Item Needed")]
    [SerializeField] private ItemBase itemNeeded;
    [SerializeField] private int amountNeeded;

    [Header("Item Giver")]
    [SerializeField] private ItemBase itemGiven;
    [SerializeField] private int amountGiven;

    protected override void OnInteract()
    {
        if (!_interacting)
        {
            _interacting = true;

            if (InventoryManager.instance.CheckForItem(itemNeeded) == true)
            {
                CheckResponseEvents(interactWithDialogue);
                DialogueBoxManager.instance.StartDialogue(this, interactWithDialogue);
            }
            else
            {
                CheckResponseEvents(interactDialogue);
                DialogueBoxManager.instance.StartDialogue(this, interactDialogue);
            }
        }
    }

    public virtual void RemoveItem()
    {
        InventoryManager.instance.RemoveItem(itemNeeded, amountNeeded);

        Debug.Log("Game Data Changed!");
    }

    public virtual void GetItem()
    {
        InventoryManager.instance.AddItem(itemGiven, amountGiven);

        Debug.Log("Game Data Changed!");
    }
}
