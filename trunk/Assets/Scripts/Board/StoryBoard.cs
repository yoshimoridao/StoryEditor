using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryBoard : Board
{
    public Transform transPanelCont;

    string defaultNewPanelName = "story";
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

    public void AddPanel(string name = "")
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
        }
    }
}
