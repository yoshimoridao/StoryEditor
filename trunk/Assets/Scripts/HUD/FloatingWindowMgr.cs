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
    private List<GameObject> selectedObj = null;

    void Start()
    {
        CursorMgr.Instance.actOnMouseRightUp += OnMouseRightUp;
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        CursorMgr.Instance.actOnMouseRightUp -= OnMouseRightUp;
    }

    public void OnMouseRightUp()
    {
        var selectedObjs = CursorMgr.Instance.GetSelectedObjs();

        bool canActive = false;
        foreach (var element in selectedObjs)
        {
            // active window
            if (selectedObjs != null &&
                (element.GetComponent<Panel>() || element.GetComponent<ReactLabel>()))
            {
                canActive = true;
            }
            else
            {
                canActive = false;
            }
        }

        // active window
        if (canActive)
        {
            selectedObj = selectedObjs;
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

    public void OnWindowDisable()
    {
        isOpenWindow = false;

        if (window == null)
            return;

        // clear window object
        IFloatingWindow iFloatWindow = window.GetComponent<IFloatingWindow>();
        if (iFloatWindow != null)
        {
            iFloatWindow.ActOnWindowDisable -= OnWindowDisable;
            window = null;
        }
    }
}
