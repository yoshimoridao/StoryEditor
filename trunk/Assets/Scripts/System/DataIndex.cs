using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DataIndex
{
    // generated key
    public string genKey;
    public string title;
    public string rgbaColor;

    // this for more VALUES of ELEMENT but merge ONE for STORY's element
    public List<DataElementIndex> elements = new List<DataElementIndex>();

    // --- these actions don't save ---
    public Action actModifyData;    // modifying title || color
    public Action<string> actOnDestroy;

    // === getter/ setter ===
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

    public Color RGBAColor
    {
        get { return Util.ParseTextToColor(rgbaColor); }
        set
        {
            rgbaColor = Util.ParseColorToText(value);
            if (actModifyData != null)
                actModifyData();
        }
    }

    public DataIndex() { }

    public void OnDestroy()
    {
        if (actOnDestroy != null)
            actOnDestroy(genKey);
    }

    // === Element ===
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
