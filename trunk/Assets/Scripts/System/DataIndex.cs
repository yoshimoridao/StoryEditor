using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DataIndex
{
    public string genKey;
    public string title;
    public int colorId;
    public List<string> elements = new List<string>();
    public List<int> testElements = new List<int>();

    public Action actModifyData;

    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            if (actModifyData != null)
                actModifyData();
        }
    }

    public int Color
    {
        get { return colorId; }
        set
        {
            colorId = value;
            if (actModifyData != null)
                actModifyData();
        }
    }

    public DataIndex() { }
    public DataIndex(Panel _panel)
    {
        genKey = _panel.Key;
        title = _panel.Title;
        colorId = (int)_panel.Color;
    }

    // === element ===
    public void AddElement(string _val)
    {
        elements.Add(_val);
    }

    public void RemoveElement(int _index)
    {
        if (_index >= 0 && _index < elements.Count)
            elements.RemoveAt(_index);
    }

    public void ReplaceElement(int _index, string _val)
    {
        if (_index >= 0 && _index < elements.Count)
            elements[_index] = _val;
    }

    // === Testing Index ===
    public List<string> GetTestElements()
    {
        List<string> tmp = new List<string>();

        if (testElements.Count > 0)
        {
            for (int i = 0; i < testElements.Count; i++)
            {
                int testingId = testElements[i];
                if (testingId < elements.Count)
                    tmp.Add(elements[testingId]);
            }
        }
        else
        {
            return elements;
        }

        return tmp;
    }
}
