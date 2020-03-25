using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Label : MonoBehaviour
{
    public Action<Label> actOnEditDone;
    public Action actOnEditing;

    // prop
    protected Image image;
    protected InputField inputField;
    //protected ContentSizeFitter contentSize;
    protected RectTransform rt;

    [SerializeField]
    protected Vector2 sizeOffset = new Vector2(7, 2);
    protected bool isEditing;
    protected string pureText;
    protected Panel panel;

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
    public virtual string PureText
    {
        get { return pureText; }
        set { pureText = value; }
    }
    public Image ImgLabel
    {
        get { return image; }
        set { image = value; }
    }
    public InputField Field { get { return inputField; } }

    public Text GetTextObject() { return GetComponentInChildren<Text>(); }

    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
        if (image == null)
            image = GetComponent<Image>();
        if (inputField == null)
            inputField = GetComponent<InputField>();
        if (rt == null)
            rt = GetComponent<RectTransform>();
        //if (contentSize == null)
        //{
        //    contentSize = GetComponent<ContentSizeFitter>();
        //    contentSize.enabled = false;
        //}
    }

    public void Update()
    {
    }

    public void OnDestroy()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init(Panel _panel, string _text)
    {
        if (image == null)
            image = GetComponent<Image>();
        if (inputField == null)
            inputField = GetComponent<InputField>();
        if (rt == null)
            rt = GetComponent<RectTransform>();
        //if (contentSize == null)
        //{
        //    contentSize = GetComponent<ContentSizeFitter>();
        //    contentSize.enabled = false;
        //}

        panel = _panel;
        //// set default text of label
        //if (_text.Length == 0)
        //    _text = DataDefine.defaultLabelVar;

        PureText = _text;
    }

    public void OnEditing()
    {
        if (!isEditing && inputField.isFocused)
            isEditing = true;

        RefreshContentSize();

        // call action func
        if (actOnEditing != null)
            actOnEditing();
    }

    public virtual void OnEditDone()
    {
        if (!isEditing || !gameObject.active)
            return;

        isEditing = false;

        RefreshContentSize();

        // call action func
        if (actOnEditDone != null)
            actOnEditDone(this);
    }

    public virtual void SelfDestroy()
    {
        if (panel)
            panel.RemoveLabel(this);
        Destroy(gameObject);
    }

    public virtual void RefreshContentSize()
    {
        // to refresh size of content
        //contentSize.enabled = false;
        //contentSize.enabled = true;

        if (inputField)
        {
            rt.sizeDelta = new Vector2(inputField.preferredWidth + sizeOffset.x * 2, inputField.preferredHeight + sizeOffset.y * 2);
        }
    }

    // ========================================= PROTECTED FUNCS =========================================
}
