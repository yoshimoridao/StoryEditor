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

    Board board;
    GameObject prefRowLabel;

    List<RowLabelMgr> lLabelRows = new List<RowLabelMgr>();
    bool isRefreshLabelRow = false;
    float panelZoneW;

    // ========================================= GET/ SET =========================================
    public Board GetBoard()
    {
        return board;
    }

    public Label GetTitleLabel()
    {
        return titleLabel;
    }

    public List<Label> GetLabels()
    {
        List<Label> labels = new List<Label>();
        for (int i = 0; i < lLabelRows.Count; i++)
        {
            labels.AddRange(lLabelRows[i].GetLabels());
        }
        return labels;
    }

    public Button GetColorBtn()
    {
        if (colorBtn)
            return colorBtn;
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

        RefreshLabelRow();

        // in case changing title text
        if (title != titleLabel.GetText() && !titleLabel.IsModifyingText() && board)
        {
            // activate add button for the first time rename
            if (!IsAddBtnActive())
                ActiveAddBtn(true);
            string newTitle = titleLabel.GetText();
            // update key in storage
            DataMgr.DataType dataType = board.boardType == Board.BoardType.Story ? DataMgr.DataType.Story : DataMgr.DataType.Element;
            DataMgr.Instance.ReplaceDataInfoKey(dataType, title, newTitle);
            // update new key
            title = newTitle;
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(Board board, string key)
    {
        base.Init();

        // load prefab
        prefRowLabel = Resources.Load<GameObject>(DataConfig.prefRow);
        // store parent
        this.board = board;

        // calculate panel's zone
        RectOffset rootPadding = this.board.GetComponent<VerticalLayoutGroup>().padding;
        panelZoneW = (this.board.transform as RectTransform).sizeDelta.x - (rootPadding.left + rootPadding.right);

        // init title
        title = key;
        if (titleLabel)
        {
            titleLabel.Init(title);
        }

        // default disable add btn -> (to force user change title text)
        //ActiveAddBtn(false);

        // load index data (color, index,...)
        DataMgr.DataIndex dataIndex = DataMgr.Instance.GetDataIndex(title);
        if (dataIndex != null)
        {
            SetColor((ColorBar.ColorType)dataIndex.colorId);
        }
    }

    public void AddInputLabel(string labelTitle = "")
    {
        // add new row if empty
        if (lLabelRows.Count == 0)
            AddLabelRow();

        if (lLabelRows.Count > 0)
        {
            // append label to last row
            RowLabelMgr rowLabel = lLabelRows[lLabelRows.Count - 1];
            if (rowLabel)
            {
                // add input panel
                rowLabel.AddInputLabel(labelTitle);

                OnChildLabelEditDone();
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void AddLinkLabel(CommonPanel referPanel)
    {
        // add new row if empty
        if (lLabelRows.Count == 0)
            AddLabelRow();

        if (lLabelRows.Count > 0)
        {
            // append label to last row
            RowLabelMgr rowLabel = lLabelRows[lLabelRows.Count - 1];
            if (rowLabel)
            {
                rowLabel.AddLinkLabel(referPanel);     // add linking panel

                OnChildLabelEditDone();
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void AddLinkLabel(string referPanelKey)
    {
        // add new row if empty
        if (lLabelRows.Count == 0)
            AddLabelRow();

        if (lLabelRows.Count > 0)
        {
            // append label to last row
            RowLabelMgr rowLabel = lLabelRows[lLabelRows.Count - 1];
            if (rowLabel)
            {
                rowLabel.AddLinkLabel(referPanelKey);     // add linking panel

                OnChildLabelEditDone();
                CanvasMgr.Instance.RefreshCanvas();
            }
        }
    }

    public void OnChildLabelEditDone()
    {
        isRefreshLabelRow = true;

        // save in case having modified in child element
        DataMgr.Instance.SaveDataInfo(this);
    }

    public void SetActiveColorButton(bool isActive)
    {
        if (colorBtn)
        {
            colorBtn.interactable = isActive;
        }
    }

    public void OnColorButtonPressed()
    {
        // de-active color button
        SetActiveColorButton(false);

        // show color bar
        ColorBar.Instance.SetReferPanel(this);
    }

    public void OnDeleteButtonPressed()
    {
        if (board is ElementBoard)
            (board as ElementBoard).RemovePanel(this);
        else if (board is StoryBoard)
            (board as StoryBoard).RemovePanel(this);

        SelfDestroy();
    }

    // ========================================= OVERRIDE FUNCS =========================================
    public override Label GetTitleObj()
    {
        return titleLabel;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private RowLabelMgr AddLabelRow()
    {
        if (prefRowLabel)
        {
            RowLabelMgr rowLabel = Instantiate(prefRowLabel, transLabelCont).GetComponent<RowLabelMgr>();
            rowLabel.Init(this);

            // store label cont
            lLabelRows.Add(rowLabel);
            return rowLabel;
        }

        return null;
    }

    private void ActiveAddBtn(bool isActive)
    {
        addBtn.interactable = isActive;
    }

    private bool IsAddBtnActive()
    {
        return addBtn.interactable;
    }

    private void RefreshLabelRow()
    {
        if (!CanvasMgr.Instance.IsRefreshCanvas())
        {
            if (isRefreshLabelRow)
                isRefreshLabelRow = false;
            return;
        }
        if (!isRefreshLabelRow)
            return;

        for (int i = 0; i < lLabelRows.Count; i++)
        {
            RowLabelMgr row = lLabelRows[i];
            float rowW = (row.transform as RectTransform).sizeDelta.x;      // row's width
            float panelW = rt.sizeDelta.x;      // panel's width

            // if cont's width > panel's width
            if (rowW > panelW)
            {
                // extend cont's width
                if (rowW <= panelZoneW)
                {
                    rt.sizeDelta = new Vector2(rowW, rt.sizeDelta.y);
                }
                // retrieve last label of current row && add as first element to next row
                else
                {
                    if (panelW < panelZoneW)
                    {
                        rt.sizeDelta = new Vector2(panelZoneW, rt.sizeDelta.y);
                    }

                    if (row.ChildCount() > 1)
                    {
                        Label lastLabel = row.RetrieveLastLabel();

                        // add label as first element of next row
                        RowLabelMgr nextRow = null;
                        if (i + 1 == lLabelRows.Count)
                            nextRow = AddLabelRow();
                        else
                            nextRow = lLabelRows[i + 1];
                        nextRow.AddLabelAsFirst(lastLabel);
                    }
                }
            }
        }
    }
}
