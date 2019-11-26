using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributePanel : RootPanelMgr
{
    List<PanelMgr> lPanels = new List<PanelMgr>();

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

        // get template panel
        for (int i = 0; i < transform.childCount; i++)
        {
            PanelMgr panel = transform.GetChild(i).GetComponent<PanelMgr>();
            if (panel)
            {
                lPanels.Add(panel);
                panel.Init(this);
            }
        }
    }
}
