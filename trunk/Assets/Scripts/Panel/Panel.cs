using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

[System.Serializable]
public class Panel : MonoBehaviour, ISelectElement, IDragElement, IDragZone
{
    public Button addBtn;
    public TitleLabel titleLabel;
    public Transform transLabelCont;
    public GameObject testTag;

    // actions
    public Action actOnChangeSiblingId = null;
    public Action<Panel> actOnDestroy = null;

    protected RectTransform rt;
    protected Image image;

    protected DataIndex dataIndex;

    protected GameObject prefRow;
    protected GameObject prefLabel;
    protected GameObject prefElementSpace;

    protected List<Transform> rows = new List<Transform>();
    protected List<Label> labels = new List<Label>();
    protected DataIndexer.DataType dataType;

    // arrange panel
    protected HorizontalLayoutGroup layoutRow;
    protected float baseWidth = 0;
    [SerializeField]
    protected float contentSize = 0.9f;
    protected Vector2 refreshPanelDt = new Vector2(0, 0.5f);

    public bool IsDragIn { get; set; }
    public Color originColor { get; set; }

    // ========================================= GET/ SET =========================================
    #region getter/setter
    public string Genkey { get { return dataIndex.genKey; } }
    public bool IsTesting
    {
        get { return dataIndex.isTest; }
        set
        {
            // set data 
            if (dataIndex == null)
                return;
            dataIndex.isTest = value;

            // active test tag
            if (testTag)
                testTag.gameObject.SetActive(dataIndex.isTest);

            // refresh testing text of result window
            ResultWindow.Instance.RefreshPickupAmountText();
        }
    }
    public Color RGBAColor
    {
        get { return dataIndex.RGBAColor; }
        set
        {
            // set data 
            if (dataIndex == null)
                return;
            dataIndex.RGBAColor = value;

            image.color = RGBAColor;
        }
    }
    public DataIndexer.DataType DataType { get { return dataType; } }
    public List<Label> Labels { get { return labels; } }
    
    public DataIndex GetDataIndex()
    {
        if (dataIndex != null)
            return dataIndex;

        return null;
    }

    public void SetDataIndex(DataIndex _dataIndex)
    {
        dataIndex = _dataIndex;

        SetDataTitle(dataIndex.title);            // load title
        RGBAColor = dataIndex.RGBAColor;    // load color
        IsTesting = dataIndex.isTest;       // load testing flag
    }

    private void SetDataTitle(string _val)
    {
        // set data 
        if (dataIndex == null)
            return;
        dataIndex.title = _val;

        if (titleLabel)
            titleLabel.PureText = dataIndex.title;
    }
    #endregion

    // ========================================= UNITY FUNCS =========================================
    #region common
    public void Start()
    {
        // get component
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();
    }

    public void Update()
    {
        // update dt for refresh panel
        if (refreshPanelDt.x > 0)
        {
            refreshPanelDt.x -= Time.deltaTime;
            if (refreshPanelDt.x <= 0)
                refreshPanelDt.x = 0;
            else
                RefreshPanel();
        }
    }

    public void OnDestroy()
    {
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            if (label)
            {
                RemoveLabel(i);
                i--;
            }
        }
    }

    public virtual void Init(DataIndex _dataIndex)
    {
        // get component
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        // clear all child rows (template)
        if (transLabelCont)
        {
            for (int i = 0; i < transLabelCont.childCount; i++)
            {
                GameObject childObj = transLabelCont.GetChild(i).gameObject;
                if (childObj == addBtn.gameObject)
                    continue;
                else
                    Destroy(childObj);
            }
        }

        // load prefab
        prefRow = Resources.Load<GameObject>(DataDefine.pref_path_rowLabel);
        prefElementSpace = Resources.Load<GameObject>(DataDefine.pref_path_element_space);
        layoutRow = prefRow.GetComponent<HorizontalLayoutGroup>();

        // store data
        dataIndex = _dataIndex;

        // init title
        if (titleLabel)
        {
            titleLabel.Init(this, "");  // init default
            titleLabel.actOnEditDone += OnTitleEdited;
        }

        // load title, color
        SetDataTitle(dataIndex.title);
        RGBAColor = dataIndex.RGBAColor;
        IsTesting = dataIndex.isTest;

        // determine data type
        dataType = this is ElementPanel ? DataIndexer.DataType.Element : DataIndexer.DataType.Story;

        // calculate panel's zone
        baseWidth = rt.sizeDelta.x;
        if (layoutRow)
            baseWidth -= (layoutRow.padding.left + layoutRow.padding.right);
        baseWidth *= contentSize;

        // refresh position of add button
        RefreshAddButtonPos();

        // arrange panel
        RefreshPanelDt();
    }

    public void SelfDestroy()
    {
        if (actOnDestroy != null)
            actOnDestroy.Invoke(this);

        Destroy(gameObject);
    }
    #endregion

    #region inheritance
    // === IDropZone ===
    public void OnMouseIn(GameObject obj)
    {
        if (obj == gameObject)
            return;

        if (obj.GetComponent<Panel>())
        {
            IsDragIn = true;

            // store origin color before change highlight color
            originColor = image.color;
            image.color = DataDefine.highlight_drop_zone_color;
        }
    }

    public void OnMouseOut()
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;

        image.color = originColor;
    }

    public void OnMouseDrop(GameObject obj)
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;
        image.color = originColor;

        // drag panel to panel
        if (obj.GetComponent<Panel>())
        {
            // draging from a panel to panel
            Panel dragPanel = obj.GetComponent<Panel>();
            // for link function
            string labelVal = "#" + dragPanel.Genkey + "#";
            AddLabel(labelVal);

            // refresh canvas
            GameMgr.Instance.RefreshCanvas();
        }
    }

    // === IDragElement ===
    public void OnDragging()
    {
        // store origin color before change highlight color
        originColor = image.color;
        image.color = DataDefine.highlight_drag_obj_color;
    }

    public void OnEndDrag()
    {
        image.color = originColor;
    }

    // === ISelectElement ===
    public void OnSelect()
    {
        if (!titleLabel.Field.isFocused)
        {
            // store origin color before change highlight color
            originColor = image.color;
            image.color = DataDefine.highlight_drag_obj_color;
        }
    }

    public void OnEndSelect()
    {
        image.color = originColor;
    }
    #endregion
    // ========================================= PUBLIC FUNCS =========================================
    #region label
    public Label AddLabel(string _val, bool _isGenData = true)
    {
        if (!prefLabel)
            return null;

        // gen data
        DataElementIndex genData = null;
        if (_isGenData)
        {
            genData = dataIndex.AddElement(_val);
        }

        Transform lastRow = GetLastRow();
        if (lastRow)
        {
            // add input panel
            ReactLabel genLabel = Instantiate(prefLabel, lastRow.transform).GetComponent<ReactLabel>();
            if (genLabel)
            {
                genLabel.Init(this, _val);

                // set data for element
                if (_isGenData && genData != null)
                {
                    genLabel.SetDataElementIndex(genData);
                }

                // register the Label's action
                genLabel.actOnEditDone += OnChildLabelEdited;
                genLabel.actOnEditing += OnChildLabelEditing;
                genLabel.actOnChangeSiblingId += OnChildChangeSiblingIndex;
                // store label
                labels.Add(genLabel);

                // refresh position of add button
                RefreshAddButtonPos();

                return genLabel;
            }
        }

        return null;
    }

    public void RemoveLabel(Label _label)
    {
        int findId = labels.FindIndex(x => x.gameObject == _label.gameObject);
        if (findId != -1)
            RemoveLabel(findId);

        // refresh panel
        RefreshPanelDt();
    }

    public void RemoveLabel(int _labelId)
    {
        if (_labelId >= 0 && _labelId < labels.Count)
        {
            // un-register action
            ReactLabel label = labels[_labelId] as ReactLabel;
            label.actOnEditDone -= OnChildLabelEdited;
            label.actOnEditing -= OnChildLabelEditing;
            label.actOnChangeSiblingId -= OnChildChangeSiblingIndex;
            // remove in list of row
            labels.RemoveAt(_labelId);

            // refresh position of add button
            RefreshAddButtonPos();

            // remove in save data
            if (label.GetDataElementIndex() != null && dataIndex != null)
                dataIndex.RemoveElement(_labelId);

            // refresh canvas
            GameMgr.Instance.RefreshCanvas();
        }
    }
    #endregion

    #region panel
    public void RefreshPanelDt()
    {
        refreshPanelDt.x = refreshPanelDt.y;
    }

    public void RefreshPanel()
    {
        var labels = Labels;
        List<Label> rowLabels = new List<Label>();
        float rowW = 0;
        int rowCounter = 0;
        float spaceW = (prefElementSpace.transform as RectTransform).sizeDelta.x;

        for (int i = 0; i <= labels.Count; i++)
        {
            Label label = null;
            float labelW = 0;
            if (i < labels.Count)
            {
                label = labels[i];
                // get label's width
                labelW = (label.transform as RectTransform).sizeDelta.x + spaceW;
                // calculate row's width
                rowW += labelW;
            }

            // if the width of content over size
            if (rowW > baseWidth || (i == labels.Count))
            {
                Transform addingRow = null;
                // get next row
                if (rowCounter < rows.Count)
                    addingRow = rows[rowCounter];
                else
                    addingRow = AddNewRow();

                // set child for next row
                for (int j = 0; j < rowLabels.Count; j++)
                {
                    Label eLabel = rowLabels[j];
                    eLabel.transform.parent = addingRow;
                    eLabel.transform.SetSiblingIndex(j);
                }

                rowLabels.Clear();
                rowW = labelW;
                rowCounter++;
            }

            rowLabels.Add(label);
        }

        // refresh space elements
        RefreshSpaceCharInRows();

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public virtual void UpdateOrderLabels()
    {
        if (dataIndex == null)
            return;

        labels.Clear();
        // update order of labels
        labels = new List<Label>(transform.GetComponentsInChildren<ReactLabel>(true));

        List<DataElementIndex> dataElementIds = new List<DataElementIndex>();
        foreach (var reactLabel in labels)
        {
            if ((reactLabel as ReactLabel).GetDataElementIndex() != null)
                dataElementIds.Add((reactLabel as ReactLabel).GetDataElementIndex());
        }

        dataIndex.elements = dataElementIds;
    }
    #endregion

    // ================= EVENT =================
    #region event
    public void OnChangeSiblingIndex(int _siblingId)
    {
        transform.SetSiblingIndex(_siblingId);
        // call action func
        if (actOnChangeSiblingId != null)
            actOnChangeSiblingId();
    }

    public void OnChildChangeSiblingIndex()
    {
        UpdateOrderLabels();
        RefreshSpaceCharInRows();
        RefreshPanelDt();
    }

    public void OnChildLabelEditing()
    {
        RefreshPanelDt();
    }

    public virtual void OnChildLabelEdited(Label _label)
    {
        RefreshPanelDt();

        // remove label null
        if (_label.PureText.Length == 0)
        {
            RemoveLabel(_label);
        }
        else
        {
            // refresh canvas
            GameMgr.Instance.RefreshCanvas();
        }
    }

    public void OnTitleEdited(Label _title)
    {
        // override new title
        if (_title.gameObject != titleLabel.gameObject)
            return;

        dataIndex.Title = titleLabel.PureText;

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }

    public virtual void OnAddButtonPress()
    {
        if (dataIndex == null)
            return;

        // gen label obj
        Label genLabel = AddLabel(DataDefine.defaultLabelVar);

        if (genLabel)
        {
            // refresh canvas
            GameMgr.Instance.RefreshCanvas();
        }
    }
    #endregion

    // ================= ROW =================
    #region row
    protected Transform AddNewRow()
    {
        if (prefRow)
        {
            Transform row = Instantiate(prefRow, transLabelCont).transform;

            // store label cont
            rows.Add(row);
            return row;
        }

        return null;
    }

    protected Transform GetLastRow()
    {
        if (rows.Count > 0)
            return rows[rows.Count - 1];
        // add new row if empty
        else
            AddNewRow();

        return null;
    }
    protected void RefreshSpaceCharInRows()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            Transform row = rows[i];
            if (row.childCount == 0)
                continue;

            var eSpaces = GenSpaceElementForRow(row);
            if (eSpaces == null)
                continue;

            for (int j = 0; j < eSpaces.Count; j++)
            {
                eSpaces[j].transform.SetSiblingIndex(2 * j);
            }
        }
    }
    protected List<ElementSpace> GenSpaceElementForRow(Transform _row)
    {
        List<ElementSpace> spaces = new List<ElementSpace>(_row.GetComponentsInChildren<ElementSpace>());
        var childLabels = _row.GetComponentsInChildren<ReactLabel>();
        if (childLabels.Length == 0)
            return null;

        var genAmount = childLabels.Length + 1;
        // gen more objs
        if (spaces.Count < genAmount)
        {
            genAmount -= spaces.Count;
            for (int i = 0; i < genAmount; i++)
            {
                var genSpace = Instantiate(prefElementSpace, _row);
                spaces.Add(genSpace.GetComponent<ElementSpace>());
            }
        }
        // destroy surplus objs
        else if (spaces.Count > genAmount)
        {
            for (int i = genAmount; i < spaces.Count; i++)
                Destroy(spaces[i].gameObject);

            spaces.RemoveRange(genAmount, spaces.Count - genAmount);
        }
        return spaces;
    }
    #endregion

    // ================= UTIL =================
    #region util
    protected void RefreshAddButtonPos()
    {
        // get last row
        Transform row = GetLastRow();
        // set button is always the last child of last row
        if (row && addBtn)
        {
            addBtn.transform.parent = row.transform;
            addBtn.transform.SetSiblingIndex(row.transform.childCount);
        }
    }
    #endregion
}
