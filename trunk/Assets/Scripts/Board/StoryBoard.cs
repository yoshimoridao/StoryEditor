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
        Load();
    }

    public override void Load()
    {
        base.Load();

        // load data
        List<DataIndex> dataIndexes = DataMgr.Instance.Stories;
        for (int i = 0; i < dataIndexes.Count; i++)
        {
            DataIndex dataIndex = dataIndexes[i];

            StoryPanel panel = null;
            // create panel
            if (i >= panels.Count)
                panel = AddPanel(dataIndex.genKey) as StoryPanel;
            // get already exist panel
            else
                panel = panels[i] as StoryPanel;

            // create label elements
            if (panel)
            {
                panel.Key = dataIndex.genKey;                                               // load gen key
                panel.Title = dataIndex.title;                                              // load title
                panel.Color = (ColorBar.ColorType)dataIndex.colorId;                        // load color
                panel.IsTesting = DataMgr.Instance.TestCases.Contains(dataIndex.genKey);    // load testing flag

                // gen labels
                List<Label> labels = panel.Labels;
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    string var = dataIndex.elements[j];
                    Label genLabel = null;
                    // add new label
                    if (j >= labels.Count)
                    {
                        genLabel = panel.AddLabel(var);
                    }
                    // or get exist label
                    else
                    {
                        genLabel = labels[j];
                        genLabel.PureText = var;
                    }
                }

                // delete excess labels
                if (dataIndex.elements.Count < labels.Count)
                {
                    int beginId = dataIndex.elements.Count;
                    for (int j = beginId; j < labels.Count; j++)
                        Destroy(labels[j].gameObject);
                    labels.RemoveRange(beginId, labels.Count - beginId);
                }
            }
        }

        // delete excess panels
        if (dataIndexes.Count < panels.Count)
        {
            int beginId = dataIndexes.Count;
            for (int i = beginId; i < panels.Count; i++)
                Destroy(panels[i].gameObject);
            panels.RemoveRange(beginId, panels.Count - beginId);
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
            panel.Init(_genKey, title);
            // register action when panel is destroyed
            panel.actOnDestroy += RemovePanel;

            panels.Add(panel);

            // set index of adding element panel as last child
            if (transPlusPanel)
                transPlusPanel.transform.SetAsLastSibling();

            return panel;
        }

        return null;
    }
}
