using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginPanel : Panel
{
    public Text titleText;
    public Text valText;

    // ========================================= GET/ SET =========================================
    public void SetValue(string val)
    {
        valText.text = val;
    }

    public override void SetTitle(string val)
    {
        base.SetTitle(val);
        titleText.text = val;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        base.Init();
    }

    public void AddLabel(Label label)
    {
        // create simple label
        GameObject prefLabel = Resources.Load<GameObject>(DataDefine.pref_path_label);
        if (prefLabel)
        {
            Label genLabel = Instantiate(prefLabel, transLabelCont).GetComponent<Label>();
            genLabel.Init();
            genLabel.SetText(label.GetTextObject());
        }
    }

    public OriginPanel AddOriginPanel(CommonPanel panel)
    {
        GameObject prefOriginPanel = Resources.Load<GameObject>(DataDefine.pref_path_originPanel);
        if (prefOriginPanel)
        {
            // add child panel
            OriginPanel genPanel = Instantiate(prefOriginPanel, transLabelCont).GetComponent<OriginPanel>();
            genPanel.Init();
            // change title
            genPanel.SetTitle(panel.GetTitle());
            genPanel.SetColor(panel.GetColorType());

            return genPanel;
        }
        return null;
    }
}
