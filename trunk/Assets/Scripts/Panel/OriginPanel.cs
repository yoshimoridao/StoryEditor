﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginPanel : Panel
{
    public Text titleText;
    public Text valText;

    // ========================================= GET/ SET =========================================
    public void SetValue(string val)
    {
        valText.text = val;
    }
    public void SetTitle(string val)
    {
        titleText.text = val;
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
        base.Init();
    }

    public void AddLabel(Label label)
    {
        // create simple label
        GameObject prefLabel = Resources.Load<GameObject>(DataConfig.prefLabelPath);
        if (prefLabel)
        {
            Label genLabel = Instantiate(prefLabel, transLabelCont).GetComponent<Label>();
            genLabel.Init();
            genLabel.SetText(label.GetTextObject());
        }
    }

    public OriginPanel AddOriginPanel(CommonPanel panel)
    {
        GameObject prefOriginPanel = Resources.Load<GameObject>(DataConfig.prefOriginPanelPath);
        if (prefOriginPanel)
        {
            // add child panel
            OriginPanel genPanel = Instantiate(prefOriginPanel, transLabelCont).GetComponent<OriginPanel>();
            genPanel.Init();
            // change title
            genPanel.SetTitle(panel.GetTitleLabel().GetTextObject().text);

            return genPanel;
        }
        return null;
    }
}