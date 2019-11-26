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
        if (Input.GetMouseButtonDown(0))
        {
            // get draging element
            if (dragingElement == null)
            {
                dragingElement = GetTouchedUIElement();

                // active draging title
                if (dragingElement)
                    ActiveTitle(true);
            }

            // catch mouse position
            Vector2 mousePos = Input.mousePosition;
            rt.position = mousePos;
        }
        else if (Input.GetMouseButton(0) && dragingElement)
        {
            // catch mouse position
            Vector2 mousePos = Input.mousePosition;
            rt.position = mousePos;
        }
        else if (dragingElement)
        {
            dragingElement = null;

            // hide draging title
            ActiveTitle(false);
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public DragingElement GetTouchedUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // cast all obj 
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);

        //for (int index = 0; index < raysastResults.Count; index++)
        {
            //RaycastResult curRaysastResult = raysastResults[index];
            RaycastResult curRaysastResult = raysastResults[0];
            if (curRaysastResult.gameObject)
            {
                GameObject castObj = curRaysastResult.gameObject;
                if (castObj.GetComponent<DragingElement>())
                    return castObj.GetComponent<DragingElement>();
            }
        }

        return null;
    }

    private void ActiveTitle(bool isActive)
    {
        dragingTitle.gameObject.SetActive(isActive);
        if (isActive && dragingElement)
        {
            LabelMgr label = dragingElement.GetTitleObj();

            // clone text & font size
            dragingTitle.text = label.GetText().text;
            dragingTitle.GetComponentInChildren<Text>().fontSize = label.GetText().fontSize;

            // clone size delta
            (dragingTitle.transform as RectTransform).sizeDelta = (label.transform as RectTransform).sizeDelta;
        }
    }
}
