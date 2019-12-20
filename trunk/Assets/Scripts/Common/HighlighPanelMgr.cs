using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlighPanelMgr : MonoBehaviour
{
    RectTransform rt;

    Panel referPanel;
    int fstSiblingIndex = -1;

    // ========================================= GET/ SET =========================================
    public bool IsActive()
    {
        return gameObject.active;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        // update arrange objects
        if (referPanel && CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            // just process arrange in same board
            string parentTag = (referPanel as CommonPanel).IsStoryElement() ? DataDefine.tag_board_story : DataDefine.tag_board_element;

            GameObject catchObj = null;
            if (CursorMgr.Instance.IsHoverObjs(out catchObj, DataDefine.tag_panel_common, parentTag))
            {
                CommonPanel hoverPanel = catchObj.GetComponent<CommonPanel>();
                if (hoverPanel)
                    ArrangePanel(hoverPanel);
            }
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Show(Panel panel)
    {
        gameObject.SetActive(true);

        // cache refer panel
        referPanel = panel;
        fstSiblingIndex = referPanel.transform.GetSiblingIndex();

        // get parent & size & child index of referral panel
        transform.parent = referPanel.transform.parent;
        transform.SetSiblingIndex(fstSiblingIndex);
        rt.sizeDelta = (referPanel.transform as RectTransform).sizeDelta;

        // hide refer panel
        referPanel.gameObject.SetActive(false);
    }

    public void Hide()
    {
        // update arrange objects
        if (referPanel && CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            // revert index in case user drag to another board
            string parentTag = (referPanel as CommonPanel).IsStoryElement() ? DataDefine.tag_board_story : DataDefine.tag_board_element;
            if (!CursorMgr.Instance.IsHoverObjs(parentTag))
            {
                referPanel.transform.SetSiblingIndex(fstSiblingIndex);
                // save index
                SaveIndex();
            }
        }

        fstSiblingIndex = -1;

        // hide highlight obj
        transform.parent = null;
        gameObject.SetActive(false);

        // re-show refer panel
        if (referPanel)
        {
            referPanel.gameObject.SetActive(true);
            referPanel = null;
        }
    }

    public void ArrangePanel(Panel dropPanel)
    {
        // diff parent -> return
        if (!referPanel ||
            dropPanel == referPanel ||
            dropPanel.transform.parent != referPanel.transform.parent)
            return;

        Transform parentTrans = referPanel.transform.parent;
        // update sibling index of the highlight
        float mouseY = Input.mousePosition.y;
        float panelY = dropPanel.transform.position.y;
        int dropSiblingIndex = dropPanel.transform.GetSiblingIndex();
        int siblingIndex = transform.GetSiblingIndex();

        if ((dropSiblingIndex > siblingIndex && mouseY <= panelY) || (dropSiblingIndex <= siblingIndex && mouseY >= panelY))
        {
            // arrange index
            referPanel.transform.SetSiblingIndex(dropPanel.transform.GetSiblingIndex());
            transform.SetSiblingIndex(referPanel.transform.GetSiblingIndex());

            // save index
            SaveIndex();
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void SaveIndex()
    {
        Transform parentTrans = referPanel.transform.parent;
        List<Panel> panels = new List<Panel>();
        for (int i = 0; i < parentTrans.childCount; i++)
        {
            Panel childPanel = parentTrans.GetChild(i).GetComponent<Panel>();
            if (childPanel)
                panels.Add(childPanel);
        }

        DataMgr.Instance.SortIndexes((referPanel as CommonPanel).GetDataType(), panels);
    }
}
