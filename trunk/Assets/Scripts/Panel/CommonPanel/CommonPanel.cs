using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CommonPanel : Panel
{
    public InputLabel titleLabel;
    public Button addBtn;
    public Button colorBtn;
    public float percentWContent = 0.9f;    // percent ratio vs base width (=> the width to contain elements)

    Board board;
    GameObject prefRowLabel;

    List<RowLabelMgr> labelRows = new List<RowLabelMgr>();
    float baseWidth = 0;
    bool isChildLabelEditing = false;
    DataIndexer.DataType dataType;

    // ========================================= GET/ SET =========================================
    public DataIndexer.DataType GetDataType()
    {
        return dataType;
    }

    public bool IsStoryElement()
    {
        return dataType == DataIndexer.DataType.Story;
    }

    public List<Label> GetLabels()
    {
        List<Label> labels = new List<Label>();
        for (int i = 0; i < labelRows.Count; i++)
        {
            labels.AddRange(labelRows[i].GetLabels());
        }
        return labels;
    }

    public Button GetColorBtn()
    {
        if (colorBtn)
            return colorBtn;
        return null;
    }

    public Label GetTitleObj()
    {
        if (titleLabel)
            return titleLabel;

        return null;
    }

    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
        base.Start();
    }

    public void Update()
    {
        base.Update();

        RefreshPanel();

        // update title
        if (title != titleLabel.GetText() && !titleLabel.IsEditing())
        {
            string newTitle = titleLabel.GetText();
            // update key of panel in storage
            DataMgr.Instance.ReplaceIndexKey(dataType, title, newTitle);

            // update new key
            title = newTitle;
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(Board board, string key)
    {
        base.Init();

        // load prefab
        prefRowLabel = Resources.Load<GameObject>(DataDefine.pref_path_rowLabel);
        // store parent
        this.board = board;
        dataType = (board is StoryBoard) ? DataIndexer.DataType.Story : DataIndexer.DataType.Element;

        // calculate panel's zone
        RectOffset boardPadding = this.board.GetComponent<VerticalLayoutGroup>().padding;
        baseWidth = (this.transform as RectTransform).sizeDelta.x - (boardPadding.left + boardPadding.right);

        // init title
        title = key;
        if (titleLabel)
        {
            titleLabel.Init(title);
        }

        // load index data (color, index,...)
        DataIndex dataIndex = DataMgr.Instance.GetIndex(dataType, key);
        if (dataIndex != null)
        {
            SetColor((ColorBar.ColorType)dataIndex.colorId);
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public void AddInputLabel(string labelTitle = "")
    {
        RowLabelMgr rowLabel = GetLastLabelRow();
        if (rowLabel)
        {
            // add input panel
            Label genLabel = rowLabel.AddInputLabel(labelTitle);
            if (genLabel)
            {
                DataMgr.Instance.AddElement(dataType, GetTitle(), genLabel);
                CanvasMgr.Instance.RefreshCanvas();
            }
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public void AddLinkLabel(CommonPanel referPanel)
    {
        RowLabelMgr rowLabel = GetLastLabelRow();
        if (rowLabel)
        {
            Label genLabel  = rowLabel.AddLinkLabel(referPanel);
            if (genLabel)
            {
                DataMgr.Instance.AddElement(dataType, GetTitle(), genLabel);
                CanvasMgr.Instance.RefreshCanvas();
            }
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public void AddLinkLabel(string referPanelKey)
    {
        // add new row if empty
        RowLabelMgr rowLabel = GetLastLabelRow();
        if (rowLabel)
        {
            // append label to last row
            Label genLabel = rowLabel.AddLinkLabel(referPanelKey);   // add linking panel
            if (genLabel)
            {
                DataMgr.Instance.AddElement(dataType, GetTitle(), genLabel);
                CanvasMgr.Instance.RefreshCanvas();
            }
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public void RemoveLabel(Label label)
    {
        int idFstLabel = 0;
        for (int i = 0; i < labelRows.Count; i++)
        {
            List<Label> labels = labelRows[i].GetLabels();
            int findId = labels.FindIndex(x => x.gameObject == label.gameObject);
            if (findId != -1)
            {
                // remove in list of row
                labels.RemoveAt(findId);

                // remove in save data
                DataMgr.Instance.RemoveElement(dataType, GetTitle(), idFstLabel + findId);
            }

            idFstLabel = labels.Count;
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public void OnChildLabelEditDone(Label childLabel)
    {
        isChildLabelEditing = false;

        // find index of label
        int labelIndex = FindIndexLabel(childLabel);
        if (labelIndex != -1)
        {
            // replace value of label in storage
            DataMgr.Instance.ReplaceElement(dataType, GetTitle(), labelIndex, childLabel);

            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void OnChildLabelEditing()
    {
        isChildLabelEditing = true;
    }

    public void SetActiveColorButton(bool isActive)
    {
        if (colorBtn)
        {
            colorBtn.interactable = isActive;
        }
    }

    public void RefreshLabels()
    {
        foreach (RowLabelMgr row in labelRows)
            row.RefreshLabels();
    }

    // === button ===
    public void OnColorButtonPressed()
    {
        // de-active color button
        SetActiveColorButton(false);

        // show color bar
        ColorBar.Instance.SetReferPanel(this);
    }
    // ========================================= OVERRIDE FUNCS =========================================
    public override void SetColor(ColorBar.ColorType type)
    {
        base.SetColor(type);

        // save color in storage
        DataMgr.Instance.SetColorIndex(dataType, GetTitle(), type);
    }

    public override void SelfDestroy()
    {
        if (IsStoryElement())
            (board as StoryBoard).RemovePanel(this);
        else
            (board as ElementBoard).RemovePanel(this);

        base.SelfDestroy();
    }

    // ========================================= PRIVATE FUNCS =========================================
    private RowLabelMgr AddLabelRow()
    {
        if (prefRowLabel)
        {
            RowLabelMgr rowLabel = Instantiate(prefRowLabel, transLabelCont).GetComponent<RowLabelMgr>();
            rowLabel.Init(this);

            // store label cont
            labelRows.Add(rowLabel);
            return rowLabel;
        }

        return null;
    }

    private void RefreshPanel()
    {
        //if (isChildLabelEditing)
        //    return;

        for (int i = 0; i < labelRows.Count; i++)
        {
            RowLabelMgr row = labelRows[i];
            float rowW = (row.transform as RectTransform).sizeDelta.x;      // row's width

            // if the width of content shorter -> check to append first element of next row
            if (rowW < baseWidth * percentWContent && i + 1 < labelRows.Count)
            {
                RowLabelMgr nextRow = labelRows[i + 1];
                if (row.CheckAppendLabel(nextRow, baseWidth))
                    break;
            }

            // if the width of content over size -> push last element to next row
            if (rowW > baseWidth && row.ChildCount() > 1)
            {
                // add label as first element of next row
                RowLabelMgr nextRow = null;
                if (i + 1 == labelRows.Count)
                    nextRow = AddLabelRow();
                else
                    nextRow = labelRows[i + 1];

                nextRow.AddFirstLabel(row);

                break;
            }
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    private int FindIndexLabel(Label label)
    {
        List<Label> labels = GetLabels();
        return labels.FindIndex(x => x.GetText() == label.GetText());
    }

    private RowLabelMgr GetLastLabelRow()
    {
        // add new row if empty
        if (labelRows.Count == 0)
            AddLabelRow();

        if (labelRows.Count > 0)
        {
            // append label to last row
            RowLabelMgr rowLabel = labelRows[labelRows.Count - 1];
            return rowLabel;
        }

        return null;
    }

    // === ADD BUTTON ===
    private void RefreshAddButtonPos()
    {
        // get last row
        RowLabelMgr row = GetLastLabelRow();
        // set button is always the last child of last row
        if (row && addBtn)
        {
            addBtn.transform.parent = row.transform;
            addBtn.transform.SetSiblingIndex(row.transform.childCount);
        }
    }
}
