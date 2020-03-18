using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SelectAbleElement : MonoBehaviour
{
    public Image highlightImg;

    bool isSelect = false;

    // ========================================= GET/ SET =========================================
    public bool IsSelect
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
        IsSelect = false;
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    private void ActiveHighlight(bool isActive)
    {
        //if (highlightImg)
        //    highlightImg.gameObject.SetActive(isActive);
    }
}
