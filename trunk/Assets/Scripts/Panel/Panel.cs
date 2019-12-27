using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Panel : MonoBehaviour
{
    public Button addBtn;
    public TitleLabel titleLabel;
    public Transform transLabelCont;
    public GameObject testTag;

    protected RectTransform rt;
    protected Image image;

    protected Board board;

    protected string key;
    protected string title;
    protected bool isTesting;
    protected ColorBar.ColorType color = ColorBar.ColorType.WHITE;

    protected GameObject prefRow;
    protected GameObject prefLabel;

    protected float baseWidth = 0;
    protected List<Transform> rows = new List<Transform>();
    protected List<Label> labels = new List<Label>();

    protected DataIndexer.DataType dataType;

    // ========================================= GET/ SET =========================================
    public string Key
    {
        get { return key; }
        set { key = value; }
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

    public ColorBar.ColorType Color
    {
        get { return color; }
        set
        {
            color = value;
            image.color = ColorBar.Instance.GetColor(color);
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
        RefreshPanel();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init(Board _board, string _key, string _title)
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
                Destroy(transLabelCont.GetChild(i).gameObject);
        }

        // load prefab
        prefRow = Resources.Load<GameObject>(DataDefine.pref_path_rowLabel);

        // store parent
        this.board = _board;
        // set key
        Key = _key;
        // set title 
        if (titleLabel)
            titleLabel.Init(this, "");  // init default
        Title = _title;
        // determine data type
        dataType = this is ElementPanel ? DataIndexer.DataType.Element : DataIndexer.DataType.Story;

        // calculate panel's zone
        RectOffset boardPadding = this.board.GetComponent<VerticalLayoutGroup>().padding;
        baseWidth = (this.transform as RectTransform).sizeDelta.x - (boardPadding.left + boardPadding.right);
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
                // store label
                labels.Add(genLabel);

                // refresh position of add button
                RefreshAddButtonPos();

                return genLabel;
            }
        }

        return null;
    }

    public virtual void RemoveLabel(Label _label)
    {
        int findId = labels.FindIndex(x => x.gameObject == _label.gameObject);
        if (findId != -1)
        {
            // remove in list of row
            labels.RemoveAt(findId);

            // refresh position of add button
            RefreshAddButtonPos();

            // remove in save data
            DataMgr.Instance.RemoveElement(dataType, Key, findId);
            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void OnChildLabelEdited(Label _label)
    {
        // find index of label
        int labelIndex = labels.FindIndex(x => x.gameObject == _label.gameObject);
        if (labelIndex != -1)
        {
            // replace value of label in storage
            DataMgr.Instance.ReplaceElement(dataType, Key, labelIndex, _label.PureText);
            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void OnTitleEdited()
    {
        // override new title
        title = titleLabel.PureText;

        DataMgr.Instance.ReplaceTitle(dataType, key, Title);

        // refresh canvas
        CanvasMgr.Instance.RefreshCanvas();
    }

    public void OnAddButtonPress()
    {
        Label genLabel = AddLabel("");

        if (genLabel)
        {
            // save
            DataMgr.Instance.AddElement(dataType, key, genLabel.PureText);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void SelfDestroy()
    {
        board.RemovePanel(this);
        Destroy(gameObject);
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

    protected void RefreshPanel()
    {
        ////if (isChildLabelEditing)
        ////    return;

        //for (int i = 0; i < labelRows.Count; i++)
        //{
        //    RowLabelMgr row = labelRows[i];
        //    float rowW = (row.transform as RectTransform).sizeDelta.x;      // row's width

        //    // if the width of content shorter -> check to append first element of next row
        //    if (rowW < baseWidth * percentWContent && i + 1 < labelRows.Count)
        //    {
        //        RowLabelMgr nextRow = labelRows[i + 1];
        //        if (row.CheckAppendLabel(nextRow, baseWidth))
        //            break;
        //    }

        //    // if the width of content over size -> push last element to next row
        //    if (rowW > baseWidth && row.ChildCount() > 1)
        //    {
        //        // add label as first element of next row
        //        RowLabelMgr nextRow = null;
        //        if (i + 1 == labelRows.Count)
        //            nextRow = AddLabelRow();
        //        else
        //            nextRow = labelRows[i + 1];

        //        nextRow.AddFirstLabel(row);

        //        break;
        //    }
        //}

        //// refresh position of add button
        //RefreshAddButtonPos();
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
