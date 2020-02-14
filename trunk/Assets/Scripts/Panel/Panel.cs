using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

[System.Serializable]
public class Panel : MonoBehaviour
{
    public Button addBtn;
    public TitleLabel titleLabel;
    public Transform transLabelCont;
    public GameObject testTag;

    public Action<Panel> actOnDestroy = null;

    protected RectTransform rt;
    protected Image image;

    protected string genKey;
    protected string title;
    protected bool isTesting;
    [SerializeField]
    protected Color rgbaColor;

    protected GameObject prefRow;
    protected GameObject prefLabel;

    protected List<Transform> rows = new List<Transform>();
    protected List<Label> labels = new List<Label>();

    protected DataIndexer.DataType dataType;

    // arrange panel
    protected HorizontalLayoutGroup layoutRow;
    protected float baseWidth = 0;
    [SerializeField]
    protected float contentSize = 0.9f;
    protected Vector2 refreshPanelDt = new Vector2(0, 0.5f);

    // ========================================= GET/ SET =========================================
    public string Genkey
    {
        get { return genKey; }
        set { genKey = value; }
    }

    public string Title
    {
        get { return title; }
        set
        {
            title = value;
            if (titleLabel)
                titleLabel.PureText = title;
        }
    }

    public bool IsTesting
    {
        get { return isTesting; }
        set
        {
            isTesting = value;
            if (testTag)
                testTag.gameObject.SetActive(isTesting);
        }
    }

    public Color RGBAColor
    {
        get { return rgbaColor; }
        set
        {
            rgbaColor = value;
            image.color = rgbaColor;
            //image.color = ColorMenu.Instance.GetColor(rgbaColor);
        }
    }

    public DataIndexer.DataType DataType
    {
        get { return dataType; }
    }

    public List<Label> Labels
    {
        get { return labels; }
    }

    // ========================================= UNITY FUNCS =========================================
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
    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init(string _key, string _title)
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
        layoutRow = prefRow.GetComponent<HorizontalLayoutGroup>();

        // set key
        Genkey = _key;
        // set title 
        if (titleLabel)
        {
            titleLabel.Init(this, "");  // init default
            titleLabel.actEditDone += OnTitleEdited;
        }
        Title = _title;
        // set default color
        rgbaColor = Color.white;

        // determine data type
        dataType = this is ElementPanel ? DataIndexer.DataType.Element : DataIndexer.DataType.Story;

        // calculate panel's zone
        baseWidth = rt.sizeDelta.x;
        if (layoutRow)
            baseWidth -= (layoutRow.padding.left + layoutRow.padding.right);
        baseWidth *= contentSize;

        // arrange panel
        RefreshPanelDt();
    }

    public virtual Label AddLabel(string _var)
    {
        if (!prefLabel)
            return null;

        Transform lastRow = GetLastRow();
        if (lastRow)
        {
            // add input panel
            Label genLabel = Instantiate(prefLabel, lastRow.transform).GetComponent<Label>();
            if (genLabel)
            {
                genLabel.Init(this, _var);
                // register the Label's action
                genLabel.actEditDone += OnChildLabelEdited;
                genLabel.actEditing += OnChildLabelEditing;

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
    }

    public void RemoveLabel(int _labelId)
    {
        if (_labelId >= 0 && _labelId < labels.Count)
        {
            // un-register action
            Label label = labels[_labelId];
            label.actEditDone -= OnChildLabelEdited;
            label.actEditing -= OnChildLabelEditing;

            // remove in list of row
            labels.RemoveAt(_labelId);

            // refresh position of add button
            RefreshAddButtonPos();

            // remove in save data
            DataMgr.Instance.RemoveElement(dataType, Genkey, _labelId);
            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
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
            // find index of label
            int labelIndex = labels.FindIndex(x => x.gameObject == _label.gameObject);
            if (labelIndex != -1)
            {
                // replace value of label in storage
                DataMgr.Instance.ReplaceElement(dataType, Genkey, labelIndex, _label.PureText);
                // refresh canvas
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void OnTitleEdited(Label _title)
    {
        // override new title
        if (_title.gameObject != titleLabel.gameObject)
            return;

        title = titleLabel.PureText;

        DataMgr.Instance.ReplaceTitle(dataType, genKey, Title);

        // refresh canvas
        CanvasMgr.Instance.RefreshCanvas();
    }

    public virtual void OnAddButtonPress()
    {
        Label genLabel = AddLabel("");

        if (genLabel)
        {
            // save
            DataMgr.Instance.AddElement(dataType, genKey, genLabel.PureText);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void SelfDestroy()
    {
        if (actOnDestroy != null)
            actOnDestroy.Invoke(this);

        Destroy(gameObject);
    }

    public void RefreshPanelDt()
    {
        refreshPanelDt.x = refreshPanelDt.y;
    }

    public void RefreshPanel()
    {
        List<Label> rowLabels = new List<Label>();
        float rowW = 0;
        int rowCounter = 0;

        for (int i = 0; i <= labels.Count; i++)
        {
            Label label = null;
            float labelW = 0;
            if (i < labels.Count)
            {
                label = labels[i];
                // get label's width
                labelW = (label.transform as RectTransform).sizeDelta.x;
                if (layoutRow)
                    labelW += layoutRow.spacing;
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

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public virtual void UpdateOrderLabels()
    {
        labels.Clear();

        // update order of labels
        labels = new List<Label>(transform.GetComponentsInChildren<ReactLabel>(true));

        List<string> eLabels = new List<string>();
        foreach (Label eLabel in labels)
            eLabels.Add(eLabel.PureText);

        // save 
        DataMgr.Instance.ReplaceElements(DataType, Genkey, eLabels);
    }

    // ========================================= PRIVATE FUNCS =========================================
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

    protected int FindLabelIndex(Label _label)
    {
        return labels.FindIndex(x => x.gameObject == _label.gameObject);
    }

    // === ADD BUTTON ===
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
}
