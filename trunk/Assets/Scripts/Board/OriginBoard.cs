using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginBoard : Board
{
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
        prefOriginPanel = Resources.Load<GameObject>(DataConfig.prefOriginPanelPath);
        prefLabel = Resources.Load<GameObject>(DataConfig.prefLabelPath);

        // clear all child
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }

    public void ShowResult(CommonPanel panel)
    {
        // show title
        string valText = panel.GetTitleObj().GetTextObject().text;
        sentencePanel.SetValue(valText);

        // show result
        
        List<Label> labels = panel.GetLabels();
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            if (label is InputLabel)
            {
                // create simple label
                Label genLabel = Instantiate(prefLabel, transPanelCont).GetComponent<Label>();
                genLabel.Init();
                genLabel.SetText(label.GetTextObject());
            }
            else if (label is LinkLabel)
            {
                // create origin label (nest labels)
                OriginPanel genPanel = Instantiate(prefOriginPanel, transPanelCont).GetComponent<OriginPanel>();
                genPanel.Init();
                // change title for this panel
                CommonPanel referPanel = (label as LinkLabel).GetReferPanel();
                genPanel.SetTitle(referPanel.GetTitleLabel().GetTextObject().text);
                genPanel.SetColor(referPanel.GetColor());

                // add child labels for genereted panel
                AddChildPanel(genPanel, label as LinkLabel);
            }
        }

        CanvasMgr.Instance.RefreshCanvas();
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void AddChildPanel(OriginPanel originPanel, LinkLabel linkLabel)
    {
        CommonPanel comPanel = linkLabel.GetReferPanel();

        // foreach all of labels
        List<Label> labels = comPanel.GetLabels();
        // in case in which lowest/ pure referral element (this mean the element(panel) have no another link label)
        bool isPureElement = true;
        foreach (Label child in labels)
        {
            if (child is LinkLabel)
            {
                isPureElement = false;
                break;
            }
        }

        // process for pure element -> pick random val
        if (isPureElement)
        {
            int rdId = Random.Range(0, labels.Count);
            if (rdId < labels.Count)
                originPanel.AddLabel(labels[rdId]);
            return;
        }

        // process for un-pure element
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];

            // add normal label
            if (label is InputLabel)
            {
                originPanel.AddLabel(label);
            }
            // add origin panel (for linking label)
            else if (label is LinkLabel)
            {
                CommonPanel referPanel = (label as LinkLabel).GetReferPanel();

                OriginPanel genPanel = originPanel.AddOriginPanel(referPanel);
                // loop add all labels of the generated panel
                AddChildPanel(genPanel, label as LinkLabel);
            }
        }
    }
}
