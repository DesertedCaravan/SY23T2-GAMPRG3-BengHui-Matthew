using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BattleManager : MonoBehaviour
{
    [Header("Overworld Encounter Manager")]
    [SerializeField] private EncounterManager encounterManager;

    [Header("Battle Menu UI Updater")]
    [SerializeField] private BattleMenuUI battleMenuUI;

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
    private MoveData chosenMove;
    private ItemBase chosenItem;

    [Header("Target Selection")]
    [SerializeField] private List<GameObject> allTargets;
    private List<GameObject> availableTargets;
    private List<int> availableSlots;

    private int _selectedTarget;
    private bool _selectAllTargets;

    // Self = allTargets[0]
    // Ally1 = allTargets[1]
    // Ally2 = allTargets[2]
    // Ally3 = allTargets[3]
    // EnemyHandler = allTargets[4]
    // Enemy1 = allTargets[5]
    // Enemy2 = allTargets[6]
    // Enemy3 = allTargets[7]

    private int _turn;
    private int _turnPhase;
    private bool _turnPhaseTransition;

    private int _chosenAlly;
    // 0 = critterList[0]
    // 1 = critterList[1]
    // 2 = critterList[2]
    // 3 = critterList[3]

    [Header("Ally Row")]
    [SerializeField] private List<CritterSlot> allyCritterList; // only full slots

    [Header("Enemy Row")]
    [SerializeField] private List<CritterSlot> enemyCritterList; // always 4 slots

    [Header("Raw Battle Phase Data")]
    [SerializeField] private List<CritterSlot> critterList; // must be empty (8 slots)
    [SerializeField] private List<BattleStats> battleStats; // must be open and filled (8 slots)

    [Header("Refined Battle Phase Data")]
    [SerializeField] private List<ChosenTargets> chosenTargets; // 8 slots
    [SerializeField] private List<BattleStats> turnOrder; // Active critter slots only

    private bool _startBattle; // battle phase begins
    private bool _moveSet; // critter moves are all set
    private bool _moveStart; // active critter can take turn

    private int _currentCritter; // current critter move

    private System.Random _rnd = new System.Random();

    [Header("Dialogue Box Manager Updater")]
    [SerializeField] protected BattleDialogueBoxManager dialogueBoxManager;

    // Start is called before the first frame update
    void Start()
    {
        encounterManager = FindObjectOfType<EncounterManager>(); // looks for the first one

        _startBattle = false;
        _moveSet = false;
        ActiveCritterMoveStart(false);

        _turn = 1;
        battleMenuUI.SetTurnNumber(_turn);

        SetupCombatants();

        InitializeBattle();

        // allowing keyboard interaction must be last
        _turnPhaseTransition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_startBattle == false)
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
                    SetupListItems(); // StartCoroutine(CO_TurnPhaseForwardDelay());
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
        else if (_startBattle == true && _moveSet == true && _moveStart == true)
        {
            // Text display must be automatic
            if (_currentCritter < turnOrder.Count)
            {
                MoveResult();
            }

            _currentCritter++;

            // Delay after pressing quitting out the last text display

            if (_currentCritter > turnOrder.Count)
            {
                _startBattle = false;
                _moveSet = false;
                ActiveCritterMoveStart(false);

                StartCoroutine(CO_NextTurn());

                InitializeBattle();

                _turn++;
                battleMenuUI.SetTurnNumber(_turn);
                Debug.Log("Battle Turn Over!");
            }
        }
    }

    #region PhaseSetup
    private void SetupCombatants()
    {
        #region OldVersion
        /*
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
        */

        #endregion

        allyCritterList = new List<CritterSlot>();

        for (int i = 0; i < 4; i++)
        {
            if (encounterManager.BattleParticipants[i] != null)
            {
                allyCritterList.Add(encounterManager.BattleParticipants[i]);
            }
        }

        enemyCritterList = new List<CritterSlot>();

        for (int i = 4; i < 8; i++)
        {
            if (encounterManager.BattleParticipants[i] != null)
            {
                enemyCritterList.Add(encounterManager.BattleParticipants[i]);
            }
            else
            {
                enemyCritterList.Add(null);
            }
        }

        for (int i = 0; i < encounterManager.BattleParticipants.Count; i++) //  Count should be 8
        {
            if (encounterManager.BattleParticipants[i] != null)
            {
                critterList.Add(encounterManager.BattleParticipants[i]);
            }
            else
            {
                critterList.Add(null);
            }
        }

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
            battleStats[i].SetCritterSlotNo = i;
            battleStats[i].SetCritterSlotData = critterList[i];
            battleStats[i].SetCritterStatusHolder = battleMenuUI.GetStatPanels[i].GetComponent<BattleStatusHolder>();
        }
    }

    private void InitializeBattle() // occurs every new turn
    {
        _chosenAlly = 0;

        battleMenuUI.UnmarkOrderMarkers();
        battleMenuUI.UnmarkOrderNumbers();

        for (int i = 0; i < battleStats.Count; i++)
        {
            battleStats[i].ResetTurn();
        }

        PassAllStatusEffectsTurn();

        SetupAllyCommand(); // occurs every new character move

        // 4 - 7 were originally for SetEnemyMoves()
        for (int i = 0; i < 8; i++)
        {
            ClearChosenTargetsList(i);
        }
    }

    private void PassAllStatusEffectsTurn()
    {
        for (int i = 0; i < critterList.Count; i++)
        {
            if (battleStats[i].GetCritterSlotData != null)
            {
                Debug.Log(i + " status effect elapsed!");
                battleStats[i].GetCritterStatusHolder.ReduceAllEffectDuration();
            }
        }
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

        Debug.Log("Selected Command: " + _selectedCommand);

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

            UpdateListSelection();

            SetSelectedData();

            StartCoroutine(CO_TurnPhaseForwardDelay());
        }
        else if (_selectedCommand == 1) // Skip Turn
        {
            battleStats[_chosenAlly].SkipTurn();

            NextCritter();
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

            UpdateListSelection();

            SetSelectedData();

            StartCoroutine(CO_TurnPhaseForwardDelay());
        }
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
                    if (i != _chosenAlly && i != 0 && i != 4 && critterList[i] != null) // not the user or either handler
                    {
                        availableTargets.Add(allTargets[i]);
                        availableSlots.Add(i);
                    }
                }

                _selectAllTargets = true;
            }
            else if (chosenMove.Target == 8) // All
            {
                for (int i = 0; i < allTargets.Count; i++)
                {
                    if (i != 0 && i != 4 && critterList[i] != null) // not either handler
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
        SetupChosenAction();
    }

    #endregion

    #region CritterChange

    private void SetupChosenAction()
    {
        // Set chosenTargets
        chosenTargets[_chosenAlly].SetUserSlotData = allyCritterList[_chosenAlly];
        chosenTargets[_chosenAlly].SetUserStatusHolder = battleMenuUI.GetStatPanels[_chosenAlly].GetComponent<BattleStatusHolder>();

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

        NextCritter();
    }

    private void NextCritter()
    {
        // Check if there are more allies to command
        _chosenAlly++; // Range: 0 - 3

        if (_chosenAlly < allyCritterList.Count) // Range: 1 - 4
        {
            Debug.Log(_chosenAlly + "|" + allyCritterList.Count);
            battleMenuUI.DisplayOrderMarker(_chosenAlly); // Set Active next marker if an ally is present
        }

        if (_chosenAlly == allyCritterList.Count) // allyCritterList is filled up at the start of battle, so no null indexes
        {
            Debug.Log("BattleStart");
            _startBattle = true;
            StartBattlePhase();
        }
        else
        {
            StartCoroutine(CO_ChangeAlly());
        }
    }

    private void PreviousCritter()
    {
        ClearChosenTargetsList(_chosenAlly);

        if (_chosenAlly != 0)
        {
            battleMenuUI.HideOrderMarker(_chosenAlly);
        }

        _chosenAlly--;

        if (_chosenAlly == 0)
        {
            ClearChosenTargetsList(_chosenAlly);
            battleStats[_chosenAlly].UnskipTurn();
        }

        StartCoroutine(CO_ChangeAlly());
    }

    private void ClearChosenTargetsList(int index)
    {
        chosenTargets[index].SetUserSlotData = null;
        chosenTargets[index].SetUserStatusHolder = null;
        chosenTargets[index].SetChosenMove = null;
        chosenTargets[index].SetChosenItem = null;
        chosenTargets[index].chosenTargets.Clear();
    }

    #endregion

    #region BattleCalculations
    // Important Variable Lists

    // List<CritterSlot> critterList;               Permanent Critter Slot Data (can have empty slots)
    // private List<BattleStats> battleStats;       Temporary Critter Slot Data

    // private List<ChosenTargets> chosenTargets;   Targets for the Current Turn (can have empty slots)
    // private List<BattleStats> turnOrder;         Attacking Critters (only has full slots)

    private void StartBattlePhase()
    {
        SetEnemyMoves();

        ClearTurnOrder();
        CalculateTurnOrder();
        ArrangeTurnOrder();
        DisplayTurnOrder();

        _currentCritter = 0;
        _moveSet = true;

        ActiveCritterMoveStart(true);
    }

    private void SetEnemyMoves()
    {
        // 4: EHandler
        // 5: Enemy1
        // 6: Enemy2
        // 7: Enemy3

        for (int i = 4; i <= 7; i++)
        {
            if (critterList[i] != null)
            {
                SetEnemyTargets(i);
            }
        }
    }

    private void SetEnemyTargets(int enemy)
    {
        // Set Enemy ChosenTargets Critter Data -----------------------------

        chosenTargets[enemy].SetUserSlotData = critterList[enemy];
        chosenTargets[enemy].SetUserStatusHolder = battleMenuUI.GetStatPanels[enemy].GetComponent<BattleStatusHolder>();

        // Create Temporary Move List
        List<MoveData> moveHolder = new List<MoveData>();

        foreach (MoveData move in critterList[enemy].SetMoves)
        {
            moveHolder.Add(move);
        }

        foreach (MoveData move in critterList[enemy].LearnedMoves)
        {
            moveHolder.Add(move);
        }

        int randomMove = _rnd.Next(0, moveHolder.Count);

        // Set Enemy ChosenTargets Move Data -----------------------------

        chosenTargets[enemy].SetChosenMove = moveHolder[randomMove];

        // Set Enemy ChosenTargets Target(s) Data -----------------------------

        if (moveHolder[randomMove].Target == 0) // Self
        {
            chosenTargets[enemy].chosenTargets.Add(enemy);
        }
        else if (moveHolder[randomMove].Target == 1) // OneAlly
        {
            // Set Enemy Targets
            List<int> randomTargets = new List<int>();

            for (int j = 5; j <= 7; j++)
            {
                if (critterList[j] != null)
                {
                    randomTargets.Add(j);
                }
            }

            int randomAlly = _rnd.Next(0, randomTargets.Count);

            chosenTargets[enemy].chosenTargets.Add(randomTargets[randomAlly]);
        }
        else if (moveHolder[randomMove].Target == 2) // AllyRow
        {
            for (int j = 5; j <= 7; j++)
            {
                if (critterList[j] != null)
                {
                    chosenTargets[enemy].chosenTargets.Add(j);
                }
            }
        }
        else if (moveHolder[randomMove].Target == 3) // PHandler
        {
            if (critterList[4] != null)
            {
                chosenTargets[enemy].chosenTargets.Add(4);
            }
        }
        else if (moveHolder[randomMove].Target == 4) // OneEnemy
        {
            // Set Enemy Targets
            List<int> randomTargets = new List<int>();

            for (int j = 1; j <= 3; j++)
            {
                if (critterList[j] != null)
                {
                    randomTargets.Add(j);
                }
            }

            int randomAlly = _rnd.Next(0, randomTargets.Count);

            chosenTargets[enemy].chosenTargets.Add(randomTargets[randomAlly]);
        }
        else if (moveHolder[randomMove].Target == 5) // EnemyRow
        {
            for (int j = 1; j <= 3; j++)
            {
                if (critterList[j] != null)
                {
                    chosenTargets[enemy].chosenTargets.Add(j);
                }
            }
        }
        else if (moveHolder[randomMove].Target == 6) // EHandler
        {
            if (critterList[0] != null)
            {
                chosenTargets[enemy].chosenTargets.Add(0);
            }
        }
        else if (moveHolder[randomMove].Target == 7) // AllOthers
        {
            for (int j = 0; j < allTargets.Count; j++)
            {
                if (j != enemy && j != 0 && j != 4 && critterList[j] != null) // not the user or either handler
                {
                    chosenTargets[enemy].chosenTargets.Add(j);
                }
            }
        }
        else if (moveHolder[randomMove].Target == 8) // All
        {
            for (int j = 0; j < allTargets.Count; j++)
            {
                if (j != 0 && j != 4 && critterList[j] != null) // not either handler
                {
                    chosenTargets[enemy].chosenTargets.Add(j);
                }
            }
        }
    }

    private void ClearTurnOrder()
    {
        for (int i = 0; i < critterList.Count; i++)
        {
            battleStats[i].SetTurnOrder = 0;
        }
    }

    private void CalculateTurnOrder()
    {
        for (int i = 0; i < critterList.Count; i++)
        {
            if (critterList[i] != null)
            {
                // maxValue of Random is exclusive
                // minValue of Random is inclusive
                // Turn Order: Agi * Agi. Mod * (Random.Range(0, Luck + 26) + 75) / 100
                battleStats[i].SetTurnOrder = critterList[i].Agility * battleStats[i].GetCritterStatusHolder.CheckStatMod(3) * (_rnd.Next(0, critterList[i].Luck + 26) + 75) / 100; // 3 = Agility
            }
        }
    }

    private void ArrangeTurnOrder()
    {
        // Check how many active Critters there are
        int active = 0;

        foreach (CritterSlot activeSlots in critterList)
        {
            if (activeSlots != null)
            {
                active++;
            }
        }

        // Organize turnOrder List
        turnOrder = new List<BattleStats>();

        for (int i = 0; i < active; i++)
        {
            float turnStatHolder = 0;
            int critterSlotHolder = 0;

            int previousIndex = -1;

            for (int j = 0; j < battleStats.Count; j++)
            {
                // Note: Turn order is correct, but 0 remains unchecked

                if (battleStats[j].GetTurnOrder > turnStatHolder && battleStats[j].CheckTurn() == false)
                {
                    turnStatHolder = battleStats[j].GetTurnOrder;
                    critterSlotHolder = battleStats[j].GetCritterSlotNo;

                    battleStats[j].TakeTurn();

                    if (previousIndex != -1)
                    {
                        battleStats[previousIndex].ResetTurn();
                    }

                    previousIndex = j;
                }
            }

            if (battleStats[critterSlotHolder].CheckSkipTurn() == false) // only add if the chosen slot is not skipped
            {
                turnOrder.Add(battleStats[critterSlotHolder]);
            }
            else
            {
                battleStats[critterSlotHolder].UnskipTurn();
            }
        }
    }

    private void DisplayTurnOrder()
    {
        battleMenuUI.UnmarkOrderNumbers();

        for (int i = 0; i < turnOrder.Count; i++)
        {
            battleMenuUI.MarkOrderNumber(turnOrder[i].GetCritterSlotNo, i + 1);
        }
    }

    #endregion

    #region BattleDialogueBox
    // Important Variable Lists

    // List<CritterSlot> critterList;               Permanent Critter Slot Data (can have empty slots)
    // private List<BattleStats> battleStats;       Temporary Critter Slot Data

    // private List<ChosenTargets> chosenTargets;   Targets for the Current Turn (can have empty slots)
    // private List<BattleStats> turnOrder;         Attacking Critters (only has full slots)

    private void MoveResult()
    {
        ActiveCritterMoveStart(false);

        // turnOrder has a variable order
        // chosenTargets and battleStats is always a set order (1 - 8, with null)

        // Active Critter (requires turnOrder)
        int actingCritter = turnOrder[_currentCritter].GetCritterSlotNo;

        // Action Taken (requires chosenTargets)
        CritterSlot actingCritterData = chosenTargets[actingCritter].GetUserSlotData;
        BattleStatusHolder actingCritterStatus = chosenTargets[actingCritter].GetUserStatusHolder;
        MoveData moveUsed = chosenTargets[actingCritter].GetChosenMove;
        ItemBase itemUsed = chosenTargets[actingCritter].GetChosenItem;

        // Affected Targets (requires chosenTargets and battleStats)
        List<int> targets = chosenTargets[actingCritter].chosenTargets;
        List<CritterSlot> targetCritterData = new List<CritterSlot>();
        List<BattleStatusHolder> targetCritterStatus = new List<BattleStatusHolder>();
        
        foreach (int t in targets)
        {
            targetCritterData.Add(battleStats[t].GetCritterSlotData);
            Debug.Log("Add: " + t);
        }

        foreach (int t in targets)
        {
            targetCritterStatus.Add(battleStats[t].GetCritterStatusHolder);
            Debug.Log("Add: " + t);
        }

        // Check if Move or Item 
        if (moveUsed != null)
        {
            MoveCalculation(actingCritter, actingCritterData, actingCritterStatus, moveUsed, targets, targetCritterData, targetCritterStatus);
        }
        else
        {
            ItemCalculation(actingCritter, actingCritterData, itemUsed, targets, targetCritterData, targetCritterStatus);
        }
    }

    private void MoveCalculation(int userInt, CritterSlot userData, BattleStatusHolder userStatus, MoveData move, List<int> targetInts, List<CritterSlot> targetData, List<BattleStatusHolder> targetStatus)
    {
        List<bool> hitOutcome = new List<bool>();

        // Cost Reduction
        userStatus.SetStaminaStatus(-move.StaminaCost); // negative to indicate lost stamina
        userStatus.SetReasonStatus(-move.ReasonCost); // negative to indicate lost reason

        // Count Targets
        for (int i = 0; i < targetData.Count; i++)
        {
            float userLuck = (float)(UnityEngine.Random.Range(0, userData.Luck + 26f) + 75f) / 100f; // use (float) or it will only 1 or 0
            float targetLuck = (float)(UnityEngine.Random.Range(0, targetData[i].Luck + 26f) + 75f) / 100f; // use (float) or it will only 1 or 0

            float posHit = ((float)userData.Level + (float)move.Accuracy) * (float)userStatus.CheckStatMod(7) * // 7 = Accuracy
                (float)userLuck * (float)userStatus.CheckStatMod(5); // 5 = Luck

            float negHit = ((float)targetData[i].GetStat(move.Save) * (float)targetStatus[i].CheckStatMod(move.Save)) +
                ((float)targetData[i].Evasion * (float)targetStatus[i].CheckStatMod(6) * // 6 = Evasion
                (float)targetLuck * (float)targetStatus[i].CheckStatMod(5)); // 5 = Luck

            // Hit Calculation (int)
            Debug.Log("Turn: " + _turn + " | " + userData.Name + " | Added Hit Rate: " + userData.Level + "|" + move.Accuracy + "|" + userStatus.CheckStatMod(7) + "|" + userLuck + "|" + userStatus.CheckStatMod(5));
            Debug.Log("Turn: " + _turn + " | " + userData.Name + " | Raw Pos Hit: " + posHit);
            Debug.Log("Turn: " + _turn + " | " + userData.Name + " | Subtracted Hit Rate: " + move.Save + "|" + userStatus.CheckStatMod(move.Save) + "|" + targetData[i].Evasion + "|" + targetStatus[i].CheckStatMod(6) + "|" + targetLuck + "|" + userStatus.CheckStatMod(5));
            Debug.Log("Turn: " + _turn + " | " + userData.Name + " | Raw Neg Hit: " + negHit);

            float hitPercent = posHit - negHit;
            int chosenPercent = UnityEngine.Random.Range(0, 101); // 1% - 100%

            Debug.Log(userData.Name + ": " + chosenPercent + "<=" + hitPercent);

            // Damage Calculation (WIP)
            if (chosenPercent <= hitPercent) // percentageCounter must be equal or less than the roll
            {
                Debug.Log("Turn: " + _turn + " | " + userData.Name + " hit!");

                hitOutcome.Add(true);

                int damageCategory;
                float damageModifier;

                if (move.Damage > 0)
                {
                    if (move.DamageCategory == 0)
                    {
                        damageCategory = userData.Strength;
                        damageModifier = userStatus.CheckStatMod(1); // 1 = Strength
                    }
                    else
                    {
                        damageCategory = userData.Power;
                        damageModifier = userStatus.CheckStatMod(4); // 4 = Power
                    }

                    float userDamageLuck = (float)(UnityEngine.Random.Range(0, userData.Luck + 26) + 75) / 100f;

                    float rawDamage =
                        ( (((float)userData.Level * 10f) + (float)move.Damage) / (float)50 *
                        ((float)damageCategory * (float)damageModifier) / ((float)targetData[i].Toughness * (float)targetStatus[i].CheckStatMod(2)) * // 2 = Toughness
                        (float)userDamageLuck ) + 1f;

                    int damage = (int)Mathf.Ceil(rawDamage);

                    Debug.Log("Turn: " + _turn + " | " + userData.Level + " | Damage Stats: " + move.Damage + "|" + damageCategory + "|" + damageModifier + "|" + targetData[i].Toughness + "|" + targetStatus[i].CheckStatMod(2) + "|" + userDamageLuck);
                    Debug.Log("Turn: " + _turn + " | " + userData.Name + " | Damage Dealt: " + rawDamage + "=>" + (int)Mathf.Ceil(rawDamage));

                    if (damage > 0)
                    {
                        targetStatus[i].SetVitalityStatus(-damage); // negative to indicate lost health
                    }
                }

                // Effect Chance (based on damage hit rate)
                foreach (EffectData effect in move.Effects)
                {
                    List<BattleStatusHolder> targets = new List<BattleStatusHolder>();

                    if (effect.Target == 0) // Self
                    {
                        targets.Add(battleStats[userInt].GetCritterStatusHolder);
                    }
                    else if (effect.Target == 1) // Ally Row
                    {
                        for (int j = 1; j < 3; j++)
                        {
                            if (battleStats[j].GetCritterSlotData != null)
                            {
                                targets.Add(battleStats[j].GetCritterStatusHolder);
                            }
                        }
                    }
                    else if (effect.Target == 2) // P Handler
                    {
                        if (battleStats[0].GetCritterSlotData != null)
                        {
                            targets.Add(battleStats[0].GetCritterStatusHolder);
                        }
                    }
                    else if (effect.Target == 3) // Target
                    {
                        targets.Add(battleStats[targetInts[i]].GetCritterStatusHolder);
                    }
                    else if (effect.Target == 4) // Enemy Row
                    {
                        for (int j = 5; j < 7; j++)
                        {
                            if (battleStats[j].GetCritterSlotData != null)
                            {
                                targets.Add(battleStats[j].GetCritterStatusHolder);
                            }
                        }
                    }
                    else if (effect.Target == 5) // E Handler
                    {
                        if (battleStats[4].GetCritterSlotData != null)
                        {
                            targets.Add(battleStats[4].GetCritterStatusHolder);
                        }
                    }
                    else if (effect.Target == 6) // All Others
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if (j != targetInts[i] && j != 0 && j != 4 && battleStats[j].GetCritterSlotData != null)
                            {
                                targets.Add(battleStats[j].GetCritterStatusHolder);
                            }
                        }
                    }
                    else if (effect.Target == 7) // All
                    {
                        for (int j = 0; j < 7; j++)
                        {
                            if (j != 0 && j != 4 && battleStats[j].GetCritterSlotData != null)
                            {
                                targets.Add(battleStats[j].GetCritterStatusHolder);
                            }
                        }
                    }

                    foreach (BattleStatusHolder status in targets)
                    {
                        int effectPercent = effect.Chance;
                        chosenPercent = UnityEngine.Random.Range(0, 101);

                        Debug.Log("Effect Rate: " + userData.Name + ": " + chosenPercent + "<=" + hitPercent);

                        if (effectPercent >= 100 || chosenPercent <= effectPercent) // if effect.Chance is already 100 or greater, or the chosenPercent beats the threshold
                        {
                            if (effect.Type == 2) // 2 = Regain Health
                            {
                                if (effect.Health == 1) // 1 = Vitality
                                {
                                    float temp = status.GetMaxVitality * ((float)effect.HealthChange / 100f);

                                    Debug.Log(status.GetMaxVitality + "|" + effect.HealthChange + " Vitality Gain: " + (int)Mathf.Ceil(temp));
                                    status.SetVitalityStatus((int)Mathf.Ceil(temp));
                                }
                                else if (effect.Health == 2) // 2 = Stamina
                                {
                                    float temp = status.GetMaxStamina * ((float)effect.HealthChange / 100f);

                                    Debug.Log(status.GetMaxStamina + "|" + effect.HealthChange + " Stamina Gain: " + (int)Mathf.Ceil(temp));
                                    status.SetStaminaStatus((int)Mathf.Ceil(temp));
                                }
                                else if (effect.Health == 3) // 3 = Reason
                                {
                                    float temp = status.GetMaxReason * ((float)effect.HealthChange / 100f);

                                    Debug.Log(status.GetMaxReason + "|" + effect.HealthChange + " Reason Gain: " + (int)Mathf.Ceil(temp));
                                    status.SetReasonStatus((int)Mathf.Ceil(temp));
                                }
                            }
                            else
                            {
                                status.AddEffect(effect);
                            }
                        }
                    }
                }
            }
            else
            {
                hitOutcome.Add(false);
            }
        }

        // Required Data
        // - Slot Side (int)
        // - Active Critter (CritterSlot)
        // - Move Taken (MoveData)
        // - Move Result (MoveCalculation bool)

        dialogueBoxManager.StartBattlePhase(userInt, userData.Name, move.Name, hitOutcome, MoveCheck());
    }

    private void ItemCalculation(int userInt, CritterSlot userData, ItemBase item, List<int> targetInts, List<CritterSlot> targetData, List<BattleStatusHolder> targetStatus)
    {

    }

    private bool MoveCheck()
    {
        return false;
    }

    #endregion

    #region BattleEndTurn

    public void ActiveCritterMoveStart(bool state)
    {
        _moveStart = state;
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

    IEnumerator CO_NextTurn()
    {
        _turnPhaseTransition = true;

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Next Turn");

        // allowing keyboard interaction must be last
        _turnPhaseTransition = false;
    }

    #endregion

    [Serializable]
    public class BattleStats
    {
        [Header("Main Info.")]
        [SerializeField] private int _slotNo;
        [SerializeField] private CritterSlot _slotData;
        [SerializeField] private BattleStatusHolder _statHolder;

        [Header("Turn Order")]
        [SerializeField] private float _turnOrder;
        [SerializeField] private bool _turnTaken;
        [SerializeField] private bool _turnSkipped;

        public int GetCritterSlotNo { get { return _slotNo; } }
        public int SetCritterSlotNo { set { _slotNo = value; } }
        public CritterSlot GetCritterSlotData { get { return _slotData; } }
        public CritterSlot SetCritterSlotData { set { _slotData = value; } }

        public BattleStatusHolder GetCritterStatusHolder { get { return _statHolder; } }
        public BattleStatusHolder SetCritterStatusHolder { set { _statHolder = value; } }

        public float GetTurnOrder { get { return _turnOrder; } }
        public float SetTurnOrder { set { _turnOrder = value; } }

        public bool CheckTurn()
        {
            return _turnTaken;
        }

        public void TakeTurn()
        {
            _turnTaken = true;
        }

        public void ResetTurn()
        {
            _turnTaken = false;
        }

        public bool CheckSkipTurn()
        {
            return _turnSkipped;
        }

        public void SkipTurn()
        {
            _turnSkipped = true;
        }

        public void UnskipTurn()
        {
            _turnSkipped = false;
        }
    }

    [Serializable] // Allows this to be displayed in the inspector (requires using System)
    public class ChosenTargets
    {
        [Header("Main Info.")]
        [SerializeField] private CritterSlot _userSlotData;
        [SerializeField] private BattleStatusHolder _userStatusHolder;
        [SerializeField] private MoveData chosenMove;
        [SerializeField] private ItemBase chosenItem;
        [SerializeField] public List<int> chosenTargets;

        public CritterSlot GetUserSlotData { get { return _userSlotData; } }
        public CritterSlot SetUserSlotData { set { _userSlotData = value; } }

        public BattleStatusHolder GetUserStatusHolder { get { return _userStatusHolder; } }
        public BattleStatusHolder SetUserStatusHolder { set { _userStatusHolder = value; } }

        public MoveData GetChosenMove { get { return chosenMove; } }
        public MoveData SetChosenMove { set { chosenMove = value; } }

        public ItemBase GetChosenItem { get { return chosenItem; } }
        public ItemBase SetChosenItem { set { chosenItem = value; } }
    }
}