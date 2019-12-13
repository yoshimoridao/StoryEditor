using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScrollZone : MonoBehaviour
{
    public bool isTopZone;
    public Scrollbar scrollBar;
    public Transform content;
    public float sensitivity = 0.5f;

    Image img;
    const int sensitivityPerUnit = 100;
    private void Awake()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }

    public void Update()
    {
        if (CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE)
        {
            if (CursorMgr.Instance.IsDragingObj())
            {
                if (!img.enabled)
                    img.enabled = true;

                GameObject hoverObj = null;
                if (scrollBar && CursorMgr.Instance.IsHoverObjs(out hoverObj, gameObject.tag))
                {
                    Debug.Log("Before = " + scrollBar.value);
                    scrollBar.value += (Time.deltaTime * sensitivity * (sensitivityPerUnit / content.childCount)) * (isTopZone ? 1 : -1);
                    Mathf.Clamp(scrollBar.value, 0, 1.0f);
                    Debug.Log("After = " + scrollBar.value);
                }
            }
            else
            {
                img.enabled = false;
            }
        }
    }
}
