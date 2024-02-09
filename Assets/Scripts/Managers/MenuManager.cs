using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public bool _menuState;

    public GameObject[] menuPanels;
    // 0 = partyPanel
    // 1 = inventoryPanel
    // 2 = settingsPanel

    public int _selectedPanel;

    // Convert to Singleton
    public static MenuManager instance = null; // public static means that it can be accessed

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
        _menuState = false;
        _selectedPanel = 0;
        ActivateMenuPanel(_selectedPanel);

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_menuState)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                GoToPrevPanel();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                GoToNextPanel();
            }
        }
    }

    public void ToggleMenu(bool state)
    {
        gameObject.SetActive(state);

        _menuState = state;
    }
    private void ActivateMenuPanel(int x) // Only activate the panel that you are currently looking at
    {
        foreach (GameObject go in menuPanels)
        {
            go.SetActive(false);
        }

        menuPanels[x].SetActive(true);
    }

    public void GoToNextPanel()
    {
        _selectedPanel++;

        if (_selectedPanel >= menuPanels.Length)
        {
            _selectedPanel = menuPanels.Length - 1; // instead of 0
        }

        ActivateMenuPanel(_selectedPanel);
    }

    public void GoToPrevPanel()
    {
        _selectedPanel--;

        if (_selectedPanel < 0)
        {
            _selectedPanel = 0; // instead of menuPanels.Length - 1
        }

        ActivateMenuPanel(_selectedPanel);
    }
}