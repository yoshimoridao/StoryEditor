using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLabelMgr : MonoBehaviour
{
    GameObject prefInputLabel;
    GameObject prefLinkLabel;

    [SerializeField]
    CommonPanel contParent = null;
    [SerializeField]
    List<Label> labels = new List<Label>();

    // ========================================= GET/ SET FUNCS =========================================
    public Panel GetParent()
    {
        if (contParent)
            return contParent;
        return null;
    }

    public int ChildCount()
    {
        return labels.Count;
    }

    public List<Label> GetLabels()
    {
        return labels;
    }

    public Label GetLabel(int index)
    {
        if (index < labels.Count)
            return labels[index];
        return null;
    }

    public Label RetrieveLabel(int index)
    {
        if (index >= 0 && index < labels.Count)
        {
            Label label = labels[index];
            labels.RemoveAt(index);   // remove in storage
            return label;
        }

        return null;
    }

    public void AddLabel(Label label, int index = -1)
    {
        // change parent transform
        label.transform.parent = transform;

        // add new label at first index (in storage)
        if (index >= 0 && index < labels.Count)
        {
            labels.Insert(index, label);
            // set sibling index
            label.transform.SetSiblingIndex(index);
        }
        else
        {
            labels.Add(label);
        }

        // refresh canvas
        CanvasMgr.Instance.RefreshCanvas();
    }

    public void RefreshLabels()
    {
        labels.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Label label = transform.GetChild(i).GetComponent<Label>();
            if (label)
            {
                labels.Add(label);
            }
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {
        // remove label null (in case label was self destroy)
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            if (label == null)
            {
                labels.RemoveAt(i);
                i--;
            }
        }
    }

    public void Init(CommonPanel labelCont)
    {
        contParent = labelCont;

        // load prefabs
        prefInputLabel = Resources.Load<GameObject>(DataDefine.pref_path_inputLabel);
        prefLinkLabel = Resources.Load<GameObject>(DataDefine.pref_path_linkLabel);

        // add template first row
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    // ========================================= PUBLIC FUNCS =========================================
    public InputLabel AddInputLabel(string labelName = "")
    {
        if (prefInputLabel)
        {
            // gen new label
            InputLabel label = Instantiate(prefInputLabel, transform).GetComponent<InputLabel>();
            label.Init(GetParent(), labelName);
            AddLabel(label);

            return label;
        }

        return null;
    }
    public LinkLabel AddLinkLabel(CommonPanel referPanel)
    {
        if (prefInputLabel)
        {
            // gen new label
            LinkLabel label = Instantiate(prefLinkLabel, transform).GetComponent<LinkLabel>();
            label.Init(GetParent(), referPanel);
            AddLabel(label);

            return label;
        }

        return null;
    }
    public LinkLabel AddLinkLabel(string referPanelKey)
    {
        if (prefInputLabel)
        {
            // gen new label
            LinkLabel label = Instantiate(prefLinkLabel, transform).GetComponent<LinkLabel>();
            label.Init(GetParent(), referPanelKey);
            AddLabel(label);

            return label;
        }

        return null;
    }

    public bool CheckAppendLabel(RowLabelMgr nextRow, float baseWidth)
    {
        Label label = nextRow.GetLabel(0);
        if (label)
        {
            float labelW = (label.transform as RectTransform).sizeDelta.x;
            float rowW = (transform as RectTransform).sizeDelta.x;
            if (rowW + labelW <= baseWidth)
            {
                AddLabel(nextRow.RetrieveLabel(0));
                return true;
            }
        }

        return false;
    }

    public void AddFirstLabel(RowLabelMgr prevRow)
    {
        List<Label> labels = prevRow.GetLabels();
        if (labels.Count == 0)
            return;

        // get last label of previous row   
        Label lastLabel = prevRow.RetrieveLabel(labels.Count - 1);

        // add to first index
        AddLabel(lastLabel, 0);
    }

    // ========== INPUT LABEL ==========
    //public void OnChildLabelEditDone()
    //{
    //    // call event to parent
    //    if (contParent)
    //        (contParent as CommonPanel).OnChildLabelEditDone();
    //}
}
