using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomInputField : InputField
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        // check twice click to start edit (only for input_label)
        if (eventData.clickCount == 2)
        {
            ActivateInputField();
            if (GetComponent<ReactLabel>())
            {
                // convert show text to form editing (which without rich text <b><color>,...)
                GetComponent<ReactLabel>().ConvertToEditText();

                CursorMgr.Instance.ClearSelectedObjs(true);
            }
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        //// [*] check select by mouse (for specific objs)
        //if (gameObject.tag == DataDefine.tag_button_toolbar)
        //{
        //    if (CursorMgr.Instance.IsHoverObjs(gameObject))
        //        base.OnSelect(eventData);
        //    else
        //        DeactivateInputField();
        //}
    }
}
