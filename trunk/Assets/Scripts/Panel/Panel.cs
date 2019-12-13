using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Panel : MonoBehaviour
{
    public Transform transLabelCont;

    protected RectTransform rt;
    protected Image image;
    protected ColorBar.ColorType colorType = ColorBar.ColorType.WHITE;
    protected string title;

    // ========================================= GET/ SET =========================================
    public string GetTitle()
    {
        return title;
    }

    public virtual void SetTitle(string val)
    {
        title = val;
    }

    // ========================================= UNITY FUNCS =========================================
    public virtual void SetColor(ColorBar.ColorType type)
    {
        if (image)
        {
            colorType = type;
            image.color = ColorBar.Instance.GetColor(colorType);
        }
    }

    public ColorBar.ColorType GetColorType()
    {
        return colorType;
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

    public virtual void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
