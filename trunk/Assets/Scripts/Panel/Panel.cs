using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : DragingElement
{
    public Transform transLabelCont;

    protected RectTransform rt;
    protected Image image;

    // ========================================= UNITY FUNCS =========================================
    public void SetColor(Color color)
    {
        if (image)
            image.color = color;
    }

    public Color GetColor()
    {
        return image.color;
    }

    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();
    }

    public void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (image == null)
            image = GetComponent<Image>();

        // clear all child rows
        for (int i = 0; i < transLabelCont.childCount; i++)
        {
            Destroy(transLabelCont.GetChild(i).gameObject);
        }
    }
}
