using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : DragingElement
{
    public Transform transLabelCont;

    protected RectTransform rt;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        rt = GetComponent<RectTransform>();

        // clear all child rows
        for (int i = 0; i < transLabelCont.childCount; i++)
        {
            Destroy(transLabelCont.GetChild(i).gameObject);
        }
    }
}
