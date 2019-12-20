using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTag : MonoBehaviour
{
    public Color enableColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
    public Color disableColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
    public CommonPanel panel;
    Image img;
    bool isActive = false;

    // ========================================= GET/ SET =========================================
    public bool IsActive() { return isActive; }

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    // ========================================= PUBLIC =========================================
    public void OnTestTagPress()
    {
        SetActiveTag(!isActive);
    }

    public void SetActiveTag(bool _isActive)
    {
        isActive = _isActive;
        gameObject.SetActive(isActive);

        if (panel)
        {
            // add || remove test case
            string panelTitle = panel.GetTitle();
            if (isActive)
            {
                // add test case in data
                DataMgr.Instance.AddPickedTestCase(panelTitle);
            }
            else
            {
                // remove test case in data
                DataMgr.Instance.RemovePickedTestCase(panelTitle);
            }

            // set color of tag
            if (img)
                img.color = isActive ? enableColor : disableColor;
        }

        // refresh text of amount of picking up panel
        ResultBoard resultBoard = CanvasMgr.Instance.GetBoard<ResultBoard>() as ResultBoard;
        resultBoard.RefreshPickupAmountText();
    }
}
