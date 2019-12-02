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
        if (referPanel)
        {
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
            if (GetColorType() != referPanel.GetColorType())
            {
                SetColor(referPanel.GetColorType());
            }
        }
        // finding refer panel
        else
        {
            Panel panel = (CanvasMgr.Instance.GetBoard<ElementBoard>() as ElementBoard).GetPanel(referralKey);
            if (panel == null)
                panel = (CanvasMgr.Instance.GetBoard<StoryBoard>() as StoryBoard).GetPanel(referralKey);

            if (panel)
                referPanel = panel as CommonPanel;
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(Panel parent, CommonPanel panel)
    {
        base.Init(parent);

        // store reference panel
        referPanel = panel;

        // set title equal to the one of reference label
        referralKey = referPanel.titleLabel.GetText();
        SetText(referralKey);

        // set color
        SetColor(referPanel.GetColorType());
    }

    public void Init(Panel parent, string panelKey)
    {
        base.Init(parent);

        // store reference panel
        referPanel = null;

        // set title equal to the one of reference label
        referralKey = panelKey;
        SetText(referralKey);
    }
}
