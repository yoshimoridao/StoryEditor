using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UI.ModernUIPack;

public class FloatingPanelCmd : MonoBehaviour, IFloatingWindow
{
    enum Option { Test, Delete };
    UICustomDropdown dropdown;

    public GameObject SelectedObj { get; set; }
    public bool IsActiveWindow { get; set; }
    public Action ActOnWindowDisable { get; set; }

    void Start()
    {
        dropdown = GetComponent<UICustomDropdown>();
        dropdown.SetupDropdown();
        dropdown.dropdownEvent.AddListener(OnDropdownValChange);

        // register event for drop down
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

    public void SetActiveWindow(bool _isActive, GameObject _selectObj)
    {
        // store obj
        SelectedObj = _selectObj;

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

    }
}
