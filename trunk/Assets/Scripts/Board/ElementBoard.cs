﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : Board
{
    public Transform transPanelCont;

    string defaultNewPanelName = "element";
    [SerializeField]
    List<Panel> lPanels = new List<Panel>();
    GameObject prefPanel;
    int panelCounter = 0;

    // ========================================= GET/ SET =========================================
    public Panel GetPanel(string key)
    {
        for (int i = 0; i < lPanels.Count; i++)
        {
            CommonPanel panel = lPanels[i] as CommonPanel;
            if (panel.GetTitle() == key)
            {
                return panel;
            }
        }
        return null;
    }

    public List<Panel> GetPanels()
    {
        return new List<Panel>(lPanels);
    }

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
        prefPanel = Resources.Load<GameObject>(DataConfig.prefPanelPath);

        // clear all child
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }

    public Panel AddPanel(string name = "")
    {
        Panel panel = Instantiate(prefPanel, transPanelCont).GetComponent<Panel>();

        if (panel)
        {
            if (name.Length == 0)
                name = defaultNewPanelName + "_" + panelCounter;
            (panel as CommonPanel).Init(this, name);
            panelCounter++;
            lPanels.Add(panel);

            CanvasMgr.Instance.RefreshCanvas();

            // save data in case just created
            DataMgr.Instance.SaveDataInfo(panel as CommonPanel);

            return panel;
        }

        return null;
    }

    public void RemovePanel(Panel panel)
    {
        int panelId = lPanels.FindIndex(x => x.GetTitle() == panel.GetTitle());
        // remove panel in list panels
        if (panelId > -1 && panelId < lPanels.Count)
        {
            lPanels.RemoveAt(panelId);

            // also remove in data storage
            DataMgr.Instance.RemoveDataInfo(DataMgr.DataType.Element, panel.GetTitle());
        }

        CanvasMgr.Instance.RefreshCanvas();
    }

    public void OnAddBtnPressed()
    {
        AddPanel();
    }
}
