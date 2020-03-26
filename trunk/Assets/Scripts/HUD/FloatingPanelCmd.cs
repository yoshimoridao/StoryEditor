using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UI.ModernUIPack;

public class FloatingPanelCmd : MonoBehaviour, IFloatingWindow
{
    UICustomDropdown dropdown;

    List<FloatingMenuConfig.FloatingItem> floatingItems = new List<FloatingMenuConfig.FloatingItem>();
    List<GameObject> selectedObjs = new List<GameObject>();
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

    public void ActiveWindow(List<FloatingMenuConfig.FloatingItem> _items, List<GameObject> _selectObjs)
    {
        // store obj
        selectedObjs = _selectObjs;
        floatingItems = _items;

        // setup drop down
        foreach (var item in floatingItems)
        {
            dropdown.SetItemTitle(item.item.itemName);
            dropdown.SetItemIcon(item.item.itemIcon);
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
        if (_index >= floatingItems.Count)
            return;

        FloatingMenuItem option = floatingItems[_index].itemType;
        foreach (var e in selectedObjs)
        {
            switch (option)
            {
                case FloatingMenuItem.TEST:
                    if (e.GetComponent<Panel>())
                        e.GetComponent<Panel>().IsTesting = !e.GetComponent<Panel>().IsTesting;
                    if (e.GetComponent<ElementLabel>())
                        e.GetComponent<ElementLabel>().IsTesting = !e.GetComponent<ElementLabel>().IsTesting;
                    break;
                case FloatingMenuItem.DELETE:
                    if (e.GetComponent<Panel>())
                        e.GetComponent<Panel>().SelfDestroy();
                    if (e.GetComponent<ReactLabel>())
                        e.GetComponent<ReactLabel>().SelfDestroy();
                    break;
                default:
                    break;
            }
        }
    }
}
