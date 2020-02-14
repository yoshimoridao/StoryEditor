using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTagMgr : Singleton<EventTagMgr>
{
    [SerializeField]
    private Transform eventTagMenu;
    [SerializeField]
    private Transform contTrans;
    [SerializeField]
    private EventTag prefEventTag;
    [SerializeField]
    private Button addTagBtn;
    // tag objects've generated
    private List<EventTag> eventTags = new List<EventTag>();

    // these label're selected
    [SerializeField]
    private List<Label> selectLabels = new List<Label>();

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        if (addTagBtn)
        {
            addTagBtn.onClick.RemoveListener(AddNewEventTag);
        }

        // unregister events
        if (CursorMgr.Instance && CursorMgr.Instance.actOnRefreshSelectedObjs != null)
            CursorMgr.Instance.actOnRefreshSelectedObjs -= OnRefreshSelectLabels;
    }
    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        // register event
        if (CursorMgr.Instance)
            CursorMgr.Instance.actOnRefreshSelectedObjs += OnRefreshSelectLabels;

        // add event button
        if (addTagBtn)
        {
            addTagBtn.onClick.AddListener(AddNewEventTag);
        }

        Load();
    }

    public void Load()
    {
        // default turn off window
        SetActiveWindow(false);

        // load tag ids
        List<EventTagId> tagIds = DataMgr.Instance.GetEventTags();

        // destroy template and get add btn
        if (contTrans)
        {
            int turn = Mathf.Max(eventTags.Count, tagIds.Count);
            for (int i = 0; i < turn; i++)
            {
                EventTagId tagId = null;
                if (i < tagIds.Count)
                    tagId = tagIds[i];

                if (tagId != null)
                {
                    // generate new tag
                    if (i >= eventTags.Count)
                    {
                        GenNewEventTag(tagId);
                    }
                    // re-use existed tag
                    else
                    {
                        eventTags[i].Init(tagId);
                    }
                }
                else
                {
                    // surplus tag -> destroy
                    if (tagIds.Count < eventTags.Count)
                        DestroyEventTag(eventTags[tagIds.Count].TagId);
                }
            }

            // refresh position of add btn
            RefreshAddBtnPos();
        }
    }

    public void ToggleWindow(RectTransform _snailPos)
    {
        // set position window
        bool isActive = !IsActive();

        if (isActive)
            eventTagMenu.transform.position = _snailPos.position;

        SetActiveWindow(isActive);
    }

    public bool IsActive()
    {
        if (eventTagMenu)
            return eventTagMenu.gameObject.active;

        return false;
    }

    public void SetActiveWindow(bool _isActive)
    {
        if (_isActive)
            CanvasMgr.Instance.RefreshCanvas();

        if (eventTagMenu)
        {
            eventTagMenu.gameObject.SetActive(_isActive);
        }
    }

    public void AddNewEventTag()
    {
        // gen new tag in data mgr
        EventTagId newTagId = DataMgr.Instance.AddEventTag(DataDefine.default_event_tag_value);

        GenNewEventTag(newTagId);
        // refresh position of add btn
        RefreshAddBtnPos();

        // refresh canvas
        CanvasMgr.Instance.RefreshCanvas();
    }

    public void OnEraseBtnPress(EventTagId _tagId)
    {
        // remove out of data
        DataMgr.Instance.RemoveEventTag(_tagId.genKey);

        // remove out of mgr list
        DestroyEventTag(_tagId);
    }

    public void OnValueChanged(EventTagId _tagId)
    {
        if (IsContainTag(_tagId))
        {
            DataMgr.Instance.ChangeEventTagVal(_tagId.genKey, _tagId.value);
        }
    }

    public void OnRefreshSelectLabels()
    {
        selectLabels.Clear();

        // add panel to colorize
        List<SelectAbleElement> selectedObjs = CursorMgr.Instance.GetSelectedObjs();
        foreach (SelectAbleElement element in selectedObjs)
        {
            // filter element label
            if (!element.GetComponent<ElementLabel>())
                continue;

            ElementLabel elementLabel = element.GetComponent<ElementLabel>();
            if (elementLabel && selectLabels.FindIndex(x => x.gameObject == elementLabel.gameObject) == -1)
            {
                selectLabels.Add(elementLabel);
            }
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void GenNewEventTag(EventTagId _eventTagId)
    {
        if (_eventTagId == null)
            return;

        // gen new tag object
        EventTag genEventTag = Instantiate(prefEventTag.gameObject, contTrans).GetComponent<EventTag>();
        genEventTag.Init(_eventTagId);

        // register events
        genEventTag.actOnErase += OnEraseBtnPress;
        genEventTag.actOnValChange += OnValueChanged;

        genEventTag.actOnToggleAply += OnToggleApplyTag;
        genEventTag.actOnToggleVisible += OnToggleVisibleTag;

        eventTags.Add(genEventTag);
    }

    private void DestroyEventTag(EventTagId _eventTagId)
    {
        int findId = eventTags.FindIndex(x => x.TagId == _eventTagId);
        if (findId != -1)
        {
            // destroy and remove out of mgr list
            EventTag eventTag = eventTags[findId];
            // un-register action
            if (eventTag.actOnErase != null)
                eventTag.actOnErase -= OnEraseBtnPress;
            if (eventTag.actOnValChange != null)
                eventTag.actOnValChange -= OnValueChanged;

            if (eventTag.actOnToggleAply != null)
                eventTag.actOnToggleAply -= OnToggleApplyTag;
            if (eventTag.actOnToggleVisible != null)
                eventTag.actOnToggleVisible -= OnToggleVisibleTag;

            Destroy(eventTag.gameObject);
            eventTags.RemoveAt(findId);
        }
    }

    private void RefreshAddBtnPos()
    {
        // refresh position of add btn
        addTagBtn.transform.SetAsLastSibling();
    }

    private bool IsContainTag(EventTagId _tagId)
    {
        int findId = eventTags.FindIndex(x => x.TagId == _tagId);

        return findId != -1;
    }

    // === Apply tag ===
    public void OnToggleApplyTag(EventTag _eventTag)
    {
        int findId = eventTags.FindIndex(x => x.gameObject == _eventTag.gameObject);

        if (findId != -1)
        {
            // toggle (apply or not) tag for selected label
            for (int i = 0; i < selectLabels.Count; i++)
            {
                ElementLabel label = selectLabels[i] as ElementLabel;
                label.OnToggleEventTag(_eventTag.TagId.genKey);
            }
        }
    }

    // === Visible tag ===
    public void OnToggleVisibleTag(EventTag _eventTag)
    {
        int findId = eventTags.FindIndex(x => x.gameObject == _eventTag.gameObject);

        if (findId != -1)
        {

        }
    }
}
