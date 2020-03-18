using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReactLabel : Label, IPointerClickHandler
{
    protected DataElementIndex dataIndex = new DataElementIndex();

    protected List<DataIndex> referPanels = new List<DataIndex>();
    [SerializeField]
    protected Text text;

    // ========================================= PROPERTIES =========================================
    public override string PureText
    {
        get { return pureText; }
        set
        {
            SetPureText(value);

            // store all of refer panels
            ParseReferPanels();
        }
    }

    public DataElementIndex GetDataElementIndex()
    {
        return dataIndex;
    }

    public virtual void SetDataElementIndex(DataElementIndex _data)
    {
        dataIndex = _data;

        PureText = dataIndex.value;
    }

    private void SetPureText(string _val)
    {
        pureText = _val;
        if (dataIndex != null)
            dataIndex.value = pureText;
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

    private void OnDestroy()
    {
        // clear all refer panels
        ClearAllReferPanels();

        // un-register font size button's action
        ToolbarMgr.Instance.fontSizeButton.actOnModifyVal -= OnChangeFontSize;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // convert show text to form editing (which without rich text <b><color>,...) 
        //ConvertToEditText();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(Panel _panel, string _text)
    {
        base.Init(_panel, _text);

        // set default font size
        ChangeFontSize(DataMgr.Instance.NormalFontSize);
        // register font size button's action
        ToolbarMgr.Instance.fontSizeButton.actOnModifyVal += OnChangeFontSize;
    }

    public override void OnEditDone()
    {
        if (!isEditing || !gameObject.active)
            return;

        // remove content empty
        if (inputField.text.Length == 0)
        {
            SelfDestroy();
            return;
        }

        // convert input text to pure text
        string parseText = inputField.text;

        string[] tmp = parseText.Split('#');
        for (int i = 0; i < tmp.Length; i++)
        {
            string splitVal = tmp[i];
            DataIndexer.DataType findDataType;
            DataIndex findData = DataMgr.Instance.FindData(splitVal, true, out findDataType);
            if (findData != null)
            {
                // add refer panel
                if (!IsContainReferPanel(findData.genKey))
                    AddReferPanel(findData);

                parseText = parseText.Replace("#" + findData.title + "#", "#" + findData.genKey + "#");
            }
        }

        // override pure text
        //pureText = parseText;
        SetPureText(parseText);

        // convert to show text
        ConvertToShowText();

        base.OnEditDone();
    }

    public void OnDragPanelInto(Panel _panel)
    {
        string contentText = pureText + "#" + _panel.Genkey + "#";
        SetPureText(contentText);

        // add refer panel
        if (!IsContainReferPanel(_panel.Genkey))
        {
            DataIndexer.DataType findDataType;
            DataIndex findData = DataMgr.Instance.FindData(_panel.Genkey, false, out findDataType);
            if (findData != null)
            {
                AddReferPanel(findData);
            }
        }

        // convert to show text
        ConvertToShowText();

        RefreshContentSize();

        if (actEditDone != null)
            actEditDone(this);
    }

    public void OnReferPanelDestroy(string _dataKey)
    {
        int findId = referPanels.FindIndex(x => x.genKey == _dataKey);

        if (findId != -1)
        {
            // remove the refer panel
            RemoveReferPanel(findId);
            // replace error text
            string contentText = pureText.Replace("#" + _dataKey + "#", DataDefine.error_refer_text);
            SetPureText(contentText);

            // convert to show text
            ConvertToShowText();
        }

        RefreshContentSize();

        // call action func
        if (actEditDone != null)
            actEditDone(this);
    }

    public void AddReferPanel(DataIndex _dataIndex)
    {
        if (_dataIndex != null)
        {
            referPanels.Add(_dataIndex);
            // register modifying action
            _dataIndex.actModifyData += ConvertToShowText;
            _dataIndex.actOnDestroy += OnReferPanelDestroy;
        }
    }

    public void RemoveReferPanel(int _id)
    {
        // remove the refer panel
        if (_id >= 0 && _id < referPanels.Count)
        {
            DataIndex dataIndex = referPanels[_id];

            // un-register modifying action
            if (dataIndex != null)
            {
                dataIndex.actModifyData -= ConvertToShowText;
                dataIndex.actOnDestroy -= OnReferPanelDestroy;
            }

            referPanels.RemoveAt(_id);
        }
    }

    /// <summary>
    /// convert pure text to editing text (which remove all rich text <b></b> <color></color>)
    /// </summary>
    public void ConvertToEditText()
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
    /// convert pure text to show text (which contain rich text <b></b> <color></color>)
    /// </summary>
    public virtual void ConvertToShowText()
    {
        if (!inputField)
            return;

        string showOffText = pureText;

        // replace all of refer panel parts (#referpanel# -> <b><color>referpanel</color></b>)
        for (int i = 0; i < referPanels.Count; i++)
        {
            DataIndex referPanel = referPanels[i];
            string referStr = "";
            Color referColor = referPanel.RGBAColor;
            // add prefix color tag
            if (referColor != Color.white)
                referStr += TextUtil.OpenColorTag(referColor);
            // remove hash
            referStr += TextUtil.OpenBoldTag() + referPanel.title + TextUtil.CloseBoldTag();

            // close color tag
            if (referColor != Color.white)
                referStr += TextUtil.CloseColorTag();

            // replace refer panel part of show off text
            showOffText = showOffText.Replace("#" + referPanel.genKey + "#", "#" + referStr + "#");
        }

        // highlight link error text
        if (showOffText.Contains(DataDefine.error_refer_text))
        {
            string errorText = TextUtil.OpenColorTag(Color.red) + DataDefine.error_refer_text + TextUtil.CloseColorTag();
            showOffText = showOffText.Replace(DataDefine.error_refer_text, errorText);
        }

        // show text
        inputField.text = showOffText;
    }

    public bool IsContainReferPanel(string _panelKey)
    {
        int findId = referPanels.FindIndex(x => x.genKey == _panelKey);

        return findId != -1;
    }

    public void OnChangeFontSize(int _val)
    {
        ChangeFontSize(_val);
    }

    public override void RefreshContentSize()
    {
        base.RefreshContentSize();
    }

    // ========================================= PRIVATE FUNCS =========================================
    /// <summary>
    /// this func to update list refer panels (call when pure text is modified)
    /// </summary>
    protected void ParseReferPanels()
    {
        // clear all refer panels
        ClearAllReferPanels();

        // parse pure text -> to retrieve referral panels
        string[] tmp = pureText.Split('#');
        for (int i = 0; i < tmp.Length; i++)
        {
            string parseKey = tmp[i];
            if (parseKey.Length == 0)
                continue;

            DataIndexer.DataType findDataType;
            DataIndex findData = DataMgr.Instance.FindData(parseKey, false, out findDataType);
            // add refer data
            if (findData != null && !IsContainReferPanel(findData.genKey))
                AddReferPanel(findData);
        }

        // convert pure text to show text
        ConvertToShowText();
    }

    protected void ClearAllReferPanels()
    {
        // un-register action
        for (int i = 0; i < referPanels.Count; i++)
        {
            if (referPanels[i] != null)
            {
                RemoveReferPanel(i);
                i--;
            }
        }

        referPanels.Clear();
    }

    protected void ChangeFontSize(int _val)
    {
        if (text)
        {
            text.fontSize = _val;
            RefreshContentSize();

            // refresh panel
            if (panel)
                panel.RefreshPanelDt();
        }
    }
}
