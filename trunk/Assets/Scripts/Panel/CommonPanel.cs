﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonPanel : Panel
{
    public Label titleLabel;

    RootPanelMgr parentPanel;
    GameObject prefRowLabel;

    List<RowLabelMgr> lLabelRows = new List<RowLabelMgr>();
    bool isRefactorRows = false;
    float panelZoneW;

    // ========================================= GET/ SET =========================================
    public Label GetTitleLabel()
    {
        return titleLabel;
    }
    public List<Label> GetLabels()
    {
        List<Label> labels = new List<Label>();
        for (int i = 0; i < lLabelRows.Count; i++)
        {
            labels.AddRange(lLabelRows[i].GetLabels());
        }
        return labels;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {
        RefractorLabelRow();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RootPanelMgr rootPanel)
    {
        base.Init();

        // load prefab
        prefRowLabel = Resources.Load<GameObject>(DataConfig.prefRow);
        // store parent
        parentPanel = rootPanel;

        // calculate panel's zone
        RectOffset rootPadding = parentPanel.GetComponent<VerticalLayoutGroup>().padding;
        panelZoneW = (parentPanel.transform as RectTransform).sizeDelta.x - (rootPadding.left + rootPadding.right);

        // init title
        if (titleLabel)
            titleLabel.Init();
    }

    public void AddInputLabel()
    {
        // add new row if empty
        if (lLabelRows.Count == 0)
            AddLabelRow();

        if (lLabelRows.Count > 0)
        {
            // append label to last row
            RowLabelMgr rowLabel = lLabelRows[lLabelRows.Count - 1];
            if (rowLabel)
            {
                rowLabel.AddInputLabel();           // add input panel

                RefactorLabelRows();
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void AddLinkLabel(CommonPanel referPanel)
    {
        // add new row if empty
        if (lLabelRows.Count == 0)
            AddLabelRow();

        if (lLabelRows.Count > 0)
        {
            // append label to last row
            RowLabelMgr rowLabel = lLabelRows[lLabelRows.Count - 1];
            if (rowLabel)
            {
                rowLabel.AddLinkLabel(referPanel);     // add linking panel

                RefactorLabelRows();
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void RefactorLabelRows()
    {
        isRefactorRows = true;
    }

    // ========================================= OVERRIDE FUNCS =========================================
    public override Label GetTitleObj()
    {
        return titleLabel;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void RefractorLabelRow()
    {
        if (!CanvasMgr.Instance.IsRefreshCanvas())
        {
            if (isRefactorRows)
                isRefactorRows = false;
            return;
        }
        if (!isRefactorRows)
            return;

        for (int i = 0; i < lLabelRows.Count; i++)
        {
            RowLabelMgr row = lLabelRows[i];
            float rowW = (row.transform as RectTransform).sizeDelta.x;      // row's width
            float panelW = rt.sizeDelta.x;      // panel's width

            // if cont's width > panel's width
            if (rowW > panelW)
            {
                // extend cont's width
                if (rowW <= panelZoneW)
                {
                    rt.sizeDelta = new Vector2(rowW, rt.sizeDelta.y);
                }
                // retrieve last label of current row && add as first element to next row
                else
                {
                    if (panelW < panelZoneW)
                    {
                        rt.sizeDelta = new Vector2(panelZoneW, rt.sizeDelta.y);
                    }

                    if (row.ChildCount() > 1)
                    {
                        Label lastLabel = row.RetrieveLastLabel();

                        // add label as first element of next row
                        RowLabelMgr nextRow = null;
                        if (i + 1 == lLabelRows.Count)
                            nextRow = AddLabelRow();
                        else
                            nextRow = lLabelRows[i + 1];
                        nextRow.AddLabelAsFirst(lastLabel);
                    }
                }
            }
        }
    }

    private RowLabelMgr AddLabelRow()
    {
        if (prefRowLabel)
        {
            RowLabelMgr rowLabel = Instantiate(prefRowLabel, transLabelCont).GetComponent<RowLabelMgr>();
            rowLabel.Init(this);

            // store label cont
            lLabelRows.Add(rowLabel);
            return rowLabel;
        }

        return null;
    }
}
