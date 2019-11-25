using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLabelMgr : MonoBehaviour
{
    public string prefLabelPath = "Prefabs/label";

    GameObject prefLabel;
    List<LabelMgr> lLabels = new List<LabelMgr>();

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        prefLabel = Resources.Load<GameObject>(prefLabelPath);
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddLabel()
    {
        if (prefLabel)
        {
            LabelMgr label = Instantiate(prefLabel, transform).GetComponent<LabelMgr>();
            lLabels.Add(label);

            CanvasMgr.RefreshCanvas();
        }
    }
}
