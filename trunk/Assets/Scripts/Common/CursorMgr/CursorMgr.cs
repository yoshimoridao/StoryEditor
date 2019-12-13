using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorMgr : Singleton<CursorMgr>
{
    public enum SelectBehavior { SINGLE, MULTIPLE };
    public enum DragBehavior { CONNECT, ARRANGE, SCROLL };

    public InputField dragingTitle;
    public float thresholdSelectTime = 0.1f;
    public float thresholdDragTime = 0.2f;
    public float thresholdDragDistance = 10.0f;
    public HighlighPanelMgr highlightPanel;
    public HighlighLabelMgr highlightLabel;

    RectTransform rt;

    GameObject selectObj = null;
    Vector2 startPos = Vector2.zero;
    float holdDt = 0;
    DragBehavior dragBehavior = DragBehavior.CONNECT;
    SelectBehavior selectBehavior = SelectBehavior.SINGLE;

    DragAbleElement dragingObj = null;
    List<SelectAbleElement> selectObjs = new List<SelectAbleElement>();

    // ========================================= GET/ SET =========================================
    public DragBehavior DragMode
    {
        get { return dragBehavior; }
        set { dragBehavior = value; }
    }

    public SelectBehavior SelectMode
    {
        get { return selectBehavior; }
        set
        {
            selectBehavior = value;
            // clear old objs if disable mode
            if (selectBehavior != SelectBehavior.MULTIPLE)
                ClearAllSelectedObj();
        }
    }

    public List<SelectAbleElement> GetSelectedObjs()
    {
        return selectObjs;
    }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
        rt = transform as RectTransform;

        // set default cursor mode
        SelectMode = SelectBehavior.SINGLE;
        DragMode = DragBehavior.SCROLL;

        // hide title default
        ActiveTitle(false);
    }

    void Start()
    {
        // default hide highlight panel
        if (highlightPanel && highlightPanel.IsActive())
            highlightPanel.Hide();
        // hide highlight label
        if (highlightLabel && highlightLabel.IsActive())
            highlightLabel.Hide();
    }

    void Update()
    {
        // mouse down
        if (Input.GetMouseButtonDown(0))
            UpdateMouseDown();

        // mouse hold
        if (Input.GetMouseButton(0))
            UpdateMouseHold();

        // mouse up
        if (Input.GetMouseButtonUp(0))
            OnMouseUp();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public bool IsDragingObj()
    {
        return dragingObj != null;
    }

    /// checking cursor's selecting the elements.
    public bool IsHoverObjs(out GameObject obj, params string[] tags)
    {
        obj = null;

        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        for (int i = 0; i < rayCast.Count; i++)
        {
            string touchedTag = rayCast[i].gameObject.tag;
            foreach (string tag in tags)
            {
                if (touchedTag == tag)
                {
                    obj = rayCast[i].gameObject;
                    return true;
                }
            }
        }
        return false;
    }

    public GameObject CatchElementHandleByMouse()
    {
        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        if (rayCast != null && rayCast.Count > 0)
        {
            for (int i = 0; i < rayCast.Count; i++)
            {
                // get first object is handled by mouse (drag || select)
                GameObject catchObj = rayCast[i].gameObject;
                if (catchObj.GetComponent<Button>())
                    return null;

                if (catchObj.GetComponent<DragAbleElement>() || catchObj.GetComponent<SelectAbleElement>())
                {
                    return catchObj.gameObject;
                }
            }
        }

        return null;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void UpdateMouseDown()
    {
        if (selectObj == null)
        {
            GameObject catchElement = CatchElementHandleByMouse();
            if (catchElement)
            {
                selectObj = catchElement;
                startPos = Input.mousePosition;
                // count down time to active drag
                holdDt = 0;
            }
        }

        // Turn off color bar if user touched out of it
        GameObject catchObj = null;
        // handle draging from a panel to another panel
        if (ColorBar.Instance.IsActive() && !IsHoverObjs(out catchObj, DataDefine.tag_colorbar, DataDefine.tag_colorbar_btn))
            ColorBar.Instance.SetActiveGameObject(false);
    }

    private void UpdateMouseHold()
    {
        // ignore process with SCROLL behavior
        if (dragBehavior == DragBehavior.SCROLL)
            return;

        holdDt += Time.deltaTime;

        // in case draging an element
        if (dragingObj)
        {
            // catch mouse position
            rt.position = Input.mousePosition;
        }
        else
        {
            // active drag by hold long
            if (holdDt >= thresholdDragTime)
            {
                holdDt = 0;
                ActiveDrag(true);
            }
            // active drag by distance
            if (startPos != Vector2.zero && Vector2.Distance(Input.mousePosition, startPos) >= thresholdDragDistance)
            {
                holdDt = 0;

                Debug.Log("start Pos = " + startPos);
                ActiveDrag(true);
            }
        }
    }

    private void OnMouseUp()
    {
        // process draging
        if (dragingObj)
        {
            ProcessDrag();
        }
        // process select object
        else if (selectObj)
        {
            if (holdDt <= thresholdSelectTime && selectObj.GetComponent<SelectAbleElement>())
            {
                // multiple select obj
                if (selectBehavior == SelectBehavior.MULTIPLE)
                {
                    AddSelectedObj(selectObj.GetComponent<SelectAbleElement>());
                }
                // singular select obj
                else
                {
                    // de-select obj (which already selected) 
                    if (selectObjs.Count > 0 && selectObjs[0].gameObject == selectObj)
                    {
                        ClearAllSelectedObj();
                    }
                    // re-select another obj
                    else
                    {
                        ClearAllSelectedObj();
                        AddSelectedObj(selectObj.GetComponent<SelectAbleElement>());
                    }
                }
            }
        }
        // non - action
        else if (!IsPressAnyButton())
        {
            ClearAllSelectedObj();
        }

        startPos = Vector2.zero;
        selectObj = null;
        holdDt = 0;
    }

    private void ActiveTitle(bool isActive)
    {
        dragingTitle.gameObject.SetActive(isActive);
        if (isActive && dragingObj)
        {
            Label label = dragingObj.GetLabelObj();

            // clone (text, font size, size delta) of draging object
            Text labelTitle = label.GetTextObject();
            dragingTitle.text = labelTitle.text;
            dragingTitle.GetComponentInChildren<Text>().fontSize = labelTitle.fontSize;
            (dragingTitle.transform as RectTransform).sizeDelta = (label.transform as RectTransform).sizeDelta;
        }
    }

    // === SELECT HANDLE ===
    private void AddSelectedObj(SelectAbleElement element)
    {
        int findId = selectObjs.FindIndex(x => x.gameObject == element.gameObject);

        // add element
        if (findId == -1)
        {
            // enable the select element
            element.Select = true;
            selectObjs.Add(element);
        }
        // remove element (if it already had)
        else
        {
            element.Select = false;
            selectObjs.RemoveAt(findId);
        }
    }

    private void ClearAllSelectedObj()
    {
        foreach (var element in selectObjs)
            element.Select = false;

        selectObjs.Clear();
    }

    // === DRAG HANDLE ===
    private void ProcessDrag()
    {
        // process for Panel
        if (dragingObj.GetComponent<CommonPanel>())
        {
            GameObject catchObj = null;
            // handle draging from a panel to another panel
            if (IsHoverObjs(out catchObj, DataDefine.tag_board_element, DataDefine.tag_board_story) && IsHoverObjs(out catchObj, DataDefine.tag_panel_common))
            {
                CommonPanel hoverPanel = catchObj.GetComponent<CommonPanel>();
                if (hoverPanel && hoverPanel.GetTitle() != dragingObj.GetLabelObj().GetText())
                {
                    switch (dragBehavior)
                    {
                        // for link function
                        case DragBehavior.CONNECT:
                            hoverPanel.AddLinkLabel(dragingObj.GetComponent<CommonPanel>());
                            break;
                            //// for arrange element function
                            //case DragBehavior.ARRANGE:
                            //    if (highlightPanel.gameObject.active)
                            //        highlightPanel.ArrangePanel(hoverPanel);
                            //    break;
                    }
                }
            }
            // handle draging element to result window
            else if (IsHoverObjs(out catchObj, DataDefine.tag_board_result))
            {
                if (catchObj.GetComponent<OriginBoard>() && dragingObj.GetComponent<CommonPanel>())
                    catchObj.GetComponent<OriginBoard>().ShowResult(dragingObj.GetComponent<CommonPanel>());
            }
        }
        // process for Label
        else if (dragingObj.GetComponent<Label>())
        {

        }

        // de-active draging
        ActiveDrag(false);
    }

    private void ActiveDrag(bool isActive)
    {
        if (isActive)
        {
            // clear all selected objs
            ClearAllSelectedObj();

            if (dragingObj == null && selectObj && selectObj.GetComponent<DragAbleElement>())
            {
                // begin trigger drag event
                dragingObj = selectObj.GetComponent<DragAbleElement>();
                selectObj = null;

                // catch mouse position
                rt.position = Input.mousePosition;

                // visible title
                ActiveTitle(true);

                // enable highlight obj for this panel
                if (dragingObj.GetComponent<Panel>() && highlightPanel)
                    highlightPanel.Show(dragingObj.GetComponent<Panel>());
                if (dragingObj.GetComponent<Label>() && highlightLabel)
                    highlightLabel.Show(dragingObj.GetComponent<Label>());
            }
        }
        else
        {
            dragingObj = null;

            // invisible title
            ActiveTitle(false);

            // hide highlight panel
            if (highlightPanel && highlightPanel.IsActive())
                highlightPanel.Hide();
            // hide highlight label
            if (highlightLabel && highlightLabel.IsActive())
                highlightLabel.Hide();
        }
    }

    // === UTIL ===
    private bool IsPressAnyButton()
    {
        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        foreach (var ray in rayCast)
            if (ray.gameObject.GetComponent<Button>())
                return true;

        return false;
    }

    private List<RaycastResult> GetRayCastResultsByMousePos()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // cast all obj 
        List<RaycastResult> ray = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, ray);

        return ray;
    }
}
