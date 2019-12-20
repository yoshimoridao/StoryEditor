using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Label : MonoBehaviour
{
    public InputField inputField;
    public ContentSizeFitter contentSize;
    public GameObject highlightPanel;
    public float scaleText = 0.98f;

    // prop
    protected RectTransform rt;
    protected Image image;
    [SerializeField]
    protected Panel panelParent;
    protected ColorBar.ColorType colorType = ColorBar.ColorType.WHITE;

    // ========================================= GET/ SET FUNCS =========================================
    public void SetParent(Panel panel, bool isAsFirstElement = false)
    {
        // set transform parent
        if (panel)
        {
            transform.parent = panel.transform;
            if (isAsFirstElement)
                transform.SetAsFirstSibling();

            // store parent
            panelParent = panel;
        }
        else
        {
            transform.parent = null;
            panelParent = null;
        }
    }

    public Panel GetParent()
    {
        return panelParent;
    }

    public Text GetTextObject()
    {
        return GetComponentInChildren<Text>();
    }

    public string GetText()
    {
        return inputField.text;
    }

    public void SetText(string val)
    {
        inputField.text = val;
    }

    public void SetText(Text t)
    {
        inputField.text = t.text;
        GetComponentInChildren<Text>().fontSize = t.fontSize;
    }

    public ColorBar.ColorType GetColorType()
    {
        return colorType;
    }

    public void SetColor(ColorBar.ColorType type)
    {
        if (image)
        {
            colorType = type;
            image.color = ColorBar.Instance.GetColor(type);
        }
    }
    // ========================================= UNITY FUNCS =========================================
    public void Start()
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
    public void Init(string name = "")
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        // set text of label
        if (name.Length == 0)
            name = DataDefine.defaultLabelVar;
        SetText(name);

        GetTextObject().transform.localScale = Vector3.one * scaleText;
    }

    public virtual void Init(Panel panel, string name = "")
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        panelParent = panel;

        // set text of label
        if (name.Length == 0)
            name = DataDefine.defaultLabelVar;
        SetText(name);

        GetTextObject().transform.localScale = Vector3.one * scaleText;
    }

    public virtual void SelfDestroy()
    {
        if (panelParent && panelParent is CommonPanel)
            (panelParent as CommonPanel).RemoveLabel(this);

        Destroy(gameObject);
    }

    // ========================================= ACTIVE FUNCS =========================================
    public bool IsActiveHighlightPanel()
    {
        if (highlightPanel)
            return highlightPanel.gameObject.active;

        return false;
    }

    public void SetActiveHighlightPanel(bool isActive)
    {
        if (!highlightPanel)
            return;

        // set active highlight panel
        highlightPanel.gameObject.SetActive(isActive);

        // storing for parent panel
        if (panelParent && panelParent is ElementPanel)
        {
            if (isActive)
                (panelParent as ElementPanel).AddTestingLabel(this);
            else
                (panelParent as ElementPanel).RemoveTestingLabel(this);
        }
    }
}
