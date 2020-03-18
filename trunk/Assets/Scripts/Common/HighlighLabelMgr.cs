using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlighLabelMgr : MonoBehaviour
{
    public float nailRange = 10.0f; // 20px
    private RectTransform rt;
    private Image img;

    private Label referLabel;
    private Transform rowTrans = null;
    private int fstSiblingIndex = -1;

    // ========================================= GET/ SET =========================================
    public bool IsActive()
    {
        return gameObject.active;
    }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    void Start()
    {
    }

    void Update()
    {
        // debug
        return;

        // arrange labels
        if (referLabel && CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            GameObject catchPanel = null;
            // in case: mouse drops on same panel
            if (IsDropOnSamePanel(out catchPanel))
            {
                GameObject element = CursorMgr.Instance.GetHandleObjOnTop();
                if (element && element.GetComponent<Label>())
                {
                    Label dropElement = element.GetComponent<Label>();
                    // if user drags to another label
                    if (referLabel && dropElement.gameObject != referLabel.gameObject && dropElement is ReactLabel)
                    {
                        // arrange label
                        ArrangeLabel(dropElement as ReactLabel);

                        // arrange panel
                        //if (referLabel.Panel)
                        //    referLabel.Panel.RefreshPanel();
                    }
                }
            }
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Show(Label label)
    {
        // cache refer panel, row parent's transform, label's index
        referLabel = label;
        rowTrans = referLabel.transform.parent;
        fstSiblingIndex = referLabel.transform.GetSiblingIndex();

        // set parent, size, child index of referral panel
        transform.parent = rowTrans;
        transform.SetSiblingIndex(fstSiblingIndex);
        rt.sizeDelta = (referLabel.transform as RectTransform).sizeDelta;

        //gameObject.SetActive(true);
        if (img)
            img.enabled = true;
        // hide refer panel
        referLabel.gameObject.SetActive(false);
    }

    public void Hide()
    {
        return;

        // Revert first index in case user drag to another board or panel
        if (referLabel && CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            GameObject catchPanel = null;
            if (!IsDropOnSamePanel(out catchPanel))
            {
                // in case: drop label out of parent panel's board
                referLabel.transform.parent = rowTrans;
                referLabel.transform.SetSiblingIndex(fstSiblingIndex);

                // update order of labels of the panel
                if (referLabel.Panel)
                    referLabel.Panel.UpdateOrderLabels();
            }
        }

        fstSiblingIndex = -1;
        rowTrans = null;
        // hide highlight obj
        //transform.parent = CanvasMgr.Instance.transform;
        transform.parent = GameMgr.Instance.CurEditor;

        //gameObject.SetActive(false);
        if (img)
            img.enabled = false;

        // re-show refer panel
        if (referLabel)
        {
            // arrange panel
            if (referLabel.Panel)
                referLabel.Panel.RefreshPanel();

            // refresh canvas after arrange panel
            GameMgr.Instance.RefreshCanvas();

            referLabel.gameObject.SetActive(true);
            referLabel = null;
        }
    }

    public void ArrangeLabel(ReactLabel _dropLabel)
    {
        float mouseX = Input.mousePosition.x;
        float labelX = transform.position.x;

        float dropX = _dropLabel.transform.position.x;

        //if (Mathf.Abs(dropX - mouseX) <= nailRange)
        if ((dropX < labelX && mouseX <= dropX)           // drag from right to left
            || (labelX <= dropX && dropX <= mouseX))      // drag from left to right
        {
            Transform transDropRow = _dropLabel.gameObject.transform.parent;
            int dropSiblingIndex = _dropLabel.transform.GetSiblingIndex();

            // set parent, sibling index for referral label
            referLabel.transform.parent = transDropRow;
            referLabel.transform.SetSiblingIndex(dropSiblingIndex);
            // set parent, sibling index for highlight
            transform.parent = referLabel.transform.parent;
            transform.SetSiblingIndex(referLabel.transform.GetSiblingIndex());
            
            // update order of labels in the panel
            if (referLabel.Panel)
                referLabel.Panel.UpdateOrderLabels();
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private bool IsDropOnSamePanel(out GameObject _catchObj)
    {
        _catchObj = null;

        if (referLabel.Panel)
        {
            // check hover obj in same board
            string boardTag = (referLabel.Panel is StoryPanel) ? DataDefine.tag_board_story : DataDefine.tag_board_element;
            // check hover obj on panel
            if (CursorMgr.Instance.IsHoverObjs(out _catchObj, DataDefine.tag_panel_common, boardTag))
            {
                Panel hoverPanel = _catchObj.GetComponent<Panel>();
                // in case: drop label in same panel
                if (hoverPanel && hoverPanel.gameObject == referLabel.Panel.gameObject)
                    return true;
            }
        }

        return false;
    }
}
