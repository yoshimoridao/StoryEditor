using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagDeleteZone : MonoBehaviour, IDragZone
{
    [SerializeField]
    private RectTransform rtZone;
    private Vector2 moveZone = Vector2.zero;

    private bool isActive = false;
    private bool isLerp = false;
    private float targetPosY = 0;

    public bool IsDragIn { get; set; }
    public Color originColor { get; set; }

    void Start()
    {
        moveZone = new Vector2(rtZone.anchoredPosition.y, 0);
    }

    void Update()
    {
        // check cursor hover this zone
        if (CursorTagEditor.Instance && CursorTagEditor.Instance.DragingObj)
        {
            var dragingObj = CursorTagEditor.Instance.DragingObj;
            if (dragingObj && (dragingObj.GetComponent<TagEditorField>() || dragingObj.GetComponent<TagElement>()))
            {
                bool tmpIsActive = Util.IsHoverObj(gameObject);
                if (tmpIsActive != isActive)
                    ActiveZone(tmpIsActive);
            }
        }
        else if (isActive)
        {
            ActiveZone(false);
        }

        if (!isLerp)
            return;

        Vector2 zonePos = rtZone.anchoredPosition;
        zonePos.y = Mathf.Lerp(zonePos.y, targetPosY, 0.5f);
        if (Mathf.Abs(targetPosY - zonePos.y) <= 1)
        {
            zonePos.y = targetPosY;
            isLerp = false;
        }
        rtZone.anchoredPosition = zonePos;
    }

    #region interface
    public void OnMouseIn(GameObject obj)
    {
        IsDragIn = true;
    }

    public void OnMouseOut()
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;
    }

    public void OnMouseDrop(GameObject obj)
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;
        GetComponent<Image>().color = originColor;

        if (obj.GetComponent<TagEditorField>())
        {
            var tagField = obj.GetComponent<TagEditorField>();
            TagEditorFieldCont.Instance.RemoveEventTag(tagField.TagId);
        }
        else if (obj.GetComponent<TagElement>())
        {
            // remove tag element
            TagElement tagElement = obj.GetComponent<TagElement>();
            tagElement.DestroySelf(true);
        }
    }
    #endregion

    public void ActiveZone(bool _isActive)
    {
        isActive = _isActive;
        targetPosY = isActive ? moveZone.y : moveZone.x;
        isLerp = true;
    }
}
