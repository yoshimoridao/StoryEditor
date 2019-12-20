using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataIndex
{
    public string key;
    public List<string> elements = new List<string>();
    public int colorId;
    public bool isStoryElement = false;
    public List<int> testingIndex = new List<int>();

    public DataIndex() { }
    public DataIndex(string _key, int _colorId, bool _isStoryElement = false)
    {
        key = _key;
        colorId = _colorId;
        isStoryElement = _isStoryElement;
    }

    public DataIndex(CommonPanel panel)
    {
        // get all text of label of panel
        key = panel.GetTitle();
        colorId = (int)panel.GetColorType();
        // determine panel is in which board (element or story)
        isStoryElement = panel.IsStoryElement();

        // add elements
        List<Label> labels = panel.GetLabels();
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            string var = "";
            if (label is LinkLabel)
            {
                var = "#" + label.GetText() + "#";
            }
            else
            {
                var = label.GetText();
            }

            elements.Add(var);
        }
    }

    // === element ===
    public void AddElement(string val)
    {
        elements.Add(val);
    }

    public void RemoveElement(int index)
    {
        if (index >= 0 && index < elements.Count)
            elements.RemoveAt(index);
    }

    public void ReplaceElement(int index, string val)
    {
        if (index >= 0 && index < elements.Count)
            elements[index] = val;
    }

    public void ReplaceElementPart(string oldVal, string newVal)
    {
        for (int j = 0; j < elements.Count; j++)
        {
            string element = elements[j];
            element = element.Replace(oldVal, newVal);

            // replace
            if (element.Length > 0)
            {
                elements[j] = element;
            }
            // remove element
            else
            {
                elements.RemoveAt(j);
                j--;
            }
        }
    }

    // === Testing Index ===
    public List<string> GetTestingElement()
    {
        List<string> testingElements = new List<string>();
        for (int i = 0; i < testingIndex.Count; i++)
        {
            int testingId = testingIndex[i];
            if (testingId < elements.Count)
                testingElements.Add(elements[testingId]);
        }

        return testingElements;
    }
}
