using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputLabel : Label
{
    public bool isTitleLabel = false;

    bool isEditing = false;
    string oldText = "";

    // ========================================= GET/ SET =========================================
    public bool IsEditing()
    {
        return isEditing;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    public void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(Panel panel, string name = "")
    {
        base.Init(panel, name);
    }

    public void OnEditDone()
    {
        if (!isEditing)
            return;

        isEditing = false;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        // Label is component or row
        if (!isTitleLabel)
        {
            if (panelParent)
                (panelParent as CommonPanel).OnChildLabelEditDone(this);
        }
    }

    public void OnChangeValue()
    {
        if (!contentSize)
            return;

        isEditing = true;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        // Label is component or row
        if (!isTitleLabel)
        {
            if (panelParent)
                (panelParent as CommonPanel).OnChildLabelEditing();
        }
    }
}
