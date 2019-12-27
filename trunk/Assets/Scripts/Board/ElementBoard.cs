using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : Board
{
    // ========================================= GET/ SET =========================================

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
        List<DataIndex> dataIndexes = DataMgr.Instance.Elements;
        for (int i = 0; i < dataIndexes.Count; i++)
        {
            DataIndex dataIndex = dataIndexes[i];
            // create panel
            ElementPanel panel = AddPanel(dataIndex.genKey) as ElementPanel;

            // create label elements
            if (panel)
            {
                panel.Title = dataIndex.title;                          // load title
                panel.Color = (ColorBar.ColorType)dataIndex.colorId;    // load color
                panel.IsTesting = dataIndex.isTest;                     // load testing flag

                // gen labels
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    string var = dataIndex.elements[j];
                    Label genLabel = panel.AddLabel(var);

                    // store testing elements
                    if (dataIndex.testElements.Contains(j) && genLabel && genLabel is ElementLabel)
                        panel.AddTestLabel(genLabel as ElementLabel);
                }

                // set active highlight for all testing labels
                panel.ActiveTestLabels();
            }
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
            panel.Init(this, _genKey, title);

            panels.Add(panel);

            // set index of adding element panel as last child
            if (transPlusPanel)
                transPlusPanel.transform.SetAsLastSibling();

            return panel;
        }

        return null;
    }
}
