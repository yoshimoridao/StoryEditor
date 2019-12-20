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
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift))
        {
            if (CursorMgr.Instance.SelectMode == CursorMgr.SelectBehavior.SINGLE)
                CursorMgr.Instance.SelectMode = CursorMgr.SelectBehavior.MULTIPLE;
        }
        else
        {
            if (CursorMgr.Instance.SelectMode == CursorMgr.SelectBehavior.MULTIPLE)
                CursorMgr.Instance.SelectMode = CursorMgr.SelectBehavior.SINGLE;
        }
    }
}
