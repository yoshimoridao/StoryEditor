using System.Collections;
using System.Collections.Generic;
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
            MidToolBarMgr.Instance.OnDeleteButtonPress();
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
                CanvasMgr.Instance.OpenSaveBrowser();
            }
        }
    }
}
