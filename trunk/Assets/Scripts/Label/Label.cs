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
    protected Image image;

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

    public Color GetColor()
    {
        return image.color;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();
    }

    public void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(string name = "")
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        // set text of label
        if (name.Length == 0)
            name = DataConfig.defaultLabelVar;
        SetText(name);

        GetTextObject().transform.localScale = Vector3.one * scaleText;
    }

    public void Init(RowLabelMgr rowLabel, string name = "")
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        rowParent = rowLabel;

        // set text of label
        if (name.Length == 0)
            name = DataConfig.defaultLabelVar;
        SetText(name);

        GetTextObject().transform.localScale = Vector3.one * scaleText;
    }
}
