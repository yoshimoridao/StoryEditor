using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryBoard : Board
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
        prefPanel = Resources.Load<GameObject>(DataDefine.pref_path_storyPanel);

        // load data
        List<DataIndex> dataIndexes = DataMgr.Instance.Stories;
        for (int i = 0; i < dataIndexes.Count; i++)
        {
            DataIndex dataIndex = dataIndexes[i];
            // create panel
            Panel panel = AddPanel(dataIndex.genKey) as Panel;

            // create label elements
            if (panel)
            {
                panel.Title = dataIndex.title;                          // load title
                panel.Color = (ColorBar.ColorType)dataIndex.colorId;    // load color
                panel.IsTesting = DataMgr.Instance.TestCases.Contains(dataIndex.genKey);                     // load testing flag

                // gen labels
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    string var = dataIndex.elements[j];
                    panel.AddLabel(var);
                }
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
            string title = DataDefine.default_name_story_panel;
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
