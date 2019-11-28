using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    public InputField inputField;
    public ContentSizeFitter contentSize;
    public float scaleText = 0.8f;

    // prop
    protected RectTransform rt;
    protected RowLabelMgr rowParent;

    // ========================================= GET/ SET FUNCS =========================================
    public void SetParent(RowLabelMgr rowLabel, bool isAsFirstElement = false)
    {
        // set transform parent
        if (rowLabel)
        {
            transform.parent = rowLabel.transform;
            if (isAsFirstElement)
                transform.SetAsFirstSibling();

            // store parent
            rowParent = rowLabel;
        }
        else
        {
            transform.parent = null;
            rowParent = null;
        }
    }

    public RowLabelMgr GetParent()
    {
        return rowParent;
    }

    public Text GetTextObject()
    {
        return GetComponentInChildren<Text>();
    }

    public string GetText()
    {
        return inputField.text;
    }

    public void SetText(Text t)
    {
        inputField.text = t.text;
        GetComponentInChildren<Text>().fontSize = t.fontSize;
    }

    public void SetText(string val)
    {
        inputField.text = val;
    }

    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
        
    }

    public void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        rt = GetComponent<RectTransform>();

        GetTextObject().transform.localScale = Vector3.one * scaleText;
    }

    public void Init(RowLabelMgr rowLabel)
    {
        rowParent = rowLabel;
        rt = GetComponent<RectTransform>();

        GetTextObject().transform.localScale = Vector3.one * scaleText;
    }
}
