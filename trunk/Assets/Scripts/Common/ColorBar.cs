using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBar : Singleton<ColorBar>
{
    [System.Serializable]
    public enum ColorType { WHITE, BLACK, RED, CYAN, GREEN, BLUE, ORANGE, PURPLE }
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

    public void OnTouchColorButton(ColorBtn colorBtn)
    {
        if (referralPanel)
        {
            (referralPanel as CommonPanel).SetActiveColorButton(true);
            referralPanel.SetColor(colorBtn.type);
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

    public Color GetColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.WHITE:
                return Color.white;
            case ColorType.BLACK:
                return Color.black;
            case ColorType.RED:
                return Color.red;
            case ColorType.CYAN:
                return Color.cyan;
            case ColorType.GREEN:
                return Color.green;
            case ColorType.BLUE:
                return Color.blue;
            case ColorType.ORANGE:
                return new Color(1, 0.5f, 0.0f, 1.0f);
            case ColorType.PURPLE:
                return new Color(1, 0.0f, 1, 1.0f);
            default:
                break;
        }
        return Color.white;
    }
}
