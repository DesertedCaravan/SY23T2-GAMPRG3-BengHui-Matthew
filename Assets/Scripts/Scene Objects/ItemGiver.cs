using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : InteractableObject
{
    [Header("Inventory Manager Updater")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Sprite Changer")]
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Sprite usedSprite;

    [Header("Item Giver")]
    [SerializeField] private ItemBase item;
    [SerializeField] private int amountGiven;

    [Header("First Interaction")]
    [SerializeField] private bool firstInteractDataChange;

    protected override void Start()
    {
        base.Start();

        inventoryManager = FindObjectOfType<InventoryManager>(); // looks for the first one
    }

    protected override void OnFirstInteract()
    {
        Debug.Log("INTERACT WITH " + name);

        if (firstInteractDataChange)
        {
            GetItem();
        }
    }

    public virtual void GetItem()
    {
        m_SpriteRenderer.sprite = usedSprite;

        inventoryManager.AddItem(item, amountGiven);

        Debug.Log("Game Data Changed!");
    }
}
