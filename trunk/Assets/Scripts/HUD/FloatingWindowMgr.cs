using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.ModernUIPack;

public class FloatingWindowMgr : MonoBehaviour
{
    [SerializeField]
    private FloatingMenuConfig config;
    [SerializeField]
    private FloatingPanelCmd floatPanelCmd;
    [SerializeField]
    private DropdownPanelCmd dropdownPanelCmd;

    private bool isOpenWindow = false;
    private IFloatingWindow floatingWindow;

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
        List<GameObject> selectedObjs = CursorMgr.Instance.GetSelectedObjs();

        int floatingType = -1;
        foreach (var element in selectedObjs)
        {
            bool ignore = false;
            // active window
            if (selectedObjs != null)
            {
                int eType = -1;
                if (element.GetComponent<Panel>())
                    eType = (int)FloatingMenuType.PANEL;
                else if (element.GetComponent<ReactLabel>())
                    eType = (int)FloatingMenuType.LABEL;

                if (eType != -1)
                {
                    if (floatingType == -1)
                        floatingType = eType;
                    else if (floatingType != -1 && floatingType != eType)
                        ignore = true;
                }
                else
                {
                    ignore = true;
                }
            }
            else
            {
                ignore = true;
            }

            if (ignore)
            {
                floatingType = -1;
                break;
            }
        }

        // active window
        if (floatingType != -1)
        {
            // active window
            var floatingItems = config.GetFloatingItems((FloatingMenuType)floatingType);
            floatPanelCmd.ActiveWindow(floatingItems, selectedObjs);
            // set position
            floatPanelCmd.transform.position = Input.mousePosition;

            // on active 
            OnActiveWindow(floatPanelCmd.gameObject);
        }
        // de-active window
        else if (isOpenWindow && floatingWindow != null)
        {
            floatingWindow.DeactiveWindow();
        }
    }

    public void OnFileButtonPress(RectTransform _btn)
    {
        // active window
        var dropdownItems = config.GetDowndownItems(DropdownMenuType.FILE);
        dropdownPanelCmd.ActiveWindow(dropdownItems);
        // set position
        dropdownPanelCmd.transform.position = _btn.position;

        // on active 
        OnActiveWindow(dropdownPanelCmd.gameObject);
    }

    public void OnWindowDisable()
    {
        isOpenWindow = false;

        // clear window object
        if (floatingWindow != null)
            floatingWindow.ActOnWindowDisable -= OnWindowDisable;
    }

    private void OnActiveWindow(GameObject _obj)
    {
        // register event
        floatingWindow = _obj.GetComponent<IFloatingWindow>();
        if (floatingWindow != null)
        {
            isOpenWindow = true;
            floatingWindow.ActOnWindowDisable += OnWindowDisable;
        }
    }
}
