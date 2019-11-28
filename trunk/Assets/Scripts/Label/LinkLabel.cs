using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkLabel : Label
{
    CommonPanel referPanel;
    string referralKey = "";

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
        if (referralKey.Length > 0)
        {
            string referralVal = referPanel.titleLabel.GetText();
            if (referralVal.Length > 0 && referralKey != referralVal)
            {
                referralKey = referralVal;
                SetText(referralVal);
            }
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RowLabelMgr rowLabel, CommonPanel panel)
    {
        base.Init(rowLabel);

        // store reference panel
        referPanel = panel;
        
        referralKey = referPanel.titleLabel.GetText();
        SetText(referralKey);
    }
}
