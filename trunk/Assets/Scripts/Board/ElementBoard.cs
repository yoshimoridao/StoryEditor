using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : Board
{
    public Transform transPanelCont;

    string defaultNameNewPanel = "element";
    List<Panel> lPanels = new List<Panel>();
    GameObject prefPanel;
    int panelCounter = 0;

    // ========================================= GET/ SET =========================================
    public List<Panel> GetPanels()
    {
        return lPanels;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        instance = this;
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init()
    {
        base.Init();
        instance = this;

        // load prefab
        prefPanel = Resources.Load<GameObject>(DataConfig.prefPanelPath);

        // clear all child
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }

    public void AddPanel()
    {
        Panel panel = Instantiate(prefPanel, transPanelCont).GetComponent<Panel>();

        if (panel)
        {
            (panel as CommonPanel).Init(this, defaultNameNewPanel + "_" + panelCounter);
            panelCounter++;

            lPanels.Add(panel);
            CanvasMgr.Instance.RefreshCanvas();
        }
    }
}
