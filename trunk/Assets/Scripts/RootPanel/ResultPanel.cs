using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPanel : RootPanelMgr
{
    public string prefOriginPanelPath = "Prefabs/panel_origin";
    public string prefLabelPath = "Prefabs/label";

    public Transform transPanelCont;
    public OriginPanel sentencePanel;

    List<Panel> lPanels = new List<Panel>();
    GameObject prefOriginPanel;
    GameObject prefLabel;

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
        prefOriginPanel = Resources.Load<GameObject>(prefOriginPanelPath);
        prefLabel = Resources.Load<GameObject>(prefLabelPath);

        // clear all child
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }
}
