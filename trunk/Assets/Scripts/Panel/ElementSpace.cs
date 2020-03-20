using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementSpace : MonoBehaviour, IDragZone
{
    // ========================================= GET/ SET =========================================
    public bool IsDragIn { get; set; }
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
        if (obj == gameObject)
            return;

        if (obj.GetComponent<ReactLabel>())
        {
            IsDragIn = true;
            GetComponent<Image>().color = DataDefine.highlight_drop_zone_color;
        }
    }

    public void OnMouseOut()
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;

        GetComponent<Image>().color = originColor;
    }

    public void OnMouseDrop(GameObject obj)
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;
        GetComponent<Image>().color = originColor;

        if (obj.GetComponent<ReactLabel>())
        {
            // change parent
            obj.transform.parent = transform.parent;
            // change sibling index
            obj.GetComponent<ReactLabel>().OnChangeSiblingIndex(transform.GetSiblingIndex());
        }
    }
    #endregion

    // ========================================= PUBLIC FUNCS =========================================

    // ========================================= PRIVATE FUNCS =========================================
}
