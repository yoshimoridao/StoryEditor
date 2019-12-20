using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputLabel : Label
{
    public bool isTitleLabel = false;

    private bool isEditing = false;

    // variables for nested label
    private List<string> referElementNames = new List<string>();
    private List<Panel> referElements = new List<Panel>();

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
                (panelParent as CommonPanel).OnChildLabelEdited(this);
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

    public void AddReferPanel(Panel panel)
    {
        ColorBar.ColorType panelColor = panel.GetColorType();

        string panelTitle = panel.GetTitle();
        // append link tag to content of text
        string val = GetText();
        val += " " + TextUtil.GetOpenColorTag(panelColor) + panelTitle + TextUtil.GetCloseColorTag();
        SetText(val);

        // store referral panel
        referElementNames.Add(panelTitle);
        referElements.Add(panel);

        // callback to parent -> save val
        if (panelParent)
            (panelParent as CommonPanel).OnChildLabelEdited(this);
    }
}
