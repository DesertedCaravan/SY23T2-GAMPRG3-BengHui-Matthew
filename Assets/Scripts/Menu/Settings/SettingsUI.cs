using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsUI : OptionsUIBase
{
    [Header("Settings UI Game Objects")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private List<TextMeshProUGUI> settingsUIList;

    int _selectedOption = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        ResetChosenOption();
    }

    // Update is called once per frame
    void Update()
    {
        if (settingsPanel.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.W) && _menuOptionActive == true)
            {
                SwitchChosenOption(false);
            }
            else if (Input.GetKeyDown(KeyCode.S) && _menuOptionActive == true)
            {
                SwitchChosenOption(true);
            }
            else if (Input.GetKeyDown(KeyCode.Q) && _menuOptionActive == true)
            {
                _menuOptionActive = false;
                CloseMenuOptionPanel();
            }
        }
    }

    public void ResetChosenOption()
    {
        _menuOptionActive = true;

        _selectedOption = 0;

        UpdateSelectedOption();
    }

    private void SwitchChosenOption(bool switchDirection)
    {
        if (switchDirection == false)
        {
            --_selectedOption;
        }
        else if (switchDirection == true)
        {
            ++_selectedOption;
        }

        _selectedOption = Mathf.Clamp(_selectedOption, 0, settingsUIList.Count - 1);

        UpdateSelectedOption();
    }

    private void UpdateSelectedOption() // Update Chosen List Name
    {
        for (int i = 0; i < settingsUIList.Count; i++)
        {
            if (i == _selectedOption)
            {
                settingsUIList[i].color = Color.red;
            }
            else
            {
                settingsUIList[i].color = Color.black;
            }
        }
    }

    protected override void CloseMenuOptionPanel()
    {
        base.CloseMenuOptionPanel();

        settingsPanel.SetActive(false);
    }
}
