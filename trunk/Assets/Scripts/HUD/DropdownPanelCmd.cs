using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UI.ModernUIPack;

public class DropdownPanelCmd : MonoBehaviour, IFloatingWindow
{
    UICustomDropdown dropdown;

    List<FloatingMenuConfig.DropdownItem> dropdownItems = new List<FloatingMenuConfig.DropdownItem>();
    bool isActiveWindow = false;

    public Action ActOnWindowDisable { get; set; }

    void Start()
    {
        dropdown = GetComponent<UICustomDropdown>();
        dropdown.SetupDropdown();

        // register event for drop down
        dropdown.dropdownEvent.AddListener(OnDropdownValChange);
        dropdown.actOnOff += OnWindowDisable;
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        if (dropdown && dropdown.actOnOff != null)
            dropdown.actOnOff -= OnWindowDisable;
    }

    public void ActiveWindow(List<FloatingMenuConfig.DropdownItem> _items)
    {
        // store obj
        dropdownItems = _items;
        // clear all items of dropdown menu
        dropdown.ClearAllItems();

        // setup drop down
        for (int i = 0; i < dropdownItems.Count; i++)
        {
            var item = dropdownItems[i];
            dropdown.SetItemTitle(item.item.itemName);
            dropdown.SetItemIcon(item.item.itemIcon);
            dropdown.CreateNewItem();
        }
        dropdown.SetupDropdown();

        if (!isActiveWindow)
        {
            dropdown.Animate();
            isActiveWindow = true;
        }
    }

    public void DeactiveWindow()
    {
        dropdown.Animate();
        isActiveWindow = false;

    }

    public void OnWindowDisable()
    {
        isActiveWindow = false;

        if (ActOnWindowDisable != null)
            ActOnWindowDisable.Invoke();
    }

    public void OnDropdownValChange(int _index)
    {
        if (_index >= dropdownItems.Count)
            return;

        DropdownMenuItem option = dropdownItems[_index].itemType;
        switch (option)
        {
            case DropdownMenuItem.SAVE:
                DataMgr.Instance.SaveFile();
                break;
            case DropdownMenuItem.LOAD:
                DataMgr.Instance.LoadFile();
                break;
            case DropdownMenuItem.EXPORT_TRACERY:
                DataMgr.Instance.ExportTraceryFile();
                break;
            case DropdownMenuItem.EXPORT_CSV:
                DataMgr.Instance.ExportCSVFile();
                break;
            case DropdownMenuItem.IMPORT_CSV:
                DataMgr.Instance.ImportCSVFile();
                break;
            case DropdownMenuItem.COLOR:
                ColorMenu.Instance.OnPressColorBtn();
                break;
            case DropdownMenuItem.HOT_KEY:
                DisplayMgr.Instance.ShowInfoPanel(true);
                break;
            default:
                break;
        }
    }
}
