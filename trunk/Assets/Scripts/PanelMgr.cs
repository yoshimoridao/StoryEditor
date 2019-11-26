using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMgr : MonoBehaviour
{
    public string prefRow = "Prefabs/row_label";
    public Transform transLabelCont;
    public LabelMgr titleLabel;

    RootPanelMgr parentPanel;
    RectTransform rt;
    GameObject prefRowLabel;

    [SerializeField]
    List<RowLabelMgr> lLabelRows = new List<RowLabelMgr>();
    bool isRefactorRows = false;
    [SerializeField]
    float panelZoneW;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
    }

    void Update()
    {
        RefractorLabelRow();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init(RootPanelMgr rootPanel)
    {
        // load prefab
        prefRowLabel = Resources.Load<GameObject>(prefRow);
        // store parent
        parentPanel = rootPanel;

        // clear all child rows
        for (int i = 0; i < transLabelCont.childCount; i++)
        {
            Destroy(transLabelCont.GetChild(i).gameObject);
        }

        // calculate panel's zone
        RectOffset rootPadding = parentPanel.GetComponent<VerticalLayoutGroup>().padding;
        panelZoneW = (parentPanel.transform as RectTransform).sizeDelta.x - (rootPadding.left + rootPadding.right);

        // init title
        if (titleLabel)
            titleLabel.Init();
    }

    public void AddLabel()
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
                rowLabel.AddLabel();

                RefactorLabelRows();
                CanvasMgr.RefreshCanvas();
            }
        }
    }

    public void RefactorLabelRows()
    {
        isRefactorRows = true;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void RefractorLabelRow()
    {
        if (!CanvasMgr.IsRefreshCanvas())
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
                        LabelMgr lastLabel = row.RetrieveLastLabel();

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
}
