﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataIndex
{
    public string genKey;
    public string title;
    public int colorId;
    public List<string> elements = new List<string>();

    public DataIndex() { }
    public DataIndex(Panel _panel)
    {

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