using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SelectAbleElement : MonoBehaviour
{
    public Image highlightNode;

    bool isSelect = false;

    // ========================================= GET/ SET =========================================
    public bool Select
    {
        get { return isSelect; }
        set
        {
            isSelect = value;
            ActiveHighlight(isSelect);
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        Select = false;
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    private void ActiveHighlight(bool isActive)
    {
        if (highlightNode)
            highlightNode.gameObject.SetActive(isActive);
    }
}
