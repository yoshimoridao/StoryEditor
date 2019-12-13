using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : Board
{
    public Transform transPanelCont;

    string defaultNewPanelName = "element";
    [SerializeField]
    List<Panel> panels = new List<Panel>();
    GameObject prefPanel;
    int panelCounter = 0;

    // ========================================= GET/ SET =========================================
    public Panel GetPanel(string key)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            CommonPanel panel = panels[i] as CommonPanel;
            if (panel.GetTitle() == key)
            {
                return panel;
            }
        }
        return null;
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
        prefPanel = Resources.Load<GameObject>(DataDefine.pref_path_Panel);

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
            panels.Add(panel);

            CanvasMgr.Instance.RefreshCanvas();

            // save data in case just created
            DataMgr.Instance.AddIndex(panel as CommonPanel);
            return panel;
        }

        return null;
    }

    public bool RemovePanel(Panel panel)
    {
        int panelId = panels.FindIndex(x => x.GetTitle() == panel.GetTitle());
        // remove panel in list panels
        if (panelId > -1 && panelId < panels.Count)
        {
            panels.RemoveAt(panelId);

            // also remove in data storage
            DataMgr.Instance.RemoveIndex(DataIndexer.DataType.Element, panel.GetTitle());

            CanvasMgr.Instance.RefreshCanvas();
            return true;
        }

        return false;
    }

    public void OnAddBtnPressed()
    {
        AddPanel();
    }
}
