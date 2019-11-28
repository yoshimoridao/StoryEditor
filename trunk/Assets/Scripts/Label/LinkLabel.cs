using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkLabel : Label
{
    CommonPanel referPanel;

    // ========================================= GET/ SET =========================================
    public CommonPanel GetReferPanel()
    {
        return referPanel;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {

    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RowLabelMgr rowLabel, CommonPanel panel)
    {
        base.Init(rowLabel);

        // store reference panel
        referPanel = panel;
        SetText(referPanel.titleLabel.GetTextObject().text);
    }
}
