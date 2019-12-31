﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBarMgr : Singleton<ToolBarMgr>
{
    public Sprite arrangeModeImg;
    public Sprite connectModeImg;

    public Image arrangeBtnImg;
    public Button deleteBtn;
    public Button testingBtn;
    public Button colorBtn;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // default hide these btns
        SetActiveTestingBtn(false);
        SetActiveDelBtn(false);
        SetActiveColorBtn(false);

        // set sprite for arrange btn
        if (arrangeBtnImg && arrangeBtnImg && connectModeImg)
            arrangeBtnImg.sprite = CursorMgr.Instance.DragMode == CursorMgr.DragBehavior.ARRANGE ? arrangeModeImg : connectModeImg;
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void OnPressDragModeBtn()
    {
        CursorMgr cursorMgr = CursorMgr.Instance;
        switch (cursorMgr.DragMode)
        {
            case CursorMgr.DragBehavior.ARRANGE:
                cursorMgr.DragMode = CursorMgr.DragBehavior.CONNECT;
                break;
            case CursorMgr.DragBehavior.CONNECT:
                cursorMgr.DragMode = CursorMgr.DragBehavior.ARRANGE;
                break;
            default:
                break;
        }

        // set sprite for arrange btn
        if (arrangeBtnImg && arrangeBtnImg && connectModeImg)
            arrangeBtnImg.sprite = cursorMgr.DragMode == CursorMgr.DragBehavior.ARRANGE ? arrangeModeImg : connectModeImg;
    }

    public void OnPressTestingBtn()
    {
        List<SelectAbleElement> selectedObjs = CursorMgr.Instance.GetSelectedObjs();
        bool alreadyActiveAll = true;

        // get selected panel also check status all of them is active test tag
        List<Panel> panels = new List<Panel>();
        List<ElementLabel> labels = new List<ElementLabel>();
        foreach (SelectAbleElement element in selectedObjs)
        {
            Panel panel = element.GetComponent<Panel>();
            // testing with label
            if (panel)
            {
                if (!panel.IsTesting)
                    alreadyActiveAll = false;
                panels.Add(panel);
            }

            ElementLabel label = element.GetComponent<ElementLabel>();
            if (label)
            {
                if (!label.IsTesting)
                    alreadyActiveAll = false;
                labels.Add(label);
            }
        }

        // set active test tag of all panels (which selected)
        foreach (Panel panel in panels)
        {
            panel.IsTesting = !alreadyActiveAll;
            // save testing panel
            DataMgr.Instance.SetTestPanel(panel.DataType, panel.Key, panel.IsTesting);
        }

        // set active highlight panel of labels 
        foreach (ElementLabel label in labels)
            label.IsTesting = !alreadyActiveAll;
    }

    public void OnPressDestroyBtn()
    {
        // active destroy mode
        List<SelectAbleElement> elements = CursorMgr.Instance.GetSelectedObjs();
        for (int i = 0; i < elements.Count; i++)
        {
            SelectAbleElement element = elements[i];
            if (element && element.GetComponent<Panel>())
                element.GetComponent<Panel>().SelfDestroy();
            if (element && element.GetComponent<Label>())
                element.GetComponent<Label>().SelfDestroy();
        }

        // clear list
        elements.Clear();
    }

    public void OnPressColorBtn()
    {
        List<SelectAbleElement> selectedObjs = CursorMgr.Instance.GetSelectedObjs();
        // add panel to colorize
        foreach (SelectAbleElement element in selectedObjs)
        {
            Panel panel = element.GetComponent<Panel>();
            if (panel)
                ColorBar.Instance.AddReferralPanel(panel);
        }

        // show color bar
        ColorBar.Instance.SetActive(true);
    }

    public void RefreshButtonState()
    {
        List<SelectAbleElement> selectedObjs = CursorMgr.Instance.GetSelectedObjs();
        bool isSelectedPanel = false;
        bool isSelectedElementLabel = false;
        for (int i = 0; i < selectedObjs.Count; i++)
        {
            SelectAbleElement element = selectedObjs[i];
            Panel panel = element.GetComponent<Panel>();
            // checking is selected panel
            if (!isSelectedPanel && panel)
            {
                isSelectedPanel = true;
            }

            // checking is selected element board's label
            if (!isSelectedElementLabel)
            {
                Label label = element.GetComponent<Label>();
                if (label && label.Panel is ElementPanel)
                    isSelectedElementLabel = true;
            }

            if (isSelectedPanel && isSelectedElementLabel)
                break;
        }

        // set active testing btn
        SetActiveTestingBtn(isSelectedPanel || isSelectedElementLabel);
        // set active color btn
        SetActiveColorBtn(isSelectedPanel);
        // set active delete btn
        SetActiveDelBtn(selectedObjs.Count > 0);
    }
    // ========================================= PRIVATE FUNCS =========================================
    private void SetActiveDelBtn(bool isActive)
    {
        // set active delete btn
        if (deleteBtn)
            deleteBtn.gameObject.SetActive(isActive);
    }

    private void SetActiveTestingBtn(bool isActive)
    {
        // set active delete btn
        if (testingBtn)
            testingBtn.gameObject.SetActive(isActive);
    }

    private void SetActiveColorBtn(bool isActive)
    {
        // set active delete btn
        if (colorBtn)
            colorBtn.gameObject.SetActive(isActive);
    }
}
