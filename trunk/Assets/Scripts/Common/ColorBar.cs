using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBar : Singleton<ColorBar>
{
    Panel referralPanel = null;
    RectTransform rt;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        rt = GetComponent<RectTransform>();
        SetActiveGameObject(false);
    }
    
    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void SetReferPanel(Panel panel)
    {
        // re-active color btn of prev panel
        if (referralPanel != panel && referralPanel is CommonPanel)
        {
            (referralPanel as CommonPanel).SetActiveColorButton(true);
        }

        // show color bar at position of panel
        if (panel is CommonPanel)
        {
            RectTransform rtColorBtn = (panel as CommonPanel).GetColorBtn().transform as RectTransform;
            rt.position = rtColorBtn.position;
        }

        referralPanel = panel;

        // active color bar
        SetActiveGameObject(true);
    }

    public void OnTouchColorButton(Image imgColor)
    {
        if (referralPanel)
        {
            (referralPanel as CommonPanel).SetActiveColorButton(true);
            referralPanel.SetColor(imgColor.color);
        }

        // de-active color bar
        SetActiveGameObject(false);
    }

    public bool IsActive()
    {
        return gameObject.active;
    }

    public void SetActiveGameObject(bool isActive)
    {
        if (!isActive)
        {
            // re-active color btn of prev panel
            if (referralPanel && referralPanel is CommonPanel)
            {
                (referralPanel as CommonPanel).SetActiveColorButton(true);
                referralPanel = null;
            }
        }

        gameObject.SetActive(isActive);
    }
}
