using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GroupFlowTagElement : MonoBehaviour, IDragZone
{
    [SerializeField]
    protected GameObject prefTagElement;

    // action
    public Action<GameObject> actOnDestroy;
    public Action<GameObject> actOnElementChange;

    [SerializeField]
    private List<TagElement> tagElements = new List<TagElement>();

    public bool IsDragIn { get; set; }
    public Color originColor { get; set; }

    public List<TagElement> TagElements
    {
        get { return tagElements; }
    }

    private void Awake()
    {
        // delete all child
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    private void Start()
    {
    }

    #region interface
    public void OnMouseIn(GameObject obj)
    {
        if (obj.GetComponent<TagEditorField>())
        {
            IsDragIn = true;

            // store origin color before change highlight color
            originColor = GetComponent<Image>().color;
            GetComponent<Image>().color = DataDefine.highlight_drop_zone_color;
        }
    }

    public void OnMouseOut()
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;

        GetComponent<Image>().color = originColor;
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
            AddTag(tagField.TagId, true);
        }
    }
    #endregion

    public void SetTagElements(List<EventTagId> _tagElements)
    {
        int childCount = tagElements.Count;
        for (int i = 0; i < _tagElements.Count; i++)
        {
            EventTagId tagId = _tagElements[i];

            // create new tag
            if (i >= tagElements.Count)
                AddTag(tagId, false);
            // set tag for existing tag
            else
                tagElements[i].TagId = tagId;
        }

        // destroy surplus tags
        for (int i = _tagElements.Count; i < tagElements.Count; i++)
            tagElements[i].DestroySelf(false);
    }

    public void AddTag(EventTagId _tagId, bool _isInvokeCallback)
    {
        // generate new element
        GameObject newElement = Instantiate(prefTagElement, transform);
        if (newElement)
        {
            TagElement genElement = newElement.GetComponent<TagElement>();
            // init new element
            genElement.Init(_tagId);

            // register event
            genElement.actOnDestroyElement += OnTagElementDestroyed;

            // store generated element
            tagElements.Add(genElement);

            // call back
            if (_isInvokeCallback && actOnElementChange != null)
                actOnElementChange.Invoke(gameObject);
        }
    }

    public void OnTagElementDestroyed(GameObject _element)
    {
        int findId = tagElements.FindIndex(x => x.gameObject == _element.gameObject);
        if (findId != -1)
        {
            // un-register event
            tagElements[findId].actOnDestroyElement -= OnTagElementDestroyed;
            tagElements.RemoveAt(findId);

            // call back on event element destroyed
            if (actOnElementChange != null)
                actOnElementChange.Invoke(gameObject);

            // destroy group if all of element were destroyed
            if (tagElements.Count <= 0)
                DestroySelf(true);
        }
    }

    public void DestroySelf(bool _isInvokeCallback = true)
    {
        if (_isInvokeCallback && actOnDestroy != null)
            actOnDestroy.Invoke(gameObject);

        // destroy obj
        Destroy(gameObject);
    }
}
