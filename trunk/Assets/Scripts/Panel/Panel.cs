using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Panel : MonoBehaviour
{
    public Button addBtn;
    public Label titleLabel;
    public Transform transLabelCont;

    protected RectTransform rt;
    protected Image image;

    protected Board board;

    protected string key;
    protected string title;
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
                titleLabel.LabelText = title;
        }
    }

    public ColorBar.ColorType Color
    {
        get { return color; }
        set { color = value; }
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

        //// update title
        //if (title != titleLabel.GetText() && !titleLabel.IsEditing())
        //{
        //    string newTitle = titleLabel.GetText();
        //    // update key of panel in storage
        //    DataMgr.Instance.ReplaceTitle(dataType, title, newTitle);

        //    // update new key
        //    title = newTitle;
        //}
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
        for (int i = 0; i < transLabelCont.childCount; i++)
            Destroy(transLabelCont.GetChild(i).gameObject);

        // load prefab
        prefRow = Resources.Load<GameObject>(DataDefine.pref_path_rowLabel);
        prefLabel = Resources.Load<GameObject>(DataDefine.pref_path_label);

        // store parent
        this.board = _board;
        // set key
        key = _key;
        // set title
        Title = _title;
        // determine data type
        dataType = this is ElementPanel ? DataIndexer.DataType.Element : DataIndexer.DataType.Story;

        // calculate panel's zone
        RectOffset boardPadding = this.board.GetComponent<VerticalLayoutGroup>().padding;
        baseWidth = (this.transform as RectTransform).sizeDelta.x - (boardPadding.left + boardPadding.right);
    }

    public void AddLabel(string _var)
    {
        if (!prefLabel)
            return;

        Transform lastRow = GetLastRow();
        if (lastRow)
        {
            // add input panel
            Label genLabel = Instantiate(prefLabel, lastRow.transform).GetComponent<Label>();
            if (genLabel)
            {
                genLabel.Init(this, _var);

                // refresh position of add button
                RefreshAddButtonPos();

                // save
                DataMgr.Instance.AddElement(dataType, Key, genLabel.LabelText);
                // refresh canvas
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void RemoveLabel(Label _label)
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
            DataMgr.Instance.ReplaceElement(dataType, Key, labelIndex, _label.LabelText);
            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public virtual void OnArrangedLabels()
    {
        //foreach (RowLabelMgr row in rows)
        //    row.RefreshLabels();

        // save testing index after arrange
        //SaveTestingLabel();
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


    //public void AddTestingLabel(Label label)
    //{
    //    // find label is the child of the panel
    //    if (!testingLabels.Contains(label))
    //        testingLabels.Add(label);

    //    // save testing index
    //    SaveTestingLabel();
    //}

    //public void RemoveTestingLabel(Label label)
    //{
    //    if (testingLabels.Contains(label))
    //        testingLabels.Remove(label);

    //    // save testing index
    //    SaveTestingLabel();
    //}

    // ========================================= PRIVATE FUNCS =========================================
    //public void LoadTestingLabel(List<int> testingIndex)
    //{
    //    List<Label> labels = Labels();
    //    for (int i = 0; i < testingIndex.Count; i++)
    //    {
    //        int labelId = testingIndex[i];
    //        if (labelId < labels.Count)
    //        {
    //            Label label = labels[labelId];
    //            // set active highlight for label (also stored the highlight panel)
    //            label.SetActiveHighlightPanel(true);
    //        }
    //    }
    //}

    //private void SaveTestingLabel()
    //{
    //    List<int> testingIndex = new List<int>();
    //    List<Label> labels = Labels();
    //    for (int i = 0; i < testingLabels.Count; i++)
    //    {
    //        int findId = labels.FindIndex(x => x == testingLabels[i]);
    //        if (findId != -1)
    //            testingIndex.Add(findId);
    //    }

    //    // save testing index
    //    if (testingIndex.Count > 0)
    //        testingIndex.Sort();
    //    DataMgr.Instance.ReplaceTestingIndex(dataType, Title(), testingIndex);
    //}
}
