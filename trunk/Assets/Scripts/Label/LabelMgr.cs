using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelMgr : MonoBehaviour
{
    public ContentSizeFitter contentSize;
    public Vector2 offset = new Vector2(5, 5);    // pixel

    // prop
    RectTransform rt;
    RowLabelMgr rowParent;
    bool isChangeVal = false;

    // ========================================= GET/ SET FUNCS =========================================
    public void SetParent(RowLabelMgr rowLabel, bool isAsFirstElement = false)
    {
        // set transform parent
        if (rowLabel)
        {
            transform.parent = rowLabel.transform;
            if (isAsFirstElement)
                transform.SetAsFirstSibling();

            // store parent
            rowParent = rowLabel;
        }
        else
        {
            transform.parent = null;
            rowParent = null;
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
    }

    public void Init(RowLabelMgr rowLabel)
    {
        rowParent = rowLabel;

        rt = GetComponent<RectTransform>();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void OnEditDone()
    {
        if (!isChangeVal)
            return;

        isChangeVal = false;
        contentSize.enabled = false;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x + offset.x, rt.sizeDelta.y + offset.y);

        CanvasMgr.RefreshCanvas();

        // call event to parent
        if (rowParent)
            rowParent.OnChildLabelEditDone();
    }

    public void OnChangeValue()
    {
        if (!contentSize)
            return;

        isChangeVal = true;
        contentSize.enabled = false;
        contentSize.enabled = true;

        CanvasMgr.RefreshCanvas();
    }
}
