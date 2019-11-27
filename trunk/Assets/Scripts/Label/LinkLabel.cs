using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkLabel : Label
{
    PanelMgr referPanel;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {

    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RowLabelMgr rowLabel, PanelMgr panel)
    {
        base.Init(rowLabel);

        // store reference panel
        referPanel = panel;
        SetText(referPanel.titleLabel.GetTextObj().text);
    }
}
