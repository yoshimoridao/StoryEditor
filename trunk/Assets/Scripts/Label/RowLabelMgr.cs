﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLabelMgr : MonoBehaviour
{
    GameObject prefInputLabel;
    GameObject prefLinkLabel;

    [SerializeField]
    CommonPanel contParent = null;
    List<Label> lLabels = new List<Label>();

    // ========================================= GET/ SET FUNCS =========================================
    public Panel GetParent()
    {
        if (contParent)
            return contParent;
        return null;
    }

    public int ChildCount()
    {
        return lLabels.Count;
    }

    public List<Label> GetLabels()
    {
        return lLabels;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {
        // remove label null (in case label was self destroy)
        for (int i = 0; i < lLabels.Count; i++)
        {
            Label label = lLabels[i];
            if (label == null)
            {
                lLabels.RemoveAt(i);
                i--;
            }
        }
    }

    public void Init(CommonPanel labelCont)
    {
        contParent = labelCont;

        // load prefabs
        prefInputLabel = Resources.Load<GameObject>(DataConfig.prefInputLabelPath);
        prefLinkLabel = Resources.Load<GameObject>(DataConfig.prefLinkLabelPath);

        // add template first row
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddInputLabel(string labelName = "")
    {
        if (prefInputLabel)
        {
            // gen new label
            InputLabel label = Instantiate(prefInputLabel, transform).GetComponent<InputLabel>();
            label.Init(GetParent(), labelName);
            lLabels.Add(label);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }
    public void AddLinkLabel(CommonPanel referPanel)
    {
        if (prefInputLabel)
        {
            // gen new label
            LinkLabel label = Instantiate(prefLinkLabel, transform).GetComponent<LinkLabel>();
            label.Init(GetParent(), referPanel);
            lLabels.Add(label);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }
    public void AddLinkLabel(string referPanelKey)
    {
        if (prefInputLabel)
        {
            // gen new label
            LinkLabel label = Instantiate(prefLinkLabel, transform).GetComponent<LinkLabel>();
            label.Init(GetParent(), referPanelKey);
            lLabels.Add(label);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void AddLabelAsFirst(Label label)
    {
        // set parent for label object  (transform)
        //label.SetParent(GetParent(), true);

        // add new label at first index (in storage)
        if (lLabels.Count > 0)
        {
            Label temp = lLabels[0];
            lLabels[0] = label;
            lLabels.Add(temp);
        }
        else
        {
            lLabels.Add(label);
        }
    }

    public Label RetrieveLastLabel()
    {
        if (lLabels.Count == 0)
            return null;

        int lastId = lLabels.Count - 1;

        Label lastLabel = lLabels[lastId];
        //lastLabel.SetParent(null);  // remove from parent
        lLabels.RemoveAt(lastId);   // remove in storage

        return lastLabel;
    }

    // ========== INPUT LABEL ==========
    //public void OnChildLabelEditDone()
    //{
    //    // call event to parent
    //    if (contParent)
    //        (contParent as CommonPanel).OnChildLabelEditDone();
    //}
}
