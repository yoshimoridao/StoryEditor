using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReactLabel : Label, IPointerClickHandler
{
    protected List<DataIndex> referPanels = new List<DataIndex>();

    // ========================================= PROPERTIES =========================================
    public override string PureText
    {
        get { return pureText; }
        set
        {
            pureText = value;

            ParseReferPanels();
            // convert pure text to show text
            ConvertoShowText();
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        base.Start();
    }

    public void Update()
    {
        base.Update();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // convert show text to form editing (which without rich text <b><color>,...) 
        ConvertToEditText();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(Panel _panel, string _text)
    {
        base.Init(_panel, _text);
    }

    public override void OnEditDone()
    {
        if (!isEditing || !gameObject.active)
            return;

        // convert input text to pure text
        ParseInputToPureText();

        base.OnEditDone();

        //if (panel)
        //    panel.OnChildLabelEdited(this);
    }

    public void AddReferalPanel(Panel _referPanel)
    {
        PureText = pureText + "#" + _referPanel.Key + "#";
        RefreshContentSize();

        //if (panel)
        //    panel.OnChildLabelEdited(this);
        if (actEditDone != null)
            actEditDone(this);
    }

    // ========================================= PRIVATE FUNCS =========================================
    /// <summary>
    /// convert pure text to show text (which contain rich text <b></b> <color></color>)
    /// </summary>
    public void ConvertoShowText()
    {
        if (!inputField)
            return;

        string showOffText = pureText;

        // replace all of refer panel parts (#referpanel# -> <b><color>referpanel</color></b>)
        for (int i = 0; i < referPanels.Count; i++)
        {
            DataIndex referPanel = referPanels[i];
            string referStr = "";
            ColorBar.ColorType referColor = (ColorBar.ColorType)referPanel.colorId;
            // add prefix color tag
            if (referColor != ColorBar.ColorType.WHITE)
                referStr += TextUtil.OpenColorTag(referColor);
            // remove hash
            referStr += TextUtil.OpenBoldTag() + referPanel.title + TextUtil.CloseBoldTag();

            // close color tag
            if (referColor != ColorBar.ColorType.WHITE)
                referStr += TextUtil.CloseColorTag();

            // replace refer panel part of show off text
            showOffText = showOffText.Replace("#" + referPanel.genKey + "#", "#" + referStr + "#");
        }

        // show text
        inputField.text = showOffText;
    }

    /// <summary>
    /// convert pure text to editing text (which remove all rich text <b></b> <color></color>)
    /// </summary>
    protected void ConvertToEditText()
    {
        if (!inputField)
            return;

        string editText = pureText;

        for (int i = 0; i < referPanels.Count; i++)
        {
            DataIndex referPanel = referPanels[i];
            // replace refer panel part of show off text
            editText = editText.Replace("#" + referPanel.genKey + "#", "#" + referPanel.title + "#");
        }

        // show text
        inputField.text = editText;
    }

    /// <summary>
    /// parse from input text to pure text
    /// </summary>
    protected void ParseInputToPureText()
    {
        string parseText = inputField.text;

        string[] tmp = parseText.Split('#');
        for (int i = 0; i < tmp.Length; i++)
        {
            string splitVal = tmp[i];
            DataIndex findData = DataMgr.Instance.FindData(splitVal, true);
            if (findData != null)
            {
                // store refer data
                if (!referPanels.Contains(findData))
                    referPanels.Add(findData);
                parseText = parseText.Replace("#" + findData.title + "#", "#" + findData.genKey + "#");
            }
        }

        PureText = parseText;
    }

    /// <summary>
    /// this func to update list refer panels (call when pure text is modified)
    /// </summary>
    protected void ParseReferPanels()
    {
        // remove trigger modifying callback
        for (int i = 0; i < referPanels.Count; i++)
        {
            DataIndex referPanel = referPanels[i];
            referPanel.actModifyData -= ConvertoShowText;
        }

        referPanels.Clear();

        // parse pure text -> to retrieve referral panels
        string[] tmp = pureText.Split('#');
        for (int i = 0; i < tmp.Length; i++)
        {
            string parseKey = tmp[i];
            if (parseKey.Length == 0)
                continue;

            DataIndex findData = DataMgr.Instance.FindData(parseKey, false);
            // store refer data
            if (findData != null && !referPanels.Contains(findData))
                referPanels.Add(findData);
        }

        // add trigger function for all refer panels
        for (int i = 0; i < referPanels.Count; i++)
        {
            DataIndex referPanel = referPanels[i];
            referPanel.actModifyData += ConvertoShowText;
        }
    }
}
