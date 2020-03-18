using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagEditorField : MonoBehaviour, IDragElement
{
    [SerializeField]
    private Vector2 sizeOffset = new Vector2(7, 2);
    [SerializeField]
    private InputField inputField;

    private EventTagId tagId;
    private int maxWidth;

    public Color originColor { get; set; }

    public EventTagId TagId
    {
        get { return tagId; }
        set
        {
            tagId = value;
            inputField.text = tagId.value;
            RefreshContentSize();
        }
    }

    public InputField TagField
    {
        get { return inputField; }
    }

    void Start()
    {
        originColor = inputField.GetComponent<Image>().color;
    }

    void Update()
    {

    }

    #region interface
    public void OnDragging()
    {
        if (inputField)
            inputField.GetComponent<Image>().color = DataDefine.highlight_drag_obj_color;
    }

    public void OnEndDrag()
    {
        if (inputField)
            inputField.GetComponent<Image>().color = originColor;
    }
    #endregion

    public void Init(EventTagId _tagId, int _maxWidth)
    {
        tagId = _tagId;
        maxWidth = _maxWidth;

        if (inputField)
        {
            // set text for input field
            inputField.text = tagId.Value;
            // register event for input field
            inputField.onEndEdit.AddListener(OnEndEditTagField);
            inputField.onValueChanged.AddListener(OnTagFieldEditing);
        }

        RefreshContentSize();
    }

    public void OnTagFieldEditing(string _val)
    {
        RefreshContentSize();
    }

    public void OnEndEditTagField(string _val)
    {
        if (tagId == null)
            return;

        tagId.Value = _val;
        RefreshContentSize();

        GameMgr.Instance.RefreshCanvas();
    }

    private void RefreshContentSize()
    {
        if (inputField)
        {
            RectTransform fieldRt = inputField.transform as RectTransform;
            Vector2 fieldSize = new Vector2(inputField.preferredWidth + sizeOffset.x * 2, inputField.preferredHeight + sizeOffset.y * 2);
            if (fieldSize.x > maxWidth)
                fieldSize.x = maxWidth;

            fieldRt.sizeDelta = fieldSize;
        }
    }
}
