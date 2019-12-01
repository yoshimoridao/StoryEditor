using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputLabel : Label
{
    bool isModifyingText = false;

    // ========================================= GET/ SET =========================================
    public bool IsModifyingText()
    {
        return isModifyingText;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    public void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RowLabelMgr rowLabel, string name = "")
    {
        base.Init(rowLabel, name);
    }

    public void OnEditDone()
    {
        if (!isModifyingText)
            return;

        isModifyingText = false;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        // Label is component or row
        if (rowParent)
            rowParent.OnChildLabelEditDone();
    }

    public void OnChangeValue()
    {
        if (!contentSize)
            return;

        isModifyingText = true;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        CanvasMgr.Instance.RefreshCanvas();
    }
}
