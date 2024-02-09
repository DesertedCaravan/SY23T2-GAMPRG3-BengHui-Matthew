using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : InteractableObject
{
    [Header("Sprite Changer")]
    public SpriteRenderer m_SpriteRenderer;
    public Sprite usedSprite;

    [Header("First Interaction")]
    public bool firstInteractDataChange;

    protected override void OnFirstInteract()
    {
        Debug.Log("INTERACT WITH " + name);

        if (firstInteractDataChange)
        {
            GetItem();
        }
    }

    public override void PlayerDataChange()
    {
        // base.PlayerDataChange(change);

        GetItem();
    }

    public virtual void GetItem()
    {
        m_SpriteRenderer.sprite = usedSprite;

        // InventoryManager.instance.AddItem();
        Debug.Log("Game Data Changed!");
    }
}
