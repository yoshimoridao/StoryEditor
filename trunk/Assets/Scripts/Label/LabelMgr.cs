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
    bool isChangeVal = false;

    // ========================================= UNITY FUNCS =========================================
    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        
    }

    void Update()
    {
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
