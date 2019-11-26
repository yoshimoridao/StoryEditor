using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributePanel : RootPanelMgr
{
    public string prefPanelPath = "Prefabs/Panel";
    public Transform transPanelCont;

    List<PanelMgr> lPanels = new List<PanelMgr>();
    GameObject prefPanel;

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
        prefPanel = Resources.Load<GameObject>(prefPanelPath);

        // clear all child
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }

    public void AddPanel()
    {
        PanelMgr panel = Instantiate(prefPanel, transPanelCont).GetComponent<PanelMgr>();
        panel.Init(this);

        if (panel)
            lPanels.Add(panel);

        CanvasMgr.RefreshCanvas();
    }
}
