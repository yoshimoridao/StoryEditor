using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorMgr : MonoBehaviour
{
    public InputField dragingTitle;

    RectTransform rt;
    DragingElement dragingElement = null;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        rt = transform as RectTransform;

        // hide title default
        ActiveTitle(false);
    }

    void Start()
    {

    }

    void Update()
    {
        // mouse down
        if (Input.GetMouseButtonDown(0) && dragingElement == null)
        {
            GameObject touchedObj = GetTouchedUIElement();
            // if touched obj is draging element
            if (touchedObj && touchedObj.GetComponent<DragingElement>())
            {
                dragingElement = touchedObj.GetComponent<DragingElement>();
                // active draging title
                if (dragingElement)
                    ActiveTitle(true);

                // catch mouse position
                rt.position = Input.mousePosition;
            }
        }

        // in case draging an element
        if (dragingElement)
        {
            // mouse hold
            if (Input.GetMouseButton(0))
            {
                // catch mouse position
                rt.position = Input.mousePosition;
            }
            // mouse up
            else if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp();
            }
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public GameObject GetTouchedUIElement(int catchLayerId = 0)
    {
        // get ray cast all objs
        var ray = GetRayCastResultByMousePos();

        //for (int index = 0; index < raysastResults.Count; index++)
        if (ray.Count > 0)
        {
            //RaycastResult curRaysastResult = raysastResults[index];
            RaycastResult rayResult = ray[catchLayerId];
            if (rayResult.gameObject)
            {
                GameObject castObj = rayResult.gameObject;
                return castObj;
            }
        }

        return null;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void OnMouseUp()
    {
        ActiveTitle(false);

        // catch event
        // event [1]: from panel to panel
        GameObject touchedObj = GetTouchedUIElement(0);
        if (touchedObj)
        {
            GameObject cacheObj = null;
            if (IsTouchUIElement(out cacheObj, "rootpanel_attribute", "rootpanel_sentence"))
            {
                cacheObj = null;
                if (IsTouchUIElement(out cacheObj, "panel_common"))
                {
                    OnDragPanelToPanel(cacheObj.GetComponent<CommonPanel>());
                }
            }
            else if (IsTouchUIElement(out cacheObj, "rootpanel_result"))
            {
                if (cacheObj.GetComponent<ResultPanel>() && dragingElement is CommonPanel)
                    cacheObj.GetComponent<ResultPanel>().ShowResult(dragingElement as CommonPanel);
            }
        }

        // hide draging title & clear draging obj
        dragingElement = null;
    }

    private bool IsTouchUIElement(out GameObject obj, params string[] tags)
    {
        obj = null;

        // get ray cast all objs
        var ray = GetRayCastResultByMousePos();

        for (int i = 0; i < ray.Count; i++)
        {
            string touchedTag = ray[i].gameObject.tag;
            foreach(string tag in tags)
            {
                if (touchedTag == tag)
                {
                    obj = ray[i].gameObject;
                    return true;
                }
            }
        }
        return false;
    }

    private List<RaycastResult> GetRayCastResultByMousePos()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // cast all obj 
        List<RaycastResult> ray = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, ray);

        return ray;
    }

    private void ActiveTitle(bool isActive)
    {
        dragingTitle.gameObject.SetActive(isActive);
        if (isActive && dragingElement)
        {
            Label label = dragingElement.GetTitleObj();

            // clone text & font size
            dragingTitle.text = label.GetTextObj().text;
            dragingTitle.GetComponentInChildren<Text>().fontSize = label.GetTextObj().fontSize;

            // clone size delta
            (dragingTitle.transform as RectTransform).sizeDelta = (label.transform as RectTransform).sizeDelta;
        }
    }

    // === drag event ===
    private void OnDragPanelToPanel(Panel dropPanel)
    {
        if (dragingElement is CommonPanel && dropPanel && dropPanel != dragingElement)
            (dropPanel as CommonPanel).AddLinkLabel(dragingElement as CommonPanel);
    }
}
