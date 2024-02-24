using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : OptionsUIBase
{
    [Header("Inventory Manager Updater")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Inventory UI Game Objects")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject pouchContent;
    [SerializeField] private ItemUI item;

    [SerializeField] private GameObject listName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite emptyItemIcon;

    int _selectedList = 0;
    int _selectedItem = 0;

    [SerializeField] private List<TextMeshProUGUI> itemLists;
    List<ItemUI> chosenItemList;

    // InventoryManager inventory;

    /*
    private void Awake()
    {
        inventory = InventoryManager.GetInventory();
    }
    */

    protected override void Start()
    {
        base.Start();

        ResetChosenListItem();
    }

    private void Update()
    {
        if (inventoryPanel.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.A) && _menuOptionActive == true)
            {
                SwitchList(false);
            }
            else if (Input.GetKeyDown(KeyCode.D) && _menuOptionActive == true)
            {
                SwitchList(true);
            }

            if (Input.GetKeyDown(KeyCode.S) && _menuOptionActive == true)
            {
                SwitchItem(false);
            }
            else if (Input.GetKeyDown(KeyCode.W) && _menuOptionActive == true)
            {
                SwitchItem(true);
            }
            else if (Input.GetKeyDown(KeyCode.Q) && _menuOptionActive == true)
            {
                _menuOptionActive = false;
                CloseMenuOptionPanel();
            }
        }
    }

    public void ResetChosenListItem()
    {
        _menuOptionActive = true;

        _selectedList = 0;
        _selectedItem = 0;

        UpdateSelectedList();
        UpdateListItems();
    }

    private void SwitchList(bool switchDirection)
    {
        if (switchDirection == false)
        {
            --_selectedList;
        }
        else if (switchDirection == true)
        {
            ++_selectedList;
        }

        _selectedList = Mathf.Clamp(_selectedList, 0, itemLists.Count - 1);
        _selectedItem = 0;

        UpdateSelectedList(); // Update Chosen List Name
        UpdateListItems(); // Update Chosen List being viewed
    }

    private void SwitchItem(bool direction)
    {
        int prevSelection = _selectedItem;

        if (direction == false)
        {
            ++_selectedItem;
        }
        else if (direction == true)
        {
            --_selectedItem;
        }

        _selectedItem = Mathf.Clamp(_selectedItem, 0, inventoryManager.SelectList(_selectedList).Count - 1);

        if (prevSelection != _selectedItem)
        {
            UpdateItemSelection();
        }
    }

    private void UpdateSelectedList() // Update Chosen List Name
    {
        for (int i = 0; i < itemLists.Count; i++)
        {
            if (i == _selectedList)
            {
                itemLists[i].color = Color.red;
            }
            else
            {
                itemLists[i].color = Color.black;
            }
        }
    }

    public void UpdateListItems() // Update Chosen List being viewed
    {
        // Clear all exisiting child Game Objects in the Pouch List Scrollview Content
        foreach (Transform child in pouchContent.transform)
        {
            Destroy(child.gameObject);
        }

        chosenItemList = new List<ItemUI>(); // Make new temporary list // Alternative is itemList.Clear();

        foreach (var slot in inventoryManager.SelectList(_selectedList)) // previously inventory.Slots
        {
            var itemDisplay = Instantiate(item, pouchContent.transform); // Instantiate Item Prefab
            itemDisplay.SetData(slot); // Call SetData function from Instantiated Item Prefab

            chosenItemList.Add(itemDisplay); // Add to ItemUI List
        }

        UpdateItemSelection();
    }

    private void UpdateItemSelection() // Update Chosen Item in Chosen List Content
    {
        for (int i = 0; i < chosenItemList.Count; i++)
        {
            if (i == _selectedItem)
            {
                chosenItemList[i].NameText.color = Color.red;
            }
            else
            {
                chosenItemList[i].NameText.color = Color.black;
            }
        }

        if (inventoryManager.SelectList(_selectedList).Count > 0) // Check if Chosen List has items
        {
            var item = inventoryManager.SelectList(_selectedList)[_selectedItem].GetItem;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        else // otherwise clear Item Icon and Item Description
        {
            itemIcon.sprite = emptyItemIcon;
            itemDescription.text = "";
        }
    }
    protected override void CloseMenuOptionPanel()
    {
        base.CloseMenuOptionPanel();

        inventoryPanel.SetActive(false);
    }
}