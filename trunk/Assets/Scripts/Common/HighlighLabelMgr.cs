using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlighLabelMgr : MonoBehaviour
{
    public float nailRange = 10.0f; // 20px
    RectTransform rt;

    Label referLabel;
    Transform rowTrans = null;
    int fstSiblingIndex = -1;

    // ========================================= GET/ SET =========================================
    public bool IsActive()
    {
        return gameObject.active;
    }

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
        // arrange labels
        if (referLabel && CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            GameObject catchObj = null;
            // in case: mouse drops on same panel
            if (IsDropOnSamePanel(out catchObj))
            {
                GameObject element = CursorMgr.Instance.CatchElementHandleByMouse();
                if (element && element.GetComponent<Label>())
                {
                    Label dropElement = element.GetComponent<Label>();
                    if (dropElement.gameObject != referLabel.gameObject)
                        ArrangeLabel(dropElement);
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

        gameObject.SetActive(true);
        // hide refer panel
        referLabel.gameObject.SetActive(false);
    }

    public void Hide()
    {
        // Revert first index in case user drag to another board or panel
        if (referLabel && CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            GameObject catchObj = null;
            if (!IsDropOnSamePanel(out catchObj))
            {
                // in case: drop label out of parent panel's board
                referLabel.transform.parent = rowTrans;
                referLabel.transform.SetSiblingIndex(fstSiblingIndex);

                // save index
                SaveIndex();
            }
        }

        fstSiblingIndex = -1;
        rowTrans = null;
        // hide highlight obj
        transform.parent = null;
        gameObject.SetActive(false);

        // re-show refer panel
        if (referLabel)
        {
            referLabel.gameObject.SetActive(true);
            referLabel = null;
        }
    }

    public void ArrangeLabel(Label dropLabel)
    {
        // if match object -> return
        if (!referLabel ||
            dropLabel.gameObject == referLabel.gameObject)
            return;

        float mouseX = Input.mousePosition.x;
        float labelX = referLabel.transform.position.x;
        float dropX = dropLabel.transform.position.x;

        //if ((dropX > labelX && mouseX >= dropX) || (dropX <= labelX && mouseX <= dropX))
        if (Mathf.Abs(dropX - mouseX) <= nailRange)
        {
            Transform transDropRow = dropLabel.gameObject.transform.parent;
            int dropSiblingIndex = dropLabel.transform.GetSiblingIndex();

            // set parent, sibling index for referral label
            referLabel.transform.parent = transDropRow;
            referLabel.transform.SetSiblingIndex(dropSiblingIndex);
            // set parent, sibling index for highlight
            transform.transform.parent = referLabel.transform.parent;
            transform.SetSiblingIndex(referLabel.transform.GetSiblingIndex());

            // save index
            SaveIndex();
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private bool IsDropOnSamePanel(out GameObject catchObj)
    {
        catchObj = null;

        Panel parentPanel = referLabel.GetParent();
        if (parentPanel is CommonPanel)
        {
            // check hover obj in same board
            string parentTag = (parentPanel as CommonPanel).IsStoryElement() ? DataDefine.tag_board_story : DataDefine.tag_board_element;
            // check hover obj on panel
            if (CursorMgr.Instance.IsHoverObjs(out catchObj, parentTag) && CursorMgr.Instance.IsHoverObjs(out catchObj, DataDefine.tag_panel_common))
            {
                CommonPanel hoverPanel = catchObj.GetComponent<CommonPanel>();
                // in case: drop label in same panel
                if (hoverPanel && hoverPanel.gameObject == parentPanel.gameObject)
                    return true;
            }
        }

        return false;
    }

    private void SaveIndex()
    {
        Panel panel = referLabel.GetParent();

        if (panel is CommonPanel)
        {
            (panel as CommonPanel).RefreshLabels();
            DataMgr.Instance.ReplaceElements(panel as CommonPanel);
        }
    }
}
