using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UI.ModernUIPack;

public class FloatingPanelCmd : MonoBehaviour, IFloatingWindow
{
    enum Option { TEST, DELETE };
    UICustomDropdown dropdown;

    public List<GameObject> SelectedObjs { get; set; }
    public bool IsActiveWindow { get; set; }
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

    public void SetActiveWindow(bool _isActive, List<GameObject> _selectObjs)
    {
        // store obj
        SelectedObjs = _selectObjs;

        if (_isActive)
        {
            if (!IsActiveWindow)
            {
                dropdown.Animate();
            }
        }
        else
        {
            dropdown.Animate();
        }

        IsActiveWindow = _isActive;
    }

    public void OnWindowDisable()
    {
        IsActiveWindow = false;

        if (ActOnWindowDisable != null)
            ActOnWindowDisable.Invoke();
    }

    public void OnDropdownValChange(int _index)
    {
        Option selectOption = (Option)_index;
        foreach (var e in SelectedObjs)
        {
            switch (selectOption)
            {
                case Option.TEST:
                    if (e.GetComponent<Panel>())
                        e.GetComponent<Panel>().IsTesting = true;
                    if (e.GetComponent<ElementLabel>())
                        e.GetComponent<ElementLabel>().IsTesting = true;
                    break;
                case Option.DELETE:
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
