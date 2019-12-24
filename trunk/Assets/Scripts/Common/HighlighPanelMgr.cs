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
            string boardTag = (referPanel is StoryPanel) ? DataDefine.tag_board_story : DataDefine.tag_board_element;

            GameObject catchObj = null;
            if (CursorMgr.Instance.IsHoverObjs(out catchObj, DataDefine.tag_panel_common, boardTag))
            {
                Panel hoverPanel = catchObj.GetComponent<Panel>();
                if (hoverPanel)
                {
                    // diff panel || diff board
                    if (hoverPanel == referPanel ||
                        hoverPanel.transform.parent != referPanel.transform.parent)
                        return;

                    Transform parentTrans = referPanel.transform.parent;
                    // update sibling index of the highlight
                    float mouseY = Input.mousePosition.y;
                    float panelY = hoverPanel.transform.position.y;
                    int dropSiblingIndex = hoverPanel.transform.GetSiblingIndex();
                    int siblingIndex = transform.GetSiblingIndex();

                    // arrange
                    if ((dropSiblingIndex > siblingIndex && mouseY <= panelY) || (dropSiblingIndex <= siblingIndex && mouseY >= panelY))
                    {
                        referPanel.transform.SetSiblingIndex(hoverPanel.transform.GetSiblingIndex());
                        transform.SetSiblingIndex(referPanel.transform.GetSiblingIndex());
                    }
                }
            }
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Show(Panel panel)
    {
        // cache refer panel
        referPanel = panel;
        fstSiblingIndex = referPanel.transform.GetSiblingIndex();

        // get parent & size & child index of referral panel
        transform.parent = referPanel.transform.parent;
        transform.SetSiblingIndex(fstSiblingIndex);
        rt.sizeDelta = (referPanel.transform as RectTransform).sizeDelta;
        gameObject.SetActive(true);

        // hide refer panel
        referPanel.gameObject.SetActive(false);
    }

    public void Hide()
    {
        // update arrange objects
        if (referPanel)
        {
            if (CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
            {
                string boardTag = (referPanel is StoryPanel) ? DataDefine.tag_board_story : DataDefine.tag_board_element;

                // set sibling index of panel
                if (CursorMgr.Instance.IsHoverObjs(boardTag))
                    referPanel.transform.SetSiblingIndex(transform.GetSiblingIndex());
                // revert index in case user drag to another board
                else
                    referPanel.transform.SetSiblingIndex(fstSiblingIndex);

                // save index
                SaveIndex();
            }

            referPanel.gameObject.SetActive(true);
            referPanel = null;
        }

        fstSiblingIndex = -1;

        // hide highlight obj
        gameObject.SetActive(false);
        transform.parent = null;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void SaveIndex()
    {
        if (!referPanel)
            return;

        Transform parentTrans = referPanel.transform.parent;
        if (parentTrans)
        {
            List<Panel> panels = new List<Panel>();
            for (int i = 0; i < parentTrans.childCount; i++)
            {
                Panel childPanel = parentTrans.GetChild(i).GetComponent<Panel>();
                if (childPanel)
                    panels.Add(childPanel);
            }

            DataMgr.Instance.SortIndexes(referPanel.DataType, panels);
        }
    }
}
