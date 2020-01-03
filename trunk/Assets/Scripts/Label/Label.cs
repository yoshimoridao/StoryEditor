﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Label : MonoBehaviour
{
    public Action<Label> actEditDone;
    public Action actEditing;

    // prop
    protected Image image;
    protected InputField inputField;
    protected ContentSizeFitter contentSize;

    // var
    [SerializeField]
    protected bool isEditing;
    protected string pureText;
    protected Panel panel;
    protected ColorBar.ColorType color = ColorBar.ColorType.WHITE;

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
    public void Start()
    {
        if (image == null)
            image = GetComponent<Image>();
        if (inputField == null)
            inputField = GetComponent<InputField>();
        if (contentSize == null)
            contentSize = GetComponent<ContentSizeFitter>();
    }

    public void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init(Panel _panel, string _text)
    {
        if (image == null)
            image = GetComponent<Image>();
        if (inputField == null)
            inputField = GetComponent<InputField>();
        if (contentSize == null)
            contentSize = GetComponent<ContentSizeFitter>();

        panel = _panel;
        // set default text of label
        if (_text.Length == 0)
            _text = DataDefine.defaultLabelVar;

        PureText = _text;

        // * fix bug: input field call OnEditing() but not OnEditDone() when init
        if (isEditing)
            isEditing = false;
    }

    public virtual void OnEditDone()
    {
        if (!isEditing || !gameObject.active)
            return;

        isEditing = false;

        RefreshContentSize();

        // call action func
        if (actEditDone != null)
            actEditDone(this);
    }

    public void OnEditing()
    {
        if (!contentSize)
            return;

        if (!isEditing)
            isEditing = true;

        RefreshContentSize();

        // call action func
        if (actEditing != null)
            actEditing();
    }

    public virtual void SelfDestroy()
    {
        if (panel)
            panel.RemoveLabel(this);
        Destroy(gameObject);
    }
    // ========================================= PROTECTED FUNCS =========================================
    protected void RefreshContentSize()
    {
        // to refresh size of content
        contentSize.enabled = false;
        contentSize.enabled = true;
    }
}
