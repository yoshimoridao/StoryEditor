using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBarMgr : MonoBehaviour
{
    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void OnPressDragModeBtn()
    {
        CursorMgr cursorMgr = CursorMgr.Instance;
        switch (cursorMgr.DragMode)
        {
            case CursorMgr.DragBehavior.CONNECT:
                cursorMgr.DragMode = CursorMgr.DragBehavior.ARRANGE;
                break;
            case CursorMgr.DragBehavior.ARRANGE:
                cursorMgr.DragMode = CursorMgr.DragBehavior.SCROLL;
                break;
            case CursorMgr.DragBehavior.SCROLL:
                cursorMgr.DragMode = CursorMgr.DragBehavior.CONNECT;
                break;
            default:
                break;
        }

        Debug.Log(cursorMgr.DragMode);
    }

    public void OnPressSelectModeBtn()
    {
        CursorMgr cursorMgr = CursorMgr.Instance;
        if (cursorMgr.SelectMode == CursorMgr.SelectBehavior.MULTIPLE)
            cursorMgr.SelectMode = CursorMgr.SelectBehavior.SINGLE;
        else
            cursorMgr.SelectMode = CursorMgr.SelectBehavior.MULTIPLE;

        Debug.Log(cursorMgr.SelectMode);
    }

    public void OnDestroyBtnPress()
    {
        // active destroy mode
        CanvasMgr.Instance.DestroyElements();
    }
}
