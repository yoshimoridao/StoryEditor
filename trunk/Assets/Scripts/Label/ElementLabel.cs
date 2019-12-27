using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ElementLabel : ReactLabel
{
    // ========================================= PROPERTIES =========================================
    public Image hlTesting;
    public Action<ElementLabel> actOnActive;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        base.Start();
    }

    public void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public bool IsTesting
    {
        get
        {
            if (hlTesting)
                return hlTesting.gameObject.active;
            return false;
        }
        set
        {
            if (hlTesting)
            {
                hlTesting.gameObject.SetActive(value);

                // callback action
                actOnActive(this);
            }
        }
    }

    public void ActiveTesting(bool _isActive)
    {
        hlTesting.gameObject.SetActive(_isActive);
    }

    public override void Init(Panel _panel, string _text)
    {
        base.Init(_panel, _text);
    }
}
