using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ElementLabel : ReactLabel
{
    public Image hlTesting;
    public Action<ElementLabel> actOnActiveTest;

    // event tag
    public Action<ElementLabel> actOnToggleEventTag;
    private List<string> eventTagKeys = new List<string>();

    // ========================================= PROPERTIES =========================================
    public List<string> EventTagKeys
    {
        get { return eventTagKeys; }
    }

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
                actOnActiveTest(this);
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

    // ======================= Event Tag =======================
    public void OnToggleEventTag(string _eventTagKey)
    {
        int findId = eventTagKeys.FindIndex(x => x == _eventTagKey);

        // remove if exist
        if (findId != -1)
        {
            eventTagKeys.RemoveAt(findId);
        }
        // add if not
        else
        {
            eventTagKeys.Add(_eventTagKey);
        }

        // call back
        if (actOnToggleEventTag != null)
            actOnToggleEventTag.Invoke(this);
    }
}
