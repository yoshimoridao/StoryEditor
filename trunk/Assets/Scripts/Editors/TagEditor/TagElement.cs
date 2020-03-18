using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class TagElement : MonoBehaviour, IDragElement
{
    [SerializeField]
    protected Vector2 sizeOffset = new Vector2(10, 10);

    // action
    public Action<GameObject> actOnDestroyElement;
    public Action actOnElementChangeSize;

    private RectTransform rt;
    private InputField inputField;
    // data
    private EventTagId tagId;

    public Color originColor { get; set; }

    public string Text
    {
        get { return inputField.text; }
        set
        {
            inputField.text = value;
            // refresh size
            RefreshContentSize();
        }
    }
    public InputField TagField { get { return inputField; } }
    public EventTagId TagId
    {
        get { return tagId; }
        set
        {
            ReleaseData();
            AddData(value);

            // change text
            Text = tagId.value;
        }
    }

    #region interface
    public void OnDragging()
    {
        GetComponent<Image>().color = DataDefine.highlight_drag_obj_color;
    }

    public void OnEndDrag()
    {
        GetComponent<Image>().color = originColor;
    }
    #endregion

    private void Start()
    {
        originColor = GetComponent<Image>().color;
    }

    private void OnDestroy()
    {
        ReleaseData();
    }

    private void AddData(EventTagId _val)
    {
        tagId = _val;

        // register event for tag id
        tagId.actOnModyingData += OnDataValChange;
        tagId.actOnDestroy += OnTagIdDestroyed;
    }

    private void ReleaseData()
    {
        if (tagId != null)
        {
            if (tagId.actOnModyingData != null)
                tagId.actOnModyingData -= OnDataValChange;
            if (tagId.actOnDestroy != null)
                tagId.actOnDestroy -= OnTagIdDestroyed;
        }
    }

    public void Init(EventTagId _tagId)
    {
        if (inputField == null)
            inputField = GetComponent<InputField>();
        if (rt == null)
            rt = (transform as RectTransform);

        TagId = _tagId;
    }

    public void DestroySelf(bool _isInvokeCallback = true)
    {
        if (_isInvokeCallback && actOnDestroyElement != null)
            actOnDestroyElement.Invoke(gameObject);

        // destroy obj
        Destroy(gameObject);
    }

    private void RefreshContentSize()
    {
        // to refresh size of content
        if (inputField)
        {
            rt.sizeDelta = new Vector2(inputField.preferredWidth + sizeOffset.x, inputField.preferredHeight + sizeOffset.y);
            if (actOnElementChangeSize != null)
                actOnElementChangeSize.Invoke();
        }
    }

    #region action
    public void OnDataValChange()
    {
        Text = tagId.value;

        GameMgr.Instance.RefreshCanvas();
    }

    public void OnTagIdDestroyed(string _genKey)
    {
        if (tagId.genKey == _genKey)
            DestroySelf(true);
    }
    #endregion
}
