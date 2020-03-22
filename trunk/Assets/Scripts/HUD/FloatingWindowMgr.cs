using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.ModernUIPack;

public class FloatingWindowMgr : MonoBehaviour
{
    [SerializeField]
    private FloatingPanelCmd floatPanelCmd;

    private bool isOpenWindow = false;
    private GameObject window = null;
    private GameObject selectedObj = null;

    void Start()
    {
    }

    void Update()
    {
        // right mouse
        if (Input.GetMouseButtonDown(1))
        {
            var selectObj = CursorMgr.Instance.GetHandleObjOnTop();

            // active window
            if (selectObj != null && selectObj.GetComponent<Panel>())
            {
                // highlight obj
                HighlightSelectObj(selectObj);

                // active window
                isOpenWindow = true;
                window = floatPanelCmd.gameObject;

                IFloatingWindow iFloatWindow = window.GetComponent<IFloatingWindow>();
                if (iFloatWindow != null)
                {
                    iFloatWindow.SetActiveWindow(true, selectedObj);
                    iFloatWindow.ActOnWindowDisable += OnWindowDisable;

                    window.transform.position = Input.mousePosition;
                }
            }
            // de-active window
            else if (isOpenWindow)
            {
                IFloatingWindow iFloatWindow = window.GetComponent<IFloatingWindow>();
                if (iFloatWindow != null)
                {
                    iFloatWindow.SetActiveWindow(false, selectedObj);
                }
            }
        }
    }

    public void OnWindowDisable()
    {
        isOpenWindow = false;

        if (window == null)
            return;

        // un-highlight selected obj
        if (selectedObj != null && selectedObj.GetComponent<ISelectElement>() != null)
            selectedObj.GetComponent<ISelectElement>().OnEndSelect();

        // clear window object
        IFloatingWindow iFloatWindow = window.GetComponent<IFloatingWindow>();
        if (iFloatWindow != null)
        {
            iFloatWindow.ActOnWindowDisable -= OnWindowDisable;
            window = null;
        }
    }

    private void HighlightSelectObj(GameObject _selectObj)
    {
        if (_selectObj == null)
            return;

        // un-highlight old selected obj
        if (selectedObj != null)
            selectedObj.GetComponent<ISelectElement>().OnEndSelect();

        // store selected obj
        selectedObj = _selectObj;
        // highlight selected obj
        if (selectedObj.GetComponent<ISelectElement>() != null)
            selectedObj.GetComponent<ISelectElement>().OnSelect();
    }
}
