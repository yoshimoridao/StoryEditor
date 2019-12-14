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

    public void OnTestTagPress()
    {
        isActive = !isActive;

        if (panel)
        {
            string panelTitle = panel.GetTitle();
            // add || remove test case
            if (isActive)
                DataMgr.Instance.AddTestCase(panelTitle);
            else
                DataMgr.Instance.RemoveTestCase(panelTitle);

            // set color of tag
            if (img)
                img.color = isActive ? enableColor : disableColor;

            // change mode test cases for result board
            ResultBoard resultBoard = CanvasMgr.Instance.GetBoard<ResultBoard>() as ResultBoard;
            if (resultBoard)
                resultBoard.IsRandomTestCases = DataMgr.Instance.GetTestCases().Count > 0 ? false : true;
        }
    }
}
