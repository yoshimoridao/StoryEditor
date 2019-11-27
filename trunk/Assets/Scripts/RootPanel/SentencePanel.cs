using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentencePanel : RootPanelMgr
{
    public string prefPanelPath = "Prefabs/Panel";
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
        Panel panel = Instantiate(prefPanel, transPanelCont).GetComponent<Panel>();
        (panel as CommonPanel).Init(this);

        if (panel)
            lPanels.Add(panel);

        CanvasMgr.Instance.RefreshCanvas();
    }
}
