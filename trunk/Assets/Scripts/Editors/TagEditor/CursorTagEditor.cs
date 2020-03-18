using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorTagEditor : Singleton<CursorTagEditor>
{
    public Action actOnRefreshSelectedObjs;

    public InputField dragingTitle;
    public float thresholdSelectTime = 0.1f;
    public float thresholdDragTime = 0.2f;
    public float thresholdDragDistance = 10.0f;

    RectTransform rt;

    Vector2 startPos = Vector2.zero;
    float holdDt = 0;

    private List<GameObject> selectObjs = new List<GameObject>();
    [SerializeField]
    private GameObject selectObj = null;

    private IDragZone dragZone = null;
    private GameObject dragingObj = null;

    public GameObject DragingObj
    {
        get { return dragingObj; }
    }

    // ========================================= GET/ SET =========================================
    public List<GameObject> GetSelectedObjs() { return selectObjs; }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

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

    private void OnEnable()
    {
        // clear all selected obj
        ClearSelectedObjs(true);
    }

    public void Init()
    {
        if (rt == null)
            rt = transform as RectTransform;

        // hide title default
        ActiveTitle(false);

        Load();
    }

    public void Load()
    {
        // clear all selected obj
        ClearSelectedObjs(true);
    }

    // ========================================= PUBLIC FUNCS =========================================
    private GameObject GetHandleObjOnTop()
    {
        // get ray cast all objs
        var rayCast = Util.GetRayCastResultsByMousePos();
        if (rayCast != null && rayCast.Count > 0)
        {
            for (int i = 0; i < rayCast.Count; i++)
            {
                // get first object is handled by mouse (drag || select)
                GameObject catchObj = rayCast[i].gameObject;

                // layer 1 (unity's ui)
                if (catchObj.GetComponent<Button>())
                {
                    return catchObj.gameObject;
                }
                // layer 2
                if (catchObj.GetComponent<IDragElement>() != null)
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
            if (topObj && (topObj.GetComponent<IDragElement>() != null))
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
    }

    private void UpdateMouseHold()
    {
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
            holdDt += Time.deltaTime;

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
        if (dragingObj != null)
        {
            ProcessDrag();
        }
        // process select object
        else if (selectObj)
        {
            if (holdDt <= thresholdSelectTime && selectObj.GetComponent<IDragElement>() != null)
            {
                // singular select obj
                // de-select obj (which already selected) 
                if (selectObjs.Count > 0 && selectObjs[0].gameObject && selectObjs[0].gameObject == selectObj)
                {
                    ClearSelectedObjs();
                }
                // select another obj
                else
                {
                    ClearSelectedObjs(false, false);
                    if (selectObj.GetComponent<IDragElement>() != null)
                        AddSelectedObj(selectObj);
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
        if (isActive && dragingObj != null)
        {
            // clone (text, font size, size delta) of draging object
            //InputField tagField = null;
            //if (dragingObj.GetComponent<TagEditorField>())
            //{
            //    tagField = dragingObj.GetComponent<TagEditorField>().TagField;
            //}
            //else if (dragingObj.GetComponent<TagElement>())
            //{
            //    tagField = dragingObj.GetComponent<TagElement>().TagField;
            //}

            //if (tagField != null)
            //{
            //    dragingTitle.text = tagField.text;
            //    dragingTitle.GetComponentInChildren<Text>().fontSize = tagField.GetComponentInChildren<Text>().fontSize;
            //    (dragingTitle.transform as RectTransform).sizeDelta = (dragingObj.transform as RectTransform).sizeDelta;
            //}
        }
    }

    // === SELECT HANDLE ===
    private void AddSelectedObj(GameObject element)
    {
        int findId = selectObjs.FindIndex(x => x.gameObject == element.gameObject);

        // add element
        if (findId == -1)
        {
            // enable the select element
            //element.IsSelect = true;
            selectObjs.Add(element);
        }
        // remove element (if it already had)
        else
        {
            //element.IsSelect = false;
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

        //foreach (var element in selectObjs)
        //    element.IsSelect = false;

        selectObjs.Clear();

        // call back action refresh list
        if (isInvokeCallback && actOnRefreshSelectedObjs != null)
            actOnRefreshSelectedObjs.Invoke();
    }

    // === DRAG HANDLE ===
    #region drag
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

    private void ActiveDrag(bool isActive)
    {
        // process draging element
        if (isActive)
        {
            if (dragingObj == null && selectObj && selectObj.GetComponent<IDragElement>() != null)
            {
                // begin trigger drag event
                dragingObj = selectObj;
                dragingObj.GetComponent<IDragElement>().OnDragging();

                // clear all selected objs
                ClearSelectedObjs(true);

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

    private void ProcessDrag()
    {
        // refresh canvas
        if (dragZone != null && dragingObj != null)
            dragZone.OnMouseDrop(dragingObj.gameObject);

        GameMgr.Instance.RefreshCanvas();
        // de-active draging
        ActiveDrag(false);
        // release drag zone
        ReleaseDragZone();
    }
    #endregion
}
