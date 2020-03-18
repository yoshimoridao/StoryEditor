using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSpace : MonoBehaviour, IDragZone
{
    // ========================================= GET/ SET =========================================
    public Color originColor { get; set; }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        originColor = GetComponent<Image>().color;
    }

    void Update()
    {
    }

    #region interface
    public void OnMouseIn(GameObject obj)
    {
        GetComponent<Image>().color = DataDefine.highlight_drop_zone_color;
    }

    public void OnMouseOut()
    {
        GetComponent<Image>().color = originColor;
    }

    public void OnMouseDrop(GameObject obj)
    {

    }
    #endregion

    // ========================================= PUBLIC FUNCS =========================================

    // ========================================= PRIVATE FUNCS =========================================
}
