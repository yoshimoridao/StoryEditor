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
        base.Start();
    }

    void Update()
    {
        base.Update();

        // update text following referral panel
        if (referralKey.Length > 0)
        {
            string referralTitle = referPanel.titleLabel.GetText();

            if (referralTitle.Length > 0 && referralKey != referralTitle)
            {
                referralKey = referralTitle;
                SetText(referralTitle);
            }
        }
        // update color following color of referral panel
        if (referPanel && GetColor() != referPanel.GetColor())
        {
            SetColor(referPanel.GetColor());
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RowLabelMgr rowLabel, CommonPanel panel)
    {
        base.Init(rowLabel);

        // store reference panel
        referPanel = panel;
        
        // set title equal to the one of reference label
        referralKey = referPanel.titleLabel.GetText();
        SetText(referralKey);

        // set color
        SetColor(referPanel.GetColor());
    }
}
