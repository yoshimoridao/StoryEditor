using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputLabel : Label
{
    bool isChangeVal = false;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    public void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RowLabelMgr rowLabel)
    {
        base.Init(rowLabel);
    }

    public void OnEditDone()
    {
        if (!isChangeVal)
            return;

        isChangeVal = false;
        contentSize.enabled = false;
        contentSize.enabled = true;

        CanvasMgr.Instance.RefreshCanvas();

        // call event to parent
        if (rowParent)
            rowParent.OnChildLabelEditDone();
    }

    public void OnChangeValue()
    {
        if (!contentSize)
            return;

        isChangeVal = true;

        contentSize.enabled = false;
        contentSize.enabled = true;

        CanvasMgr.Instance.RefreshCanvas();
    }
}
