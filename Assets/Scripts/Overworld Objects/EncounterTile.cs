using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EncounterTile : InteractableObject
{
    [Header("Encounter Manager")]
    [SerializeField] private EncounterManager encounterManager;

    [Header("Encounter Data")]
    [SerializeField] private int encounterPercent;
    protected bool _playerWalk = false;

    [Header("Encounter Data")]
    [SerializeField] private List<EncounterData> encounterList;
    private EncounterData chosenEncounter;

    public void OnValidate()
    {
        if (encounterPercent < 1)
        {
            encounterPercent = 1;
        }
        else if (encounterPercent > 100)
        {
            encounterPercent = 100;
        }
    }

    protected override void Start()
    {
        base.Start();

        encounterManager = FindObjectOfType<EncounterManager>(); // looks for the first one
    }

    protected override void Update()
    {
        thisCollider.OverlapCollider(filter, collidedObjects);

        if (collidedObjects.Count > 0 && _playerWalk == false) // Activates when the player enters the tile.
        {
            _playerWalk = true;

            foreach (var o in collidedObjects) // Checks how many other Game Objects are currently colliding with this Game Object.
            {
                OnCollided(o.gameObject);
            }
        }
        
        if (collidedObjects.Count == 0 && _playerWalk == true) // Activates when the player leaves the tile.
        {
            _playerWalk = false;
        }
    }


    protected override void OnCollided(GameObject collidedObject)
    {
        int chosenPercent = UnityEngine.Random.Range(0, 101); // 1% - 100%

        if (chosenPercent <= encounterPercent) // must roll lower than encounterPercent
        {
            EncounterCheck();
        }
    }

    protected virtual void EncounterCheck()
    {
        int percentCounter = 0;
        int chosenPercent = UnityEngine.Random.Range(0, 101); // 1% - 100%

        for (int i = 0; i < encounterList.Count; i++)
        {
            percentCounter += encounterList[i].Percentage; // cumulative percentage from encounterList (combined total must always be 100%)

            if (chosenPercent <= percentCounter) // percentageCounter must be  equal or less than the roll
            {
                chosenEncounter = encounterList[i];
                OnInteract();
                break;
            }
        }
    }

    protected override void OnInteract() // So that it can be inherited
    {
        if (!_interacting)
        {
            _interacting = true;

            CheckResponseEvents(interactDialogue);
            dialogueBoxManager.StartDialogue(this, interactDialogue);
        }
    }

    public override void OnEndInteract() // Occurs only after the dialogue has concluded
    {
        Debug.Log("INTERACT WITH " + name);
        Debug.Log(chosenEncounter.EncounterName);

        encounterManager.StartEncounter(chosenEncounter.Critter1, chosenEncounter.Critter2, chosenEncounter.Critter3);
    }
}

[Serializable] // Allows this to be displayed in the inspector (requires using System)
public class EncounterData
{
    [SerializeField] private string encounterName;
    [SerializeField] private CritterSlot critter1;
    [SerializeField] private CritterSlot critter2;
    [SerializeField] private CritterSlot critter3;
    [SerializeField] private int _percentage;

    public string EncounterName { get { return encounterName; } }
    public CritterSlot Critter1 { get { return critter1; } }
    public CritterSlot Critter2 { get { return critter2; } }
    public CritterSlot Critter3 { get { return critter3; } }
    public int Percentage { get { return _percentage; } }
}