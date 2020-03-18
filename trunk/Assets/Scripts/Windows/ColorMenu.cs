using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMenu : Singleton<ColorMenu>
{
    public FlexibleColorPicker fcp;

    // these panels're selected
    private List<Panel> selectPanels = new List<Panel>();
    private RectTransform rt;
    [SerializeField]
    private bool isDragging = false;
    private Vector3 mouseOffset = Vector3.zero;
    private Vector3 prevPos = Vector3.zero;
    private Color oldfcpColor;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        rt = GetComponent<RectTransform>();
        SetActiveFCP(false);

        if (CursorMgr.Instance)
            CursorMgr.Instance.actOnRefreshSelectedObjs += RefreshReferPanels;

        // register close btn event of fcp window
        if (fcp && fcp.colorToolbar && fcp.colorToolbar.closeBtn)
            fcp.colorToolbar.closeBtn.onClick.AddListener(OnFCPCloseBtnPress);
    }
    
    void Update()
    {
        // dont recognize mouse when popup show up
        if (PopupMgr.Instance.IsActive())
            return;

        // set color for each of panels
        if (fcp.gameObject.active)
        {
            // update color for selected panels
            if (oldfcpColor != fcp.color)
            {
                foreach (Panel panel in selectPanels)
                {
                    panel.RGBAColor = fcp.color;
                    // save color
                    //DataMgr.Instance.SetColorIndexData(panel.DataType, panel.Genkey, panel.RGBAColor);
                }

                oldfcpColor = fcp.color;
            }

            // start dragging
            if (!isDragging && Input.GetMouseButton(0) && 
                CursorMgr.Instance.GetHandleObjOnTop() == fcp.gameObject && Util.IsHoverObjs(DataDefine.tag_colormenu_topbar))
            {
                isDragging = true;
                mouseOffset = fcp.transform.position - Input.mousePosition;
            }

            // process dragging
            if (isDragging)
            {
                // update position
                if (Input.GetMouseButton(0))
                    fcp.transform.position = Input.mousePosition + mouseOffset;

                // stop dragging
                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                    prevPos = fcp.transform.localPosition;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (CursorMgr.Instance && CursorMgr.Instance.actOnRefreshSelectedObjs != null)
            CursorMgr.Instance.actOnRefreshSelectedObjs -= RefreshReferPanels;
    }

    // ========================================= PUBLIC FUNCS =========================================
    public bool IsActive()
    {
        return fcp.gameObject.active;
    }

    public void OnFCPCloseBtnPress()
    {
        SetActiveFCP(false);
    }
    public void SetActiveFCP(bool isActive)
    {
        // active color menu first
        fcp.gameObject.SetActive(isActive);
         
        if (isActive)
        {
            // set position menu
            fcp.transform.localPosition = prevPos;

            // refresh list of referral panels
            RefreshReferPanels();
        }
    }

    public void OnPressColorBtn()
    {
        if (IsActive())
            return;

        // show color menu
        SetActiveFCP(true);
    }

    private void RefreshReferPanels()
    {
        if (!IsActive())
            return;

        selectPanels.Clear();

        // add panel to colorize
        List<SelectAbleElement> selectedObjs = CursorMgr.Instance.GetSelectedObjs();
        foreach (SelectAbleElement element in selectedObjs)
        {
            Panel panel = element.GetComponent<Panel>();
            if (panel && selectPanels.FindIndex(x => x.gameObject == panel.gameObject) == -1)
            {
                selectPanels.Add(panel);
            }
        }

        // pick color of single panel
        if (selectPanels.Count == 1)
            fcp.color = selectPanels[0].RGBAColor;
        oldfcpColor = fcp.color;
    }

    //public Color GetColor(ColorType colorType)
    //{
    //    switch (colorType)
    //    {
    //        case ColorType.WHITE:
    //            return Color.white;
    //        case ColorType.BLACK:
    //            return Color.black;
    //        case ColorType.RED:
    //            return Color.red;
    //        case ColorType.CYAN:
    //            return Color.cyan;
    //        case ColorType.GREEN:
    //            return Color.green;
    //        case ColorType.BLUE:
    //            return Color.blue;
    //        case ColorType.ORANGE:
    //            return new Color(1, 0.5f, 0.0f, 1.0f);
    //        case ColorType.PURPLE:
    //            return new Color(1, 0.0f, 1, 1.0f);
    //        default:
    //            break;
    //    }
    //    return Color.white;
    //}
}
