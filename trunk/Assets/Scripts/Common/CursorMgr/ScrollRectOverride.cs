using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectOverride : ScrollRect
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.SCROLL)
            base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.SCROLL)
            base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.SCROLL)
            base.OnEndDrag(eventData);
    }
}
