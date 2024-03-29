using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("Party UI Updater")]
    [SerializeField] private PartyUI partyUI;

    [Header("Party Roster")]
    [SerializeField] private CritterSlot handler;
    [SerializeField] private List<CritterSlot> partyRoster;

    public PartyUI PartyUI => partyUI;
    public CritterSlot Handler => handler;
    public List<CritterSlot> PartyRoster => partyRoster; // 0 - 2

    /*
    // Convert to Singleton
    public static PartyManager instance = null; // public static means that it can be accessed

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }
    */

    public void SetCritter(int rosterSlot, CritterSlot critter) // 0 - 2
    {
        PartyRoster[rosterSlot] = critter;

        PartyUI.UpdateCritterRoster();
    }

    public void RemoveCritter(int rosterSlot) // 0 - 2
    {
        PartyRoster[rosterSlot] = null;

        PartyUI.UpdateCritterRoster();
    }


    public void SwitchRoster(int switch1, int switch2) // 0 - 2
    {
        CritterSlot holder = PartyRoster[switch1];

        PartyRoster[switch1] = PartyRoster[switch2];

        PartyRoster[switch2] = holder;

        PartyUI.UpdateCritterRoster();
    }
}