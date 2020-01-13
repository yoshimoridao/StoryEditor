﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : Board
{
    // ========================================= GET/ SET =========================================
    [SerializeField]
    private RectTransform panelviewRt;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init()
    {
        base.Init();

        // load prefab
        prefPanel = Resources.Load<GameObject>(DataDefine.pref_path_elementPanel);

        // load data
        Load();

        // scale height for all ratio
        float canvasHeight = (CanvasMgr.Instance.transform as RectTransform).sizeDelta.y;
        RectTransform rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (rt.sizeDelta.y / 1080) * canvasHeight);
        // scale height for panel's view
        if (panelviewRt)
            panelviewRt.sizeDelta = new Vector2(panelviewRt.sizeDelta.x, (panelviewRt.sizeDelta.y / 1080) * canvasHeight);
    }

    public override void Load()
    {
        base.Load();

        // load data
        List<DataIndex> dataIndexes = DataMgr.Instance.Elements;
        for (int i = 0; i < dataIndexes.Count; i++)
        {
            DataIndex dataIndex = dataIndexes[i];

            ElementPanel panel = null;
            // create panel
            if (i >= panels.Count)
                panel = AddPanel(dataIndex.genKey) as ElementPanel;
            // get already exist panel
            else
                panel = panels[i] as ElementPanel;

            // create label elements
            if (panel)
            {
                panel.Key = dataIndex.genKey;                                               // load gen key
                panel.Title = dataIndex.title;                                              // load title
                panel.Color = (ColorBar.ColorType)dataIndex.colorId;                        // load color
                panel.IsTesting = DataMgr.Instance.TestCases.Contains(dataIndex.genKey);    // load testing flag

                // clear all of test labels
                panel.ClearTestLabels();
                // gen labels
                List<Label> labels = panel.Labels;
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    string var = dataIndex.elements[j];
                    Label genLabel = null;
                    // add new label
                    if (j >= labels.Count)
                    {
                        genLabel = panel.AddLabel(var);
                    }
                    // or get exist label
                    else
                    {
                        genLabel = labels[j];
                        genLabel.PureText = var;
                    }

                    // store testing elements
                    if (genLabel && genLabel is ElementLabel && dataIndex.testElements.Contains(j))
                        panel.AddTestLabel(genLabel as ElementLabel);
                }

                // set active highlight for all testing labels
                panel.ActiveTestLabels();

                // delete excess labels
                if (dataIndex.elements.Count < labels.Count)
                {
                    int beginId = dataIndex.elements.Count;
                    for (int j = beginId; j < labels.Count; j++)
                        Destroy(labels[j].gameObject);
                    labels.RemoveRange(beginId, labels.Count - beginId);
                }
            }
        }

        // delete excess panels
        if (dataIndexes.Count < panels.Count)
        {
            int beginId = dataIndexes.Count;
            for (int i = beginId; i < panels.Count; i++)
                Destroy(panels[i].gameObject);
            panels.RemoveRange(beginId, panels.Count - beginId);
        }

        // refresh canvas
        CanvasMgr.Instance.RefreshCanvas();
    }

    public override Panel AddPanel(string _genKey)
    {
        if (!prefPanel)
            return null;

        // create new panel
        Panel panel = Instantiate(prefPanel, transPanelCont).GetComponent<Panel>();
        if (panel)
        {
            string title = DataDefine.default_name_element_panel;
            panel.Init(_genKey, title);
            // register action when panel is destroyed
            panel.actOnDestroy += RemovePanel;

            panels.Add(panel);

            // set index of adding element panel as last child
            if (transPlusPanel)
                transPlusPanel.transform.SetAsLastSibling();

            return panel;
        }

        return null;
    }
}
