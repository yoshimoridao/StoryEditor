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
    public bool isTest;
    public List<string> elements = new List<string>();
    public List<int> testElements = new List<int>();

    public Action actModifyData;

    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            actModifyData();
        }
    }

    public int Color
    {
        get { return colorId; }
        set
        {
            colorId = value;
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
    //public List<string> GetTestingElement()
    //{
    //    List<string> testingElements = new List<string>();
    //    for (int i = 0; i < testingIndex.Count; i++)
    //    {
    //        int testingId = testingIndex[i];
    //        if (testingId < elements.Count)
    //            testingElements.Add(elements[testingId]);
    //    }

    //    return testingElements;
    //}
}
