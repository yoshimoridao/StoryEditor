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
    public override void Load()
    {
        base.Load();

        // load data
        List<DataIndex> dataIndexes = DataMgr.Instance.Elements;
        for (int i = 0; i < dataIndexes.Count; i++)
        {
            DataIndex dataIndex = dataIndexes[i];

            ElementPanel panel = null;
            // create panel
            if (i >= panels.Count)
            {
                panel = AddPanel(dataIndex) as ElementPanel;
            }
            // get already exist panel
            else
            {
                panel = panels[i] as ElementPanel;
                // re-load dataindex
                panel.SetDataIndex(dataIndex);
            }

            // create label elements
            if (panel)
            {
                // clear all of test labels
                //panel.ClearTestLabels();

                // gen labels
                List<Label> labels = panel.Labels;
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    Label genLabel = null;
                    // add new label
                    if (j >= labels.Count)
                    {
                        genLabel = panel.AddLabel("", false);
                    }
                    // or get exist label
                    else
                    {
                        genLabel = labels[j];
                    }

                    if (genLabel && genLabel is ReactLabel)
                    {
                        DataElementIndex dataElementId = dataIndex.elements[j];
                        (genLabel as ReactLabel).SetDataElementIndex(dataElementId);
                    }

                    //// store testing elements
                    //if (genLabel && genLabel is ElementLabel && dataIndex.testElements.Contains(j))
                    //    panel.AddTestLabel(genLabel as ElementLabel);
                }

                // set active highlight for all testing labels
                //panel.ActiveTestLabels();

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

        // refresh content
        StartCoroutine("RefreshContent");
        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }
}
