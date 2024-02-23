using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuUI : MonoBehaviour
{
    public bool _menuState;

    [SerializeField] private GameObject menuPanelToggle; // menu panel toggle
    [SerializeField] private GameObject menuOptionsPanelToggle; // menu option panel toggle

    [SerializeField] private TextMeshProUGUI[] menuOptions; // menu option text
    [SerializeField] private GameObject[] menuPanels; // menu option panels
    // 0 = partyPanel
    // 1 = inventoryPanel
    // 2 = settingsPanel

    private int _selectedOption;
    private bool _optionOpenState;

    public bool OptionOpenState { set { _optionOpenState = value; } }
    public GameObject MenuOptionsPanelToggle => menuOptionsPanelToggle;

    // Convert to Singleton
    public static MenuUI instance = null; // public static means that it can be accessed

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

    // Start is called before the first frame update
    void Start()
    {
        ToggleMenu(false);

        _selectedOption = 0;
        UpdateOptionSelection();
    }

    // Update is called once per frame
    void Update()
    {
        if (_menuState == false)
        {
            if (Input.GetKeyDown(KeyCode.Q) && PlayerController.instance.IsSpeaking == false && PlayerController.instance.PlayerMovementCheck() == false)
            {
                ToggleMenu(true);

                _selectedOption = 0;
                UpdateOptionSelection();

                PlayerController.instance.TogglePlayerMovement(false);
            }
        }
        else if (_menuState == true && _optionOpenState == false)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                HandleOptionUpdate(false);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                HandleOptionUpdate(true);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                OpenMenuPanel(_selectedOption);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleMenu(false);

                PlayerController.instance.TogglePlayerMovement(true);
            }
        }
    }

    public void ToggleMenu(bool state)
    {
        menuPanelToggle.SetActive(state);
        menuOptionsPanelToggle.SetActive(state);
        _menuState = state;
    }

    private void HandleOptionUpdate(bool direction)
    {
        if (direction == false)
        {
            ++_selectedOption;
        }
        else if (direction == true)
        {
            --_selectedOption;
        }

        _selectedOption = Mathf.Clamp(_selectedOption, 0, menuOptions.Length - 1);

        UpdateOptionSelection();
    }

    private void UpdateOptionSelection()
    {
        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (i == _selectedOption)
            {
                menuOptions[i].color = Color.red;
            }
            else
            {
                menuOptions[i].color = Color.black;
            }
        }
    }

    private void OpenMenuPanel(int x) // Only activate the panel that you are currently looking at
    {
        _optionOpenState = true;

        menuOptionsPanelToggle.SetActive(false);

        foreach (GameObject go in menuPanels)
        {
            go.SetActive(false);
        }

        menuPanels[x].SetActive(true);

        if (x == 0)
        {
            PartyManager.instance.PartyUI.ResetToHandler();
        }
        else if (x == 1)
        {
            InventoryManager.instance.InventoryUI.ResetChosenListItem();
        }
        else if (x == 2)
        {
            SettingsManager.instance.SettingsUI.ResetChosenOption();
        }
    }
}