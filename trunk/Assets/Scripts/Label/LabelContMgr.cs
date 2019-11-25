using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelContMgr : MonoBehaviour
{
    public string prefRowLabelPath = "Prefabs/row_label";

    RectTransform rt;
    GameObject prefRowLabel;
    [SerializeField]
    List<RowLabelMgr> lLabelRows = new List<RowLabelMgr>();

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        prefRowLabel = Resources.Load<GameObject>(prefRowLabelPath);
        // add default first row
        for (int i = 0; i < transform.childCount; i++)
        {
            RowLabelMgr row = transform.GetChild(i).GetComponent<RowLabelMgr>();
            if (row)
                lLabelRows.Add(row);
        }
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddLabel()
    {
        if (lLabelRows.Count == 1 && !lLabelRows[0].gameObject.active)
        {
            lLabelRows[0].gameObject.SetActive(true);
        }
        else if (lLabelRows.Count > 0)
        {
            // append label to last row
            lLabelRows[lLabelRows.Count - 1].AddLabel();
        }

        CanvasMgr.RefreshCanvas();
    }

    public void OnChildLabelEditDone()
    {
        for (int i = 0; i < lLabelRows.Count; i++)
        {
            RowLabelMgr row = lLabelRows[i];
            if (row.enabled)
            {
                if ((row.transform as RectTransform).sizeDelta.x > rt.sizeDelta.x)
                {
                    AddRowLabel();
                }
            }
        }
    }

    private void AddRowLabel()
    {
        if (prefRowLabel)
        {
            RowLabelMgr rowLabel = Instantiate(prefRowLabel, transform).GetComponent<RowLabelMgr>();
            lLabelRows.Add(rowLabel);
        }
    }
}
