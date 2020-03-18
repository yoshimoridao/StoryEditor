using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTagMgr : Singleton<EventTagMgr>
{
    // tag menu
    [SerializeField] private Transform tagMenu;
    [SerializeField] private Transform contTrans;
    [SerializeField] private EventTag prefEventTag;
    [SerializeField] private Button addTagBtn;
    // tag objects've generated
    private List<EventTag> eventTags = new List<EventTag>();

    // these label're selected
    private List<Label> selectLabels = new List<Label>();
    private bool isInit;

    // test mode
    [SerializeField] private Transform testModeMenu;
    [SerializeField] private Toggle selectTestBtn;
    [SerializeField] private Toggle tagTestBtn;
    [SerializeField] private Toggle groupTestBtn;
    [SerializeField] private Toggle grammarTestBtn;

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

    private void OnEnable()
    {
        if (isInit)
            Load();
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
        isInit = true;

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
            for (int i = 0; i < tagIds.Count; i++)
            {
                EventTagId tagId = tagIds[i];
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

            // remove surplus tags
            if (tagIds.Count < eventTags.Count)
            {
                for (int i = tagIds.Count; i < eventTags.Count; i++)
                {
                    // if remove event tag success
                    if (RemoveEventTag(i))
                        i--;
                }
            }

            // refresh position of add btn
            RefreshAddBtnPos();
        }
    }

    public bool IsActive()
    {
        if (tagMenu)
            return tagMenu.gameObject.active;

        return false;
    }

    public void SetActiveWindow(bool _isActive)
    {
        if (_isActive)
            GameMgr.Instance.RefreshCanvas();

        if (tagMenu)
        {
            tagMenu.gameObject.SetActive(_isActive);
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
        GameMgr.Instance.RefreshCanvas();
    }

    public void OnEraseBtnPress(EventTagId _tagId)
    {
        // remove out of mgr list
        int findId = eventTags.FindIndex(x => x.TagId == _tagId);
        if (findId != -1)
        {
            RemoveEventTag(findId);
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

        // active apply toggle of event tag base on element
        if (selectLabels.Count == 1)
        {
            DataElementIndex elementDataIndex = (selectLabels[0] as ElementLabel).GetDataElementIndex();

            if (elementDataIndex != null)
            {
                for (int i = 0; i < eventTags.Count; i++)
                {
                    EventTag tmpEventTag = eventTags[i];
                    if (tmpEventTag.TagId == null)
                        continue;

                    tmpEventTag.IsApplyTag = elementDataIndex.IsContainEventTag(tmpEventTag.TagId.genKey);
                }
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
        genEventTag.actOnToggleApply += OnToggleApplyTag;

        eventTags.Add(genEventTag);
    }

    private bool RemoveEventTag(int _index)
    {
        if (_index < eventTags.Count)
        {
            // destroy and remove out of mgr list
            EventTag eventTag = eventTags[_index];
            // un-register action
            if (eventTag.actOnErase != null)
                eventTag.actOnErase -= OnEraseBtnPress;
            if (eventTag.actOnToggleApply != null)
                eventTag.actOnToggleApply -= OnToggleApplyTag;

            // destroy obj && remove in list obj
            Destroy(eventTag.gameObject);
            eventTags.RemoveAt(_index);

            return true;
        }
        return false;
    }

    private void RefreshAddBtnPos()
    {
        // refresh position of add btn
        addTagBtn.transform.SetAsLastSibling();
    }

    // === Apply tag ===
    #region event
    public void OnToggleApplyTag(EventTag _eventTag, bool _isActive)
    {
        int findId = eventTags.FindIndex(x => x.gameObject == _eventTag.gameObject);

        if (findId != -1)
        {
            // toggle (apply or not) tag for selected label
            for (int i = 0; i < selectLabels.Count; i++)
            {
                ElementLabel label = selectLabels[i] as ElementLabel;
                label.ActiveEventTag(_eventTag.TagId.genKey, _isActive);
            }
        }
    }

    public void OnToggleTagMenu(RectTransform _snailPos)
    {
        // set position window
        bool isActive = !IsActive();

        if (isActive)
            tagMenu.transform.position = _snailPos.position;

        SetActiveWindow(isActive);
    }

    public void OnToggleTestModeMenu(RectTransform _snailPos)
    {
        // toggle enable
        testModeMenu.gameObject.SetActive(!testModeMenu.gameObject.active);

        // set position window
        testModeMenu.position = _snailPos.position;

        // toggle btn
        selectTestBtn.isOn = DataMgr.Instance.IsActiveSelectTest;
        tagTestBtn.isOn = DataMgr.Instance.IsActiveTagTest;
        groupTestBtn.isOn = DataMgr.Instance.IsActiveGroupTest;
        grammarTestBtn.isOn = DataMgr.Instance.IsActiveGrammarTest;

        GameMgr.Instance.RefreshCanvas();
    }

    public void OnToggleSelectTest(bool _isOn)
    {
        DataMgr.Instance.IsActiveSelectTest = _isOn;
    }
    public void OnToggleTagTest(bool _isOn)
    {
        DataMgr.Instance.IsActiveTagTest = _isOn;
    }
    public void OnToggleGroupTest(bool _isOn)
    {
        DataMgr.Instance.IsActiveGroupTest = _isOn;
    }
    public void OnToggleGrammarTest(bool _isOn)
    {
        DataMgr.Instance.IsActiveGrammarTest = _isOn;
    }
    #endregion

    // === Visible tag ===
}
