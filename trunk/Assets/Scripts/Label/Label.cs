using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
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

    public Text GetTextObj()
    {
        return GetComponentInChildren<Text>();
    }
    public void SetText(string val)
    {
        GetComponentInChildren<Text>().text = val;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Init(RowLabelMgr rowLabel)
    {
        rowParent = rowLabel;
        rt = GetComponent<RectTransform>();
    }
}
