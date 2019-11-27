using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentencePanel : RootPanelMgr
{
    public Transform transPanelCont;

    List<Panel> lPanels = new List<Panel>();
    GameObject prefPanel;

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
        (panel as CommonPanel).Init(this);

        if (panel)
            lPanels.Add(panel);

        CanvasMgr.Instance.RefreshCanvas();
    }
}
