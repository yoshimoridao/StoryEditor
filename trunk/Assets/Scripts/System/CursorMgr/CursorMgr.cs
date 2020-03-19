using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorMgr : Singleton<CursorMgr>
{
    public Action actOnRefreshSelectedObjs;

    public enum SelectBehavior { SINGLE, MULTIPLE };
    public enum DragBehavior { ARRANGE, CONNECT };

    public InputField dragingTitle;
    public float thresholdSelectTime = 0.1f;
    public float thresholdDragTime = 0.2f;
    public float thresholdDragDistance = 10.0f;

    RectTransform rt;

    Vector2 startPos = Vector2.zero;
    float holdDt = 0;
    DragBehavior dragBehavior = DragBehavior.CONNECT;
    SelectBehavior selectBehavior = SelectBehavior.SINGLE;

    GameObject dragingObj = null;
    private List<GameObject> selectObjs = new List<GameObject>();

    private IDragZone dragZone = null;
    private GameObject selectObj = null;

    // ========================================= GET/ SET =========================================
    public DragBehavior DragMode
    {
        get { return dragBehavior; }
        set { dragBehavior = value; }
    }

    public SelectBehavior SelectMode
    {
        get { return selectBehavior; }
        set { selectBehavior = value; }
    }

    public List<GameObject> GetSelectedObjs()
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
        DragMode = DragBehavior.ARRANGE;

        // hide title default
        ActiveTitle(false);
    }

    void Start()
    {

    }

    void Update()
    {
        // dont recognize mouse when popup show up
        if (PopupMgr.Instance.IsActive())
            return;

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

    public void Init()
    {
    }

    public void Load()
    {
        // clear all selected obj
        ClearSelectedObjs(true);
    }

    // ========================================= PUBLIC FUNCS =========================================
    public bool IsDragingObj()
    {
        return dragingObj != null;
    }

    public GameObject GetHandleObjOnTop()
    {
        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        if (rayCast != null && rayCast.Count > 0)
        {
            for (int i = 0; i < rayCast.Count; i++)
            {
                // get first object is handled by mouse (drag || select)
                GameObject catchObj = rayCast[i].gameObject;

                // layer 1 (unity's ui)
                if (catchObj.GetComponent<Button>() || catchObj.GetComponent<Dropdown>() || catchObj.GetComponent<Toggle>())
                    return catchObj.gameObject;
                // layer 2 (window)
                if (catchObj.GetComponent<FlexibleColorPicker>() || catchObj.tag == DataDefine.tag_window_event_tag)
                    return catchObj.gameObject;
                if (catchObj.GetComponent<ISelectElement>() != null || catchObj.GetComponent<IDragElement>() != null)
                    return catchObj.gameObject;
            }
        }

        return null;
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void UpdateMouseDown()
    {
        if (selectObj == null)
        {
            GameObject topObj = GetHandleObjOnTop();
            // process for obj handle by mouse
            if (topObj && (topObj.GetComponent<ISelectElement>() != null))
            {
                selectObj = topObj;
                startPos = Input.mousePosition;
                // count down time to active drag
                holdDt = 0;
            }
            // non - action
            else if (topObj == null)
            {
                ClearSelectedObjs(false, true);
            }
        }
    }

    private void UpdateMouseHold()
    {
        holdDt += Time.deltaTime;

        // in case draging an element
        if (dragingObj != null)
        {
            // catch mouse position
            rt.position = Input.mousePosition;

            // highlight hovering zone
            IDragZone hoverZone = GetDragZone();
            if (hoverZone != null)
            {
                if (hoverZone != dragZone)
                {
                    ReleaseDragZone();
                    dragZone = hoverZone;
                    dragZone.OnMouseIn(dragingObj.gameObject);
                }
            }
            else if (dragZone != null)
            {
                ReleaseDragZone();
            }
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
            if (holdDt <= thresholdSelectTime && selectObj.GetComponent<IDragElement>() != null)
            {
                // multiple select obj
                if (selectBehavior == SelectBehavior.MULTIPLE)
                {
                    AddSelectedObj(selectObj);
                }
                // singular select obj
                else
                {
                    // de-select obj (which already selected) 
                    if (selectObjs.Count > 0 && selectObjs[0].gameObject && selectObjs[0].gameObject == selectObj)
                    {
                        ClearSelectedObjs(false, true);
                    }
                    // select another obj
                    else
                    {
                        ClearSelectedObjs(false, false);
                        AddSelectedObj(selectObj);
                    }
                }
            }
        }

        startPos = Vector2.zero;
        selectObj = null;
        holdDt = 0;
    }

    private void ActiveTitle(bool isActive)
    {
        dragingTitle.gameObject.SetActive(isActive);
    }

    // === SELECT HANDLE ===
    private void AddSelectedObj(GameObject _element)
    {
        ISelectElement selectedElemnt = _element.GetComponent<ISelectElement>();
        if (selectedElemnt == null)
            return;

        int findId = selectObjs.FindIndex(x => x.gameObject == _element.gameObject);
        // add element
        if (findId == -1)
        {
            // enable the select element
            selectedElemnt.OnSelect();
            selectObjs.Add(_element);
        }
        // remove element (if it already had)
        else
        {
            selectedElemnt.OnEndSelect();
            selectObjs.RemoveAt(findId);
        }

        // call back action refresh list
        if (actOnRefreshSelectedObjs != null)
            actOnRefreshSelectedObjs.Invoke();
    }

    public void ClearSelectedObjs(bool isDelCurObj, bool isInvokeCallback = true)
    {
        if (isDelCurObj)
            selectObj = null;

        if (selectObjs.Count == 0)
            return;

        foreach (var element in selectObjs)
        {
            if (element.GetComponent<ISelectElement>() != null)
                element.GetComponent<ISelectElement>().OnEndSelect();
        }

        selectObjs.Clear();

        // call back action refresh list
        if (isInvokeCallback && actOnRefreshSelectedObjs != null)
            actOnRefreshSelectedObjs.Invoke();
    }

    // === DRAG HANDLE ===
    private IDragZone GetDragZone()
    {
        // get ray cast all objs
        var rayCast = Util.GetRayCastResultsByMousePos();
        if (rayCast != null && rayCast.Count > 0)
        {
            foreach (var catchObj in rayCast)
            {
                IDragZone comp = catchObj.gameObject.GetComponent<IDragZone>();
                if (comp != null)
                    return comp;
            }
        }

        return null;
    }
    private void ReleaseDragZone()
    {
        if (dragZone == null)
            return;

        dragZone.OnMouseOut();
        dragZone = null;
    }

    private void ProcessDrag()
    {
        // process mouse up on drag zone
        if (dragZone != null && dragingObj != null)
            dragZone.OnMouseDrop(dragingObj.gameObject);

        // release drag zone
        ReleaseDragZone();
        // de-active draging
        ActiveDrag(false);
    }

    private void ActiveDrag(bool isActive)
    {
        // process draging element
        if (isActive)
        {
            if (dragingObj == null && selectObj && selectObj.GetComponent<IDragElement>() != null)
            {
                // begin trigger drag event
                dragingObj = selectObj;

                // clear all selected objs
                ClearSelectedObjs(true);

                // highlight color of obj
                dragingObj.GetComponent<IDragElement>().OnDragging();

                // catch mouse position
                rt.position = Input.mousePosition;

                // visible title
                ActiveTitle(true);
            }
        }
        else
        {
            if (dragingObj.GetComponent<IDragElement>() != null)
                dragingObj.GetComponent<IDragElement>().OnEndDrag();
            dragingObj = null;

            // invisible title
            ActiveTitle(false);
        }
    }

    // === UTIL ===
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
