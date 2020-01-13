using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FontSizeButton : MonoBehaviour
{
    public Action<int> actOnModifyVal;

    private int oldSize = -1;
    private InputField inputField;

    void Start()
    {
        if (inputField == null)
            inputField = GetComponent<InputField>();
    }

    void Update()
    {

    }

    // ==================================== PUBLIC ====================================
    public void Init()
    {
        if (inputField == null)
            inputField = GetComponent<InputField>();

        // set text size
        if (inputField)
            inputField.text = DataMgr.Instance.NormalFontSize.ToString();

        ChangeFontSize(DataMgr.Instance.NormalFontSize);
    }

    public void OnEditDone()
    {
        int curSize = -1;
        if (int.TryParse(inputField.text, out curSize) && curSize > 0)
        {
            // save
            if (curSize != oldSize && oldSize != -1)
            {
                DataMgr.Instance.NormalFontSize = curSize;

                ChangeFontSize(curSize);
            }
        }
        // revert if input is not number format
        else if (oldSize != -1)
        {
            inputField.text = oldSize.ToString();
        }
    }

    public void OnEditing()
    {
        int curSize = -1;
        if (int.TryParse(inputField.text, out curSize) && curSize > 0)
            ChangeFontSize(curSize);
    }

    private void ChangeFontSize(int _val)
    {
        oldSize = _val;

        // invoke action
        if (actOnModifyVal != null)
            actOnModifyVal.Invoke(_val);

        CanvasMgr.Instance.RefreshCanvas();
    }
}
