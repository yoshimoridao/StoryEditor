using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowTagPanel : MonoBehaviour, IDragZone
{
    [SerializeField]
    private InputField title;
    [SerializeField]
    private RectTransform rtViewport;
    [SerializeField]
    private RectTransform transCont;
    [SerializeField]
    private Button delBtn;

    [SerializeField]
    private GameObject prefGroupElemt;

    // action
    public Action<GameObject> actOnDestroyPanel;

    // data
    private DataTagFlow data;
    List<GroupFlowTagElement> groupTags = new List<GroupFlowTagElement>();

    public bool IsDragIn { get; set; }
    public Color originColor { get; set; }

    public DataTagFlow Data
    {
        get { return data; }
    }

    void Start()
    {
        originColor = GetComponent<Image>().color;
    }

    void Update()
    {

    }

    #region interface
    public void OnMouseIn(GameObject obj)
    {
        if (obj.GetComponent<TagEditorField>())
        {
            IsDragIn = true;
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
            AddGroup(tagField.TagId, true);
        }
    }
    #endregion

    #region common
    public void Init(DataTagFlow _data)
    {
        // delete all child
        for (int i = 0; i < transCont.childCount; i++)
            Destroy(transCont.GetChild(i).gameObject);

        // register listener for button
        if (delBtn)
            delBtn.onClick.AddListener(delegate { DestroySelf(true); });

        // load data tag group
        SetData(_data);
    }

    public void DestroySelf(bool _isInvokeCallback = true)
    {
        if (_isInvokeCallback && actOnDestroyPanel != null)
            actOnDestroyPanel.Invoke(gameObject);

        Destroy(gameObject);
    }

    public void LoadContentPanel()
    {
        if (data == null)
            return;

        char[] splitterGroup = { '_' };
        var splitGroups = data.val.Split(splitterGroup, StringSplitOptions.RemoveEmptyEntries);
        // split groups
        for (int i = 0; i < splitGroups.Length; i++)
        {
            string valGroup = splitGroups[i];
            char[] splitterTag = { '@' };
            var splitTags = valGroup.Split(splitterTag, StringSplitOptions.RemoveEmptyEntries);
            if (splitTags.Length == 0)
                continue;

            // load tag of groups
            List<EventTagId> eventTagIds = new List<EventTagId>();
            foreach (string tagKey in splitTags)
            {
                EventTagId eventTagId = DataMgr.Instance.GetEventTag("@" + tagKey);
                if (eventTagId != null)
                    eventTagIds.Add(eventTagId);
            }

            // generate group
            if (eventTagIds.Count > 0)
            {
                // add new group
                if (i >= groupTags.Count)
                {
                    GroupFlowTagElement groupFlowTag = AddEmptyGroup();
                    foreach (var eventTag in eventTagIds)
                        groupFlowTag.AddTag(eventTag, false);
                }
                // set new element for existing
                else
                {
                    groupTags[i].SetTagElements(eventTagIds);
                }
            }
        }

        // destroy sur-plus tag elements
        for (int i = splitGroups.Length; i < groupTags.Count; i++)
        {
            RemoveTag(i);
        }
    }
    #endregion

    // ======= Element Tag =======
    #region element_tag
    public GroupFlowTagElement AddGroup(EventTagId _tagId, bool _isUpdateData)
    {
        GroupFlowTagElement flowGroup = AddEmptyGroup();
        if (flowGroup)
            flowGroup.AddTag(_tagId, true);

        if (_isUpdateData)
            UpdateDataVal();

        return flowGroup;
    }

    private GroupFlowTagElement AddEmptyGroup()
    {
        GameObject newGroup = Instantiate(prefGroupElemt, transCont);
        if (newGroup)
        {
            GroupFlowTagElement newGroupFlow = newGroup.GetComponent<GroupFlowTagElement>();
            // store in list
            groupTags.Add(newGroupFlow);

            // register event
            newGroupFlow.actOnDestroy += OnFlowTagDestroyed;
            newGroupFlow.actOnElementChange += OnFlowTagModified;

            return newGroupFlow;
        }

        return null;
    }

    private bool RemoveTag(int _id)
    {
        if (_id < groupTags.Count)
        {
            //un - register event
            // register event
            GroupFlowTagElement groupElemnt = groupTags[_id];
            if (groupElemnt.actOnDestroy != null)
                groupElemnt.actOnDestroy -= OnFlowTagDestroyed;
            if (groupElemnt.actOnElementChange != null)
                groupElemnt.actOnElementChange -= OnFlowTagModified;

            // remove out of manage list
            groupTags.RemoveAt(_id);
            return true;
        }

        return false;
    }
    #endregion

    // ======= Event =======
    #region event
    public void OnFlowTagModified(GameObject _tag)
    {
        // update data
        UpdateDataVal();

        StartCoroutine("RefreshViewportHeight");
    }
    IEnumerator RefreshViewportHeight()
    {
        yield return new WaitForSeconds(0.1f);

        float contentHeight = transCont.sizeDelta.y;
        Vector2 viewportSize = rtViewport.sizeDelta;
        viewportSize.y = contentHeight + 15 * 2;
        rtViewport.sizeDelta = viewportSize;
    }

    public void OnFlowTagDestroyed(GameObject _tag)
    {
        int findId = groupTags.FindIndex(x => x.gameObject == _tag.gameObject);
        if (findId != -1)
        {
            RemoveTag(findId);

            // update data
            UpdateDataVal();
        }
    }
    #endregion

    // ======= Data =======
    #region data
    public void SetData(DataTagFlow _val)
    {
        data = _val;

        // change title
        if (title)
        {
            title.text = data.title;
            // register action for title
            title.onEndEdit.AddListener(OnTitleEditEnd);
        }

        // load content for panel
        LoadContentPanel();
    }

    private void UpdateDataVal()
    {
        if (data == null)
            return;

        // data format: @1@2_@3_@4@5@6_...
        string dataVal = "";
        foreach (var group in groupTags)
        {
            string elementVal = "";
            List<TagElement> elements = group.TagElements;
            foreach (var element in elements)
                elementVal += element.TagId.genKey;

            if (elementVal.Length > 0)
            {
                // mark splits groups
                if (dataVal.Length > 0)
                    dataVal += "_";
                // append element value
                dataVal += elementVal;
            }
        }

        data.val = dataVal;
    }
    #endregion

    // ======= Title =======
    #region title
    public void OnTitleEditEnd(string _val)
    {
        if (data != null)
            data.title = _val;
    }
    #endregion
}
