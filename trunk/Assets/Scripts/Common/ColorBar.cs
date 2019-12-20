using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBar : Singleton<ColorBar>
{
    [System.Serializable]
    public enum ColorType { WHITE, BLACK, RED, CYAN, GREEN, BLUE, ORANGE, PURPLE }
    List<Panel> referralPanels = new List<Panel>();
    RectTransform rt;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        rt = GetComponent<RectTransform>();
        SetActive(false);
    }
    
    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddReferralPanel(Panel panel)
    {
        // re-active color btn of prev panel
        referralPanels.Add(panel);
    }

    public void OnColorButtonPress(ColorBtn colorBtn)
    {
        foreach (Panel panel in referralPanels)
            (panel as CommonPanel).SetColor(colorBtn.type);

        //// de-active color bar
        //SetActiveGameObject(false);
    }

    public bool IsActive()
    {
        return gameObject.active;
    }

    public void SetActive(bool isActive)
    {
        if (!isActive)
        {
            referralPanels.Clear();
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
