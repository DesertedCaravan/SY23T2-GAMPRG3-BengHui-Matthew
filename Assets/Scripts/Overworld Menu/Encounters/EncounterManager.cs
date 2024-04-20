using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For SceneManager

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerControllerRef;
    [SerializeField] private PartyManager partyManager;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private List<CritterSlot> battleParticipants; // must be exposed

    public List<CritterSlot> BattleParticipants => battleParticipants;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEncounter(CritterSlot enemyHandler, CritterSlot enemyCritter1, CritterSlot enemyCritter2, CritterSlot enemyCritter3)
    {
        DisableOverworldSettings();

        // Set Battle Participants
        AddBattleParticipant(partyManager.Handler);
        AddBattleParticipant(partyManager.PartyRoster[0]);
        AddBattleParticipant(partyManager.PartyRoster[1]);
        AddBattleParticipant(partyManager.PartyRoster[2]);

        AddBattleParticipant(enemyHandler);
        AddBattleParticipant(enemyCritter1);
        AddBattleParticipant(enemyCritter2);
        AddBattleParticipant(enemyCritter3);

        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);

        mainCamera.SetActive(false); // turn off after new scene is added to avoid disable problem
    }

    private void DisableOverworldSettings()
    {
        playerControllerRef.StopMovement(); // Freeze Player Movement and Interactions
        eventSystem.SetActive(false); // Ensure only one Event Manager is active at a given time.
    }

    private void RenableOverworldSettings()
    {
        playerControllerRef.StartMovement(); // Unfreeze Player Movement and Interactions
        eventSystem.SetActive(true); // Ensure only one Event Manager is active at a given time.
        mainCamera.SetActive(true);
    }

    private void AddBattleParticipant(CritterSlot slot)
    {
        if (slot != null)
        {
            battleParticipants.Add(slot);
        }
        else
        {
            battleParticipants.Add(null);
        }
    }
}
