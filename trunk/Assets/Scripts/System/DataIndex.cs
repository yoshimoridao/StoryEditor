using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DataIndex
{
    // generated key
    public string genKey = "";
    public string title = "";
    public string rgbaColor = "r:1,g:1,b:1,a:1";
    public bool isTest = false;

    // this for more VALUES of ELEMENT but merge ONE for STORY's element
    public List<DataElementIndex> elements = new List<DataElementIndex>();

    // --- these actions don't save ---
    public Action actModifyData;            // modifying title || color (call back for other link labels)
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
        get
        {
            if (rgbaColor.Length == 0)
                return Color.white;

            return Util.ParseTextToColor(rgbaColor);
        }
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
    #region common
    public DataElementIndex AddElement(string _val)
    {
        DataElementIndex genElement = new DataElementIndex();
        genElement.value = _val;
        elements.Add(genElement);

        return genElement;
    }

    public void RemoveElement(int _id)
    {
        if (_id < elements.Count)
            elements.RemoveAt(_id);
    }
    public void RemoveElements(int _startId, int _count)
    {
        if (_startId < elements.Count && _startId + _count <= elements.Count)
            elements.RemoveRange(_startId, _count);
    }
    #endregion

    // === Testing Index ===
    #region test
    /// <summary>
    /// To get elements which picked to test by player
    /// </summary>
    /// <returns></returns>
    public List<DataElementIndex> GetTestElements()
    {
        List<DataElementIndex> vals = new List<DataElementIndex>();
        for (int i = 0; i < elements.Count; i++)
        {
            var tmpElement = elements[i];
            bool isTestElement = false;

            // if element has any event tags
            if (DataMgr.Instance.IsActiveTagTest)
            {
                List<string> eventTags = tmpElement.GetEventTagKeys();
                foreach (string eventTag in eventTags)
                {
                    // if event tag is enable test
                    if (DataMgr.Instance.IsTestingTag(eventTag))
                    {
                        isTestElement = true;
                        break;
                    }
                }
            }

            // if element is marked test
            if (!isTestElement && tmpElement.isTest && DataMgr.Instance.IsActiveSelectTest)
                isTestElement = true;

            if (isTestElement)
                vals.Add(tmpElement);
        }

        return vals;
    }
    #endregion
}
