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
    public HighlighPanelMgr highlightPanel;
    public HighlighLabelMgr highlightLabel;

    RectTransform rt;

    Vector2 startPos = Vector2.zero;
    float holdDt = 0;
    DragBehavior dragBehavior = DragBehavior.CONNECT;
    SelectBehavior selectBehavior = SelectBehavior.SINGLE;

    DragAbleElement dragingObj = null;
    private List<SelectAbleElement> selectObjs = new List<SelectAbleElement>();
    private GameObject selectObj = null;
    private IDragZone dragZone = null;

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
        // default hide highlight panel
        if (highlightPanel && highlightPanel.IsActive())
            highlightPanel.Hide();
        // hide highlight label
        if (highlightLabel && highlightLabel.IsActive())
            highlightLabel.Hide();
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

    public bool IsHoverObjs(out GameObject obj, string catchingTag, params string[] tags)
    {
        obj = null;
        List<string> checkingTags = new List<string>(tags);
        checkingTags.Add(catchingTag);

        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        for (int i = 0; i < rayCast.Count; i++)
        {
            string touchedTag = rayCast[i].gameObject.tag;
            if (checkingTags.Contains(touchedTag))
            {
                // store obj match catching tag
                if (touchedTag == catchingTag)
                    obj = rayCast[i].gameObject;

                // remove the tag in list checking
                checkingTags.Remove(touchedTag);
                // return true if matched all of tags
                if (checkingTags.Count == 0)
                    return true;
            }
        }

        obj = null;
        return false;
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
                    //|| catchObj.GetComponent<InputField>())
                {
                    return catchObj.gameObject;
                }
                // layer 2 (window)
                if (catchObj.GetComponent<FlexibleColorPicker>() || catchObj.tag == DataDefine.tag_window_event_tag)
                {
                    return catchObj.gameObject;
                }
                // layer 3
                if (catchObj.GetComponent<SelectAbleElement>())
                {
                    // ignore select when editing input
                    if (catchObj.GetComponent<CustomInputField>() && catchObj.GetComponent<CustomInputField>().isFocused)
                        return null;

                    return catchObj.gameObject;
                }
                // layer 4
                if (catchObj.GetComponent<DragAbleElement>())
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
            GameObject topObj = GetHandleObjOnTop();
            // process for obj handle by mouse
            if (topObj && (topObj.GetComponent<SelectAbleElement>() || topObj.GetComponent<DragAbleElement>()))
            {
                selectObj = topObj;
                startPos = Input.mousePosition;
                // count down time to active drag
                holdDt = 0;
            }
            // non - action
            else if (topObj == null)
            {
                ClearSelectedObjs();
            }
        }

        //// Turn off color menu if user touched out of it
        //if (ColorMenu.Instance.IsActive() && !IsHoverObjs(DataDefine.tag_colorbar, DataDefine.tag_colorbar_btn))
        //    ColorMenu.Instance.SetActive(false);
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
                    if (selectObjs.Count > 0 && selectObjs[0].gameObject && selectObjs[0].gameObject == selectObj)
                    {
                        ClearSelectedObjs();
                    }
                    // select another obj
                    else
                    {
                        ClearSelectedObjs(false, false);
                        AddSelectedObj(selectObj.GetComponent<SelectAbleElement>());
                    }
                }
            }
        }
        //// non - action
        //else if (!IsPressAnyButton())
        //{
        //    ClearSelectedObjs();
        //}

        startPos = Vector2.zero;
        selectObj = null;
        holdDt = 0;
    }

    private void ActiveTitle(bool isActive)
    {
        dragingTitle.gameObject.SetActive(isActive);
        //if (isActive && dragingObj)
        //{
        //    Label label = dragingObj.GetLabelObj();

        //    // clone (text, font size, size delta) of draging object
        //    Text labelTitle = label.GetTextObject();
        //    dragingTitle.text = labelTitle.text;
        //    dragingTitle.GetComponentInChildren<Text>().fontSize = labelTitle.fontSize;
        //    (dragingTitle.transform as RectTransform).sizeDelta = (label.transform as RectTransform).sizeDelta;
        //}
    }

    // === SELECT HANDLE ===
    private void AddSelectedObj(SelectAbleElement element)
    {
        int findId = selectObjs.FindIndex(x => x.gameObject == element.gameObject);

        // add element
        if (findId == -1)
        {
            // enable the select element
            element.IsSelect = true;
            selectObjs.Add(element);
        }
        // remove element (if it already had)
        else
        {
            element.IsSelect = false;
            selectObjs.RemoveAt(findId);
        }

        // call back action refresh list
        if (actOnRefreshSelectedObjs != null)
            actOnRefreshSelectedObjs.Invoke();
    }

    public void ClearSelectedObjs(bool isDelCurObj = false, bool isInvokeCallback = true)
    {
        if (isDelCurObj)
            selectObj = null;

        if (selectObjs.Count == 0)
            return;

        foreach (var element in selectObjs)
            element.IsSelect = false;

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
        // process for Panel
        Panel dragPanel = dragingObj.GetComponent<Panel>();

        if (dragBehavior == DragBehavior.CONNECT && dragPanel)
        {
            GameObject catchObj = null;
            // drag from a panel to label
            if (IsHoverObjs(out catchObj, DataDefine.tag_label_input))
            {
                ReactLabel hoverLabel = catchObj.GetComponent<ReactLabel>();
                if (hoverLabel)
                    hoverLabel.OnDragPanelInto(dragPanel);
            }
            // draging from a panel to panel
            else if (IsHoverObjs(out catchObj, DataDefine.tag_panel_common))
            {
                Panel hoverPanel = catchObj.GetComponent<Panel>();
                if (hoverPanel)
                {
                    // for link function
                    string labelVal = "#" + dragPanel.Genkey + "#";
                    hoverPanel.AddLabel(labelVal);

                    // refresh canvas
                    GameMgr.Instance.RefreshCanvas();
                }
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
        // process draging element
        if (isActive)
        {
            if (dragingObj == null && selectObj && selectObj.GetComponent<DragAbleElement>())
            {
                // begin trigger drag event
                dragingObj = selectObj.GetComponent<DragAbleElement>();
                // clear all selected objs
                ClearSelectedObjs(true);
                //selectObj = null;

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
    //private bool IsPressAnyButton()
    //{
    //    // get ray cast all objs
    //    var rayCast = GetRayCastResultsByMousePos();
    //    foreach (var ray in rayCast)
    //    {
    //        if (ray.gameObject.GetComponent<Button>())
    //            return true;
    //        if (ray.gameObject.GetComponent<FlexibleColorPicker>())
    //            return true;
    //    }

    //    return false;
    //}

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
