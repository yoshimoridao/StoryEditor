using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputLabel : Label
{
    public ContentSizeFitter contentSize;
    public Vector2 offset = new Vector2(20, 20);    // pixel

    bool isChangeVal = false;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
        
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
        rt.sizeDelta = new Vector2(rt.sizeDelta.x + offset.x, rt.sizeDelta.y + offset.y);

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
