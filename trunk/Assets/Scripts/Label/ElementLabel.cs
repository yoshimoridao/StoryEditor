using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ElementLabel : ReactLabel
{
    public Image hlTesting;

    // ========================================= PROPERTIES =========================================
    public bool IsTesting
    {
        get
        {
            if (dataIndex != null)
                return dataIndex.isTest;

            return false;
        }
        set
        {
            if (dataIndex == null)
                return;
            dataIndex.isTest = value;

            if (hlTesting)
                hlTesting.gameObject.SetActive(value);
        }
    }

    public override void SetDataElementIndex(DataElementIndex _data)
    {
        base.SetDataElementIndex(_data);

        if (dataIndex == null)
            return;

        // is test
        IsTesting = dataIndex.isTest;

        // register event tag's event 
        if (dataIndex.eventTagKeys.Length > 0)
        {
            List<string> tagKeys = dataIndex.GetEventTagKeys();
            foreach (string tagKey in tagKeys)
            {
                RegisterEventTagEvent(tagKey, true);
            }
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        base.Start();

        if (inputField)
            originColor = inputField.GetComponent<Image>().color;
    }

    public void Update()
    {
        base.Update();
    }
    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(Panel _panel, string _text)
    {
        base.Init(_panel, _text);
    }

    public override void ConvertToShowText()
    {
        base.ConvertToShowText();

        // show text
        if (inputField == null)
            return;

        // add tag key before show text
        string eventText = GetEventTagText();
        if (eventText.Length > 0)
        {
            string showText = eventText + " " + inputField.text;
            inputField.text = showText;
        }
    }

    // ======================= Event Tag =======================
    public void ActiveEventTag(string _tagKey, bool _isActive)
    {
        if (dataIndex == null)
            return;

        // add event tag
        if (_isActive)
        {
            dataIndex.AddEventTag(_tagKey);

            // register event tag's event
            RegisterEventTagEvent(_tagKey, _isActive);
        }
        // remove event tag
        else if (dataIndex != null && dataIndex.IsContainEventTag(_tagKey))
        {
            RemoveEventTag(_tagKey);
        }

        ConvertToShowText();
    }

    private void RemoveEventTag(string _tagKey)
    {
        // un-register event tag's event
        RegisterEventTagEvent(_tagKey, false);

        // remove event tag in data
        dataIndex.RemoveEventTag(_tagKey);
    }

    private void RegisterEventTagEvent(string _tagKey, bool _isRegister)
    {
        EventTagId eventTagId = DataMgr.Instance.GetEventTag(_tagKey);
        if (eventTagId == null)
            return;

        if (_isRegister)
        {
            // register event tag's event
            eventTagId.actOnModyingData += OnModifiedEventTag;
            eventTagId.actOnDestroy += OnDestroyTag;
        }
        else
        {
            // un-register event tag's event
            eventTagId.actOnModyingData -= OnModifiedEventTag;
            eventTagId.actOnDestroy -= OnDestroyTag;
        }
    }

    private string GetEventTagText()
    {
        if (dataIndex == null)
            return "";

        string result = "";

        // get tag keys
        List<string> tagKeys = dataIndex.GetEventTagKeys();
        foreach (string tagKey in tagKeys)
        {
            // get event tag
            EventTagId eventTagId = DataMgr.Instance.GetEventTag(tagKey);
            if (eventTagId != null && eventTagId.IsVisible)
            {
                string tmp = TextUtil.AddColorTag(Color.blue, "@" + eventTagId.value);
                result += tmp + " ";
            }
        }

        // add size
        if (result.Length > 0)
            result = TextUtil.AddSizeTag(DataMgr.Instance.NormalFontSize - 5, result);

        return result;
    }

    public void OnModifiedEventTag()
    {
        ConvertToShowText();
    }

    public void OnDestroyTag(string _tagKey)
    {
        if (dataIndex == null)
            return;

        List<string> tagKeys = dataIndex.GetEventTagKeys();
        if (tagKeys.Contains(_tagKey))
        {
            RemoveEventTag(_tagKey);

            // refresh show text
            ConvertToShowText();
        }
    }
}
