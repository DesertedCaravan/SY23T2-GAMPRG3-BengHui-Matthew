using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyUI : OptionsUIBase
{
    [Header("Party Manager Updater")]
    [SerializeField] private PartyManager partyManager;

    [Header("Party UI Game Objects")]
    [SerializeField] private GameObject partyPanel;
    [SerializeField] private List<GameObject> partyUIList;

    [Header("Summary UI")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private SummaryUI summaryUI; // get movesListHolder or movesUIList

    private bool _summaryActive;

    int _selectedCritter = 0;
    int _selectedMove = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        ResetToHandler();

        _summaryActive = false;
        summaryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (partyPanel.activeSelf == true) // Party UI is active
        {
            if (_optionSelected == false && _menuOptionActive == true && _summaryActive == false)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    SwitchChosenCritter(false);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    SwitchChosenCritter(true);
                }
                else if (Input.GetKeyDown(KeyCode.E)) // open summary
                {
                    _optionSelected = true;

                    CheckResponseEvents(optionSelectDialogue);
                    dialogueBoxManager.StartMenuText(this, optionSelectDialogue);
                }
                else if (Input.GetKeyDown(KeyCode.Q)) // quit party but only if summary is not yet active
                {
                    _menuOptionActive = false;
                    CloseMenuOptionPanel();
                }
            }

            if (_summaryActive == true)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    SwitchChosenMove(false);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    SwitchChosenMove(true);
                }
                if (Input.GetKeyDown(KeyCode.Q)) // quit summary
                {
                    CloseCritterSummary();
                }
            }

            if (_menuOptionActiveTrigger == true)
            {
                _menuOptionActiveTrigger = false;
                _menuOptionActive = true;
            }
        }
    }

    public void ResetToHandler()
    {
        _menuOptionActiveTrigger = true;

        _selectedCritter = 0;
        _selectedMove = 0;

        UpdateSelectCritter();
        UpdateHandler();
        UpdateCritterRoster();
    }

    private void SwitchChosenCritter(bool switchDirection)
    {
        if (switchDirection == false)
        {
            --_selectedCritter;
            _selectedCritter = Mathf.Clamp(_selectedCritter, 0, partyUIList.Count - 1); // 0 is the Handler, which should always be present
        }
        else if (switchDirection == true)
        {
            ++_selectedCritter;
            _selectedCritter = Mathf.Clamp(_selectedCritter, 0, partyUIList.Count - 1);

            if (partyManager.PartyRoster[_selectedCritter - 1] == null)
            {
                _selectedCritter--;
            }
        }

        UpdateSelectCritter();
    }

    private void UpdateSelectCritter()
    {
        for (int i = 0; i < partyUIList.Count; i++)
        {
            if (i == _selectedCritter)
            {
                partyUIList[i].GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                partyUIList[i].GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void UpdateHandler()
    {
        partyUIList[0].GetComponent<CritterUI>().SetCritterSlotData(partyManager.Handler);
    }

    public void UpdateCritterRoster()
    {
        // partyUIList[0] = Handler
        // partyUIList[1] = Critter1
        // partyUIList[2] = Critter2
        // partyUIList[3] = Critter3

        for (int i = 0; i < 3; i++) // max of 3 slots (3 Critters)
        {
            if (partyManager.PartyRoster[i] == null)
            {
                if (partyPanel.activeSelf == true)
                {
                    partyUIList[i + 1].GetComponent<CritterUI>().CritterInfo.SetActive(false);
                }
            }
            else
            {
                partyUIList[i + 1].GetComponent<CritterUI>().SetCritterSlotData(partyManager.PartyRoster[i]);

                if (partyPanel.activeSelf == true)
                {
                    partyUIList[i + 1].GetComponent<CritterUI>().CritterInfo.SetActive(true);
                }
            }
        }
    }

    public void OpenSummarySelection()
    {
        if (_selectedCritter == 0)
        {
            summaryUI.SetCritterSlotData(partyManager.Handler);
        }
        else if (_selectedCritter >= 1 || _selectedCritter <= 3)
        {
            summaryUI.SetCritterSlotData(partyManager.PartyRoster[_selectedCritter - 1]);
        }

        UpdateSelectedMove();

        _summaryActive = true;

        summaryPanel.SetActive(true);
    }

    public void SwitchChosenMove(bool switchDirection)
    {
        if (switchDirection == false)
        {
            --_selectedMove;
        }
        else if (switchDirection == true)
        {
            ++_selectedMove;
        }

        _selectedMove = Mathf.Clamp(_selectedMove, 0, summaryUI.MovesUIList.Count - 1);

        UpdateSelectedMove();
    }

    public void UpdateSelectedMove()
    {
        for (int i = 0; i < summaryUI.MovesUIList.Count; i++)
        {
            if (i == _selectedMove)
            {
                summaryUI.MovesUIList[i].NameText.color = Color.red;
            }
            else
            {
                summaryUI.MovesUIList[i].NameText.color = Color.black;
            }
        }

        summaryUI.SetMoveData(_selectedMove);
    }

    public void CloseCritterSummary()
    {
        _optionSelected = false;

        _summaryActive = false;

        summaryPanel.SetActive(false);

        _selectedMove = 0;
    }

    public void OpenItemSelection()
    {
        // TBA
    }

    public void CancelSelection()
    {
        _optionSelected = false;
    }

    protected override void CloseMenuOptionPanel()
    {
        base.CloseMenuOptionPanel();

        partyPanel.SetActive(false);
    }
}
