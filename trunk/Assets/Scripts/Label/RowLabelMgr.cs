using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLabelMgr : MonoBehaviour
{
    public string prefLabelPath = "Prefabs/label";

    GameObject prefLabel;
    PanelMgr contParent;
    List<LabelMgr> lLabels = new List<LabelMgr>();

    // ========================================= GET/ SET FUNCS =========================================
    public void SetParent(PanelMgr labelCont)
    {
        contParent = labelCont;
    }

    public int ChildCount()
    {
        return lLabels.Count;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {

    }

    public void Init(PanelMgr labelCont)
    {
        contParent = labelCont;
        prefLabel = Resources.Load<GameObject>(prefLabelPath);

        // add template first row
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddLabel()
    {
        if (prefLabel)
        {
            // gen new label
            LabelMgr label = Instantiate(prefLabel, transform).GetComponent<LabelMgr>();
            label.Init(this);
            lLabels.Add(label);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void AddLabelAsFirst(LabelMgr label)
    {
        // set parent for label object  (transform)
        label.SetParent(this, true);
        //label.transform.parent = transform;
        //label.transform.SetAsFirstSibling();

        // add new label at first index (in storage)
        if (lLabels.Count > 0)
        {
            LabelMgr temp = lLabels[0];
            lLabels[0] = label;
            lLabels.Add(temp);
        }
        else
        {
            lLabels.Add(label);
        }
    }

    public LabelMgr RetrieveLastLabel()
    {
        if (lLabels.Count == 0)
            return null;

        int lastId = lLabels.Count - 1;

        LabelMgr lastLabel = lLabels[lastId];
        lastLabel.SetParent(null);  // remove from parent
        lLabels.RemoveAt(lastId);   // remove in storage

        return lastLabel;
    }

    public void OnChildLabelEditDone()
    {
        // call event to parent
        if (contParent)
            contParent.RefactorLabelRows();
    }
}
