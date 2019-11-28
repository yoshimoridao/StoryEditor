using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputLabel : Label
{
    bool isChangeVal = false;
    [SerializeField]
    bool isTitleLabel = false;
    string oldKey = "";

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

        oldKey = GetText();
    }

    public void OnEditDone()
    {
        if (!isChangeVal)
            return;

        isChangeVal = false;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        CanvasMgr.Instance.RefreshCanvas();

        // call event to parent
        if (rowParent)
            rowParent.OnChildLabelEditDone();

        // modified key in storage
        if (isTitleLabel)
            DataMgr.Instance.ReplaceKey(oldKey, GetText());

        // update old key
        oldKey = GetText();
    }

    public void OnChangeValue()
    {
        if (!contentSize)
            return;

        isChangeVal = true;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        CanvasMgr.Instance.RefreshCanvas();
    }
}
