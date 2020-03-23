using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KeyboardMgr : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        // hot key: select mode
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
        {
            if (CursorMgr.Instance.SelectMode == CursorMgr.SelectBehavior.SINGLE)
                CursorMgr.Instance.SelectMode = CursorMgr.SelectBehavior.MULTIPLE;
        }
        else
        {
            if (CursorMgr.Instance.SelectMode == CursorMgr.SelectBehavior.MULTIPLE)
                CursorMgr.Instance.SelectMode = CursorMgr.SelectBehavior.SINGLE;
        }

        // hot key: delete elements
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            OnDeleteButtonPress();
        }

        // hot key: active cheat panel
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DisplayMgr.Instance.DisplayCheatPanel();
        }

        // hot key: override save
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.S))
        {
            if (!DataMgr.Instance.SaveLastFile())
            {
                GameMgr.Instance.OpenSaveBrowser();
            }
        }
    }

    private void OnDeleteButtonPress()
    {
        // active destroy mode
        var elements = CursorMgr.Instance.GetSelectedObjs();
        foreach (var element in elements)
        {
            if (element && element.GetComponent<Panel>())
                element.GetComponent<Panel>().SelfDestroy();
            if (element && element.GetComponent<Label>())
                element.GetComponent<Label>().SelfDestroy();
        }

        // clear all selected items
        CursorMgr.Instance.ClearSelectedObjs(true, true);
    }
}
