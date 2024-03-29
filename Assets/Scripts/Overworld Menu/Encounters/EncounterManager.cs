using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For SceneManager

public class EncounterManager : MonoBehaviour
{
    [SerializeField] PlayerController playerControllerRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartEncounter(CritterSlot critter1, CritterSlot critter2, CritterSlot critter3)
    {
        playerControllerRef.StopMovement(); // Freeze Player Movement and Interactions

        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
    }
}
