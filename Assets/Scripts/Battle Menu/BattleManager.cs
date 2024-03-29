using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BattleManager : MonoBehaviour
{
    [Header("Battle Menu UI Updater")]
    [SerializeField] private BattleMenuUI battleMenuUI;

    [Header("Ally Row")]
    [SerializeField] private List<CritterSlot> allyCritterList; // 4 slots

    [Header("Enemy Row")]
    [SerializeField] private List<CritterSlot> enemyCritterList; // 4 slots

    [SerializeField] private List<CritterSlot> critterList; // 8 slots

    [Header("Turn Order")]
    [SerializeField] private GameObject[] turnMarkers;
    private int _turn;
    private int _turnPhase;
    private bool _turnPhaseTransition;

    private int _chosenAlly;
    // 0 = critterList[0]
    // 1 = critterList[1]
    // 2 = critterList[2]
    // 3 = critterList[3]

    [Header("Battle Commands")]
    [SerializeField] private GameObject[] menuOptions;
    [SerializeField] private GameObject secondPhaseList;
    [SerializeField] private GameObject secondPhaseContent;
    private int _selectedCommand;

    [Header("Attack Command")]
    [SerializeField] private GameObject moveRow;
    [SerializeField] private GameObject movePrefab;

    [Header("Use Pouch Command")]
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private GameObject stockRow;
    [SerializeField] private GameObject stockPrefab;

    [Header("List Item Selection")]
    [SerializeField] private BattleSelectedUI selectedUI;

    private List<GameObject> selectionList;
    private List<MoveData> moveDataList;
    private List<ItemBase> stockDataList;

    private int _selectedListItem;

    [Header("Target Selection")]
    [SerializeField] private List<GameObject> allTargets;
    private List<GameObject> availableTargets;
    private List<int> availableSlots;

    private bool _selectAllTargets;

    // Self = allTargets[0]
    // Ally1 = allTargets[1]
    // Ally2 = allTargets[2]
    // Ally3 = allTargets[3]
    // EnemyHandler = allTargets[4]
    // Enemy1 = allTargets[1]
    // Enemy2 = allTargets[6]
    // Enemy3 = allTargets[7]

    private MoveData chosenMove;
    private ItemBase chosenItem;
    private int _selectedTarget;

    [Header("Battle Phase")]
    [SerializeField] private List<BattleStats> battleStats; // 8 slots
    [SerializeField] private List<ChosenTargets> chosenTargets; // depends on number of active allies

    // Start is called before the first frame update
    void Start()
    {
        _turnPhaseTransition = false;

        InitializeBattle();
    }

    // Update is called once per frame
    void Update()
    {
        if (_turnPhase == 1 && _turnPhaseTransition == false) // Player selects battle commands
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                HandleCommandUpdate(true);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                HandleCommandUpdate(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SetupListItems();
                StartCoroutine(CO_TurnPhaseForwardDelay());
            }
            else if (Input.GetKeyDown(KeyCode.Q) && _chosenAlly > 0)
            {
                PreviousCritter();
            }
        }

        if (_turnPhase == 2 && _turnPhaseTransition == false)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                HandleListUpdate(true);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                HandleListUpdate(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                SelectTarget();
                StartCoroutine(CO_TurnPhaseForwardDelay());
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ClearListItems();
                selectedUI.ClearData();

                _selectedListItem = 0;

                StartCoroutine(CO_TurnPhaseBackwardDelay());
            }
        }

        if (_turnPhase == 3 && _turnPhaseTransition == false)
        {
            if (_selectAllTargets == false)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    HandleTargetUpdate(true);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    HandleTargetUpdate(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ConfirmTarget();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ClearSelect();
                ClearTargets();

                _selectedTarget = 0;

                StartCoroutine(CO_TurnPhaseBackwardDelay());
            }
        }
    }

    #region PhaseSetup

    private void InitializeBattle()
    {
        _turn = 1;

        _chosenAlly = 0;

        SetupCombatants();

        UnmarkTurnMarkers();

        SetupAllyCommand();
    }

    private void SetupCombatants()
    {
        critterList = new List<CritterSlot>();

        int allyCounter = 0;

        foreach (CritterSlot allySlot in allyCritterList)
        {
            critterList.Add(allySlot);
            allyCounter++;
        }

        if (allyCounter < 4)
        {
            for (int i = 0; i < 4 - allyCounter; i++)
            {
                critterList.Add(null);
            }
        }

        critterList.Add(enemyCritterList[0]);
        critterList.Add(enemyCritterList[1]);
        critterList.Add(enemyCritterList[2]);
        critterList.Add(enemyCritterList[3]);

        // Go to BattleMenuUI and set UI and BattleStatHolders
        
        for (int i = 0; i < critterList.Count; i++)
        {
            if (critterList[i] != null)
            {
                battleMenuUI.ActivateSprite(critterList[i], i);
            }
            else
            {
                battleMenuUI.CloseSprite(i);
            }
        }

        // Set battleStats for Battle Phase

        for (int i = 0; i < critterList.Count; i++)
        {
            battleStats[i].SetCritterSlot = i;
            battleStats[i].SetStatHolder = battleMenuUI.GetStatPanels[i].GetComponent<BattleStatHolder>();
        }
    }

    private void UnmarkTurnMarkers()
    {
        foreach (GameObject marker in turnMarkers)
        {
            marker.SetActive(false);
        }

        turnMarkers[0].SetActive(true);
    }

    private void SetupAllyCommand()
    {
        _selectedCommand = 0;
        UpdateCommandSelection();

        _selectedListItem = 0;
        ClearListItems();
        selectedUI.ClearData();

        _selectedTarget = 0;
        ClearSelect();
        ClearTargets();

        _turnPhase = 1;
    }

    #endregion

    #region Phase1CommandSelection
    private void HandleCommandUpdate(bool direction)
    {
        if (direction == false)
        {
            ++_selectedCommand;
        }
        else if (direction == true)
        {
            --_selectedCommand;
        }

        _selectedCommand = Mathf.Clamp(_selectedCommand, 0, menuOptions.Length - 1);

        UpdateCommandSelection();
    }

    private void UpdateCommandSelection()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (i == _selectedCommand)
            {
                menuOptions[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
            }
            else
            {
                menuOptions[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    #endregion

    #region Phase2ListSelection
    private void SetupListItems()
    {
        if (_selectedCommand == 0)
        {
            secondPhaseList.SetActive(true);
            moveRow.SetActive(true);
            stockRow.SetActive(false);

            // Clear all exisiting child Game Objects in the Pouch List Scrollview Content
            foreach (Transform child in secondPhaseContent.transform)
            {
                Destroy(child.gameObject);
            }

            selectionList = new List<GameObject>(); // Make new temporary list
            moveDataList = new List<MoveData>();

            foreach (var slot in critterList[_chosenAlly].SetMoves)
            {
                var itemDisplay = Instantiate(movePrefab, secondPhaseContent.transform); // Instantiate Item Prefab
                itemDisplay.GetComponent<MoveUI>().SetData(slot);

                selectionList.Add(itemDisplay);
                moveDataList.Add(slot);
            }

            foreach (var slot in critterList[_chosenAlly].LearnedMoves)
            {
                var itemDisplay = Instantiate(movePrefab, secondPhaseContent.transform); // Instantiate Item Prefab
                itemDisplay.GetComponent<MoveUI>().SetData(slot);

                selectionList.Add(itemDisplay);
                moveDataList.Add(slot);
            }
        }
        else if (_selectedCommand == 2)
        {
            secondPhaseList.SetActive(true);

            moveRow.SetActive(false);
            stockRow.SetActive(true);

            // Clear all exisiting child Game Objects in the Pouch List Scrollview Content
            foreach (Transform child in secondPhaseContent.transform)
            {
                Destroy(child.gameObject);
            }

            selectionList = new List<GameObject>(); // Make new temporary list
            stockDataList = new List<ItemBase>();

            foreach (var slot in inventory.HealingSlots)
            {
                var itemDisplay = Instantiate(stockPrefab, secondPhaseContent.transform); // Instantiate Item Prefab
                itemDisplay.GetComponent<ItemUI>().SetData(slot);

                selectionList.Add(itemDisplay);
                stockDataList.Add(slot.GetItem);
            }

            foreach (var slot in inventory.BuffingSlots)
            {
                var itemDisplay = Instantiate(stockPrefab, secondPhaseContent.transform); // Instantiate Item Prefab
                itemDisplay.GetComponent<ItemUI>().SetData(slot);

                selectionList.Add(itemDisplay);
                stockDataList.Add(slot.GetItem);
            }

            foreach (var slot in inventory.ToolSlots)
            {
                var itemDisplay = Instantiate(stockPrefab, secondPhaseContent.transform); // Instantiate Item Prefab
                itemDisplay.GetComponent<ItemUI>().SetData(slot);

                selectionList.Add(itemDisplay);
                stockDataList.Add(slot.GetItem);
            }
        }

        UpdateListSelection();

        SetSelectedData();
    }

    private void ClearListItems()
    {
        secondPhaseList.SetActive(false);

        moveRow.SetActive(false);
        stockRow.SetActive(false);

        // Clear all exisiting child Game Objects in the Pouch List Scrollview Content
        foreach (Transform child in secondPhaseContent.transform)
        {
            Destroy(child.gameObject);
        }

        selectionList = new List<GameObject>();
        moveDataList = new List<MoveData>();
        stockDataList = new List<ItemBase>();
    }

    private void HandleListUpdate(bool direction)
    {
        if (direction == false)
        {
            ++_selectedListItem;
        }
        else if (direction == true)
        {
            --_selectedListItem;
        }

        _selectedListItem = Mathf.Clamp(_selectedListItem, 0, selectionList.Count - 1);

        UpdateListSelection();
    }

    private void UpdateListSelection()
    {
        for (int i = 0; i < selectionList.Count; i++)
        {
            if (i == _selectedListItem)
            {
                selectionList[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
            }
            else
            {
                selectionList[i].GetComponent<Image>().color = Color.white;
            }
        }

        SetSelectedData();
    }

    private void SetSelectedData()
    {
        if (_selectedCommand == 0)
        {
            selectedUI.SetMoveData(moveDataList[_selectedListItem]);
        }
        else if (_selectedCommand == 2)
        {
            selectedUI.SetStockData(stockDataList[_selectedListItem]);
        }
    }

    #endregion

    #region Phase3TargetSelection
    private void SelectTarget()
    {
        ClearSelect();

        if (_selectedCommand == 0)
        {
            chosenMove = moveDataList[_selectedListItem];
        }
        else if (_selectedCommand == 2)
        {
            chosenItem = stockDataList[_selectedListItem];
        }

        // Move Targets
        /*
            Self,
            OneAlly,
            AllyRow,
            PHandler,
            OneEnemy,
            EnemyRow,
            EHandler,
            AllOthers,
            All
        */

        availableTargets = new List<GameObject>();
        availableSlots = new List<int>();
        _selectAllTargets = false;

        if (chosenMove != null)
        {
            if (chosenMove.Target == 0) // Self (Always Possible)
            {
                availableTargets.Add(allTargets[_chosenAlly]);
                availableSlots.Add(_chosenAlly);
            }
            else if (chosenMove.Target == 1) // OneAlly
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (critterList[i] != null)
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }
            }
            else if (chosenMove.Target == 2) // AllyRow
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (critterList[i] != null)
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }

                _selectAllTargets = true;
            }
            else if (chosenMove.Target == 3) // PHandler (Always Possible)
            {
                availableTargets.Add(allTargets[0]);
                availableSlots.Add(0);
            }
            else if (chosenMove.Target == 4) // OneEnemy
            {
                for (int i = 5; i <= 7; i++)
                {
                    if (critterList[i] != null)
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }
            }
            else if (chosenMove.Target == 5) // EnemyRow
            {
                for (int i = 5; i <= 7; i++)
                {
                    if (critterList[i] != null)
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }

                _selectAllTargets = true;
            }
            else if (chosenMove.Target == 6) // EHandler
            {
                if (critterList[4] != null)
                {
                    availableTargets.Add(allTargets[4]);
                    availableSlots.Add(4);
                }
            }
            else if (chosenMove.Target == 7) // AllOthers
            {
                for (int i = 0; i < allTargets.Count; i++)
                {
                    if (i != _chosenAlly && i != 0 && 1 != 4 && critterList[i] != null) // not the user or either handler
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }

                _selectAllTargets = true;
            }
            else if (chosenMove.Target == 7) // All
            {
                for (int i = 0; i < allTargets.Count; i++)
                {
                    if (i != 0 && 1 != 4 && critterList[i] != null) // not either handler
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }

                _selectAllTargets = true;
            }
        }

        _selectedTarget = 0;

        UpdateTargetSelection();
    }

    private void ClearSelect()
    {
        chosenMove = null;
        chosenItem = null;
    }

    private void HandleTargetUpdate(bool direction)
    {
        if (direction == false)
        {
            ++_selectedTarget;
        }
        else if (direction == true)
        {
            --_selectedTarget;
        }

        _selectedTarget = Mathf.Clamp(_selectedTarget, 0, availableTargets.Count - 1);

        UpdateTargetSelection();
    }

    private void UpdateTargetSelection()
    {
        foreach (GameObject target in allTargets)
        {
            target.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.0f);
        }

        if (_selectAllTargets == true)
        {
            foreach (GameObject target in availableTargets)
            {
                target.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
            }
        }
        else // (_selectAllTargets == false
        {
            for (int i = 0; i < availableTargets.Count; i++)
            {
                if (i == _selectedTarget)
                {
                    availableTargets[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
                }
                else
                {
                    availableTargets[i].GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
                }
            }
        }
    }

    private void ClearTargets()
    {
        foreach (GameObject target in allTargets)
        {
            target.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.0f);
        }

        availableTargets = new List<GameObject>();
        availableSlots = new List<int>();
    }

    private void ConfirmTarget()
    {
        NextCritter();
    }

    #endregion

    #region BattlePhase1Calculations
    #endregion

    #region BattleDialogueBox
    #endregion

    #region BattleEndTurn
    #endregion

    #region CritterChange

    private void NextCritter()
    {
        // Set chosenTargets
        chosenTargets[_chosenAlly].SetCritterSlot = allyCritterList[_chosenAlly];

        if (_selectedCommand == 0)
        {
            chosenTargets[_chosenAlly].SetChosenMove = moveDataList[_selectedListItem];
        }
        else if (_selectedCommand == 2)
        {
            chosenTargets[_chosenAlly].SetChosenItem = stockDataList[_selectedListItem];
        }

        if (_selectAllTargets == true)
        {
            foreach (int slot in availableSlots)
            {
                chosenTargets[_chosenAlly].chosenTargets.Add(slot);
            }
        }
        else
        {
            chosenTargets[_chosenAlly].chosenTargets.Add(availableSlots[_selectedTarget]);
        }

        // Check if there are more allies to command
        _chosenAlly++;

        if (_chosenAlly < allyCritterList.Count)
        {
            turnMarkers[_chosenAlly].SetActive(true); // Set Active next marker if an ally is present
        }

        if (_chosenAlly == allyCritterList.Count) // allyCritterList is filled up at the start of battle, so no null indexes
        {
            Debug.Log("Start Battle");
        }
        else
        {
            StartCoroutine(CO_ChangeAlly());
        }
    }

    private void PreviousCritter()
    {
        if (_chosenAlly != 0)
        {
            turnMarkers[_chosenAlly].SetActive(false);
        }

        _chosenAlly--;

        StartCoroutine(CO_ChangeAlly());
    }

    #endregion

    #region Coroutines

    IEnumerator CO_TurnPhaseForwardDelay()
    {
        _turnPhaseTransition = true;

        yield return new WaitForSeconds(0.5f);

        _turnPhase++;

        _turnPhase = Mathf.Clamp(_turnPhase, 1, 3);

        _turnPhaseTransition = false;

        Debug.Log("Turn Phase: " + _turnPhase);
    }

    IEnumerator CO_TurnPhaseBackwardDelay()
    {
        _turnPhaseTransition = true;
        yield return new WaitForSeconds(0.5f);

        _turnPhase--;

        _turnPhase = Mathf.Clamp(_turnPhase, 1, 3);

        _turnPhaseTransition = false;

        Debug.Log("Turn Phase: " + _turnPhase);
    }

    IEnumerator CO_ChangeAlly()
    {
        _turnPhaseTransition = true;

        availableTargets = new List<GameObject>(); // Bug Fix

        SetupAllyCommand();

        yield return new WaitForSeconds(0.5f);

        _turnPhaseTransition = false;
    }

    #endregion
    
    [Serializable]
    public class BattleStats
    {
        [SerializeField] private int _critterSlot;
        [SerializeField] private BattleStatHolder statHolder;

        public int GetCritterSlot { get { return _critterSlot; } }
        public int SetCritterSlot { set { _critterSlot = value; } }

        public BattleStatHolder GetStatHolder { get { return statHolder; } }
        public BattleStatHolder SetStatHolder { set { statHolder = value; } }
    }

    [Serializable] // Allows this to be displayed in the inspector (requires using System)
    public class ChosenTargets
    {
        [SerializeField] private CritterSlot _ally;
        [SerializeField] private MoveData chosenMove;
        [SerializeField] private ItemBase chosenItem;
        [SerializeField] public List<int> chosenTargets;

        public CritterSlot GetCritterSlot { get { return _ally; } }
        public CritterSlot SetCritterSlot { set { _ally = value; } }

        public MoveData GetChosenMove { get { return chosenMove; } }
        public MoveData SetChosenMove { set { chosenMove = value; } }

        public ItemBase GetChosenItem { get { return chosenItem; } }
        public ItemBase SetChosenItem { set { chosenItem = value; } }
    }
}