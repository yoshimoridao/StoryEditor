using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    public InputField inputField;
    public ContentSizeFitter contentSize;
    //public GameObject highlightPanel;

    // prop
    private RectTransform rt;
    private Image image;

    // var
    private Panel panel;
    private ColorBar.ColorType color = ColorBar.ColorType.WHITE;

    private bool isEditing = false;

    // ========================================= GET/ SET =========================================
    public Panel Panel
    {
        get { return panel; }
        set
        {
            panel = value;
            transform.parent = panel.transform;
        }
    }

    public string LabelText
    {
        get { return inputField.text; }
        set { inputField.text = value; }
    }

    public Text GetTextObject()
    {
        return GetComponentInChildren<Text>();
    }

    public ColorBar.ColorType Color
    {
        get { return color; }
        set
        {
            color = value;
            image.color = ColorBar.Instance.GetColor(color);
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();
    }

    public void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(Panel _panel, string _text)
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        // set text of label
        if (_text.Length == 0)
            _text = DataDefine.defaultLabelVar;
        LabelText = _text;
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
        if (panel)
            panel.OnChildLabelEdited(this);
    }

    public void OnEditing()
    {
        if (!contentSize)
            return;

        isEditing = true;

        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;

        // Label is component or row
        if (panel)
            panel.OnEditing();
    }

    public void AddReferalPanel(Panel panel)
    {
        //ColorBar.ColorType panelColor = panel.GetColorType();

        //string panelTitle = panel.Title();
        //// append link tag to content of text
        //string val = Text();
        //val += " " + TextUtil.GetOpenColorTag(panelColor) + panelTitle + TextUtil.GetCloseColorTag();
        //SetText(val);

        //// store referral panel
        //referElementNames.Add(panelTitle);
        //referElements.Add(panel);

        //// callback to parent -> save val
        //if (this.panel)
        //    (this.panel as Panel).OnChildLabelEdited(this);
    }

    public virtual void SelfDestroy()
    {
        if (panel)
            panel.RemoveLabel(this);
        Destroy(gameObject);
    }

    //public bool IsActiveHighlightPanel()
    //{
    //    if (highlightPanel)
    //        return highlightPanel.gameObject.active;

    //    return false;
    //}

    //public void SetActiveHighlightPanel(bool isActive)
    //{
    //    if (!highlightPanel)
    //        return;

    //    // set active highlight panel
    //    highlightPanel.gameObject.SetActive(isActive);

    //    // storing for parent panel
    //    if (panelParent && panelParent is ElementPanel)
    //    {
    //        if (isActive)
    //            (panelParent as ElementPanel).AddTestingLabel(this);
    //        else
    //            (panelParent as ElementPanel).RemoveTestingLabel(this);
    //    }
    //}
}
