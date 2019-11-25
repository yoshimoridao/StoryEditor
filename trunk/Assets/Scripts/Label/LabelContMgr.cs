using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelContMgr : MonoBehaviour
{
    public string prefRowLabelPath = "Prefabs/row_label";

    GameObject prefRowLabel;
    List<RowLabelMgr> labelRows = new List<RowLabelMgr>();

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        prefRowLabel = Resources.Load<GameObject>(prefRowLabelPath);
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddLabelRow()
    {
        if (prefRowLabel)
        {
            RowLabelMgr rowLabel = Instantiate(prefRowLabel, transform).GetComponent<RowLabelMgr>();
            labelRows.Add(rowLabel);
        }
    }
}
