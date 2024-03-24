using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : InteractableObject
{
    [Header("Inventory Manager Updater")]
    [SerializeField] private InventoryManager inventoryManager;

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

    protected override void Start()
    {
        base.Start();

        inventoryManager = FindObjectOfType<InventoryManager>(); // looks for the first one
    }

    protected override void OnInteract()
    {
        if (!_interacting)
        {
            _interacting = true;

            if (inventoryManager.CheckForItem(itemNeeded) == true)
            {
                CheckResponseEvents(interactWithDialogue);
                dialogueBoxManager.StartDialogue(this, interactWithDialogue);
            }
            else
            {
                CheckResponseEvents(interactDialogue);
                dialogueBoxManager.StartDialogue(this, interactDialogue);
            }
        }
    }

    public virtual void RemoveItem()
    {
        inventoryManager.RemoveItem(itemNeeded, amountNeeded);

        Debug.Log("Game Data Changed!");
    }

    public virtual void GetItem()
    {
        inventoryManager.AddItem(itemGiven, amountGiven);

        Debug.Log("Game Data Changed!");
    }
}
