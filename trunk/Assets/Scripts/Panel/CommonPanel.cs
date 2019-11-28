using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonPanel : Panel
{
    public Label titleLabel;
    public Button addBtn;

    Board board;
    GameObject prefRowLabel;

    List<RowLabelMgr> lLabelRows = new List<RowLabelMgr>();
    bool isRefactorRows = false;
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

    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
    }

    public void Update()
    {
        base.Update();

        RefractorLabelRow();

        // checking user change title -> to activate add btn
        if (!IsAddBtnActive() && titleLabel.GetText() != DataConfig.defaultPanelTitle)
        {
            ActiveAddBtn(true);
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(Board board)
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
        if (titleLabel)
            titleLabel.Init();

        // default disable add btn -> (to force user change title text)
        ActiveAddBtn(false);
    }

    public void AddInputLabel()
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
                rowLabel.AddInputLabel();

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

    public void OnChildLabelEditDone()
    {
        isRefactorRows = true;

        DataMgr.Instance.SaveData(this);
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

    private void RefractorLabelRow()
    {
        if (!CanvasMgr.Instance.IsRefreshCanvas())
        {
            if (isRefactorRows)
                isRefactorRows = false;
            return;
        }
        if (!isRefactorRows)
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
