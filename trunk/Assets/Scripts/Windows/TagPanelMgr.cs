using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagPanelMgr : Singleton<TagPanelMgr>
{
    // tag menu
    [SerializeField] private Transform tagMenu;
    [SerializeField] private Transform contTrans;
    [SerializeField] private TagPanelItem prefTagItem;
    // tag objects've generated
    private List<TagPanelItem> eventTags = new List<TagPanelItem>();

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
        // destroy child objs
        for (int i = 0; i < contTrans.childCount; i++)
            Destroy(contTrans.GetChild(i).gameObject);

        // register button event
        if (selectTestBtn)
            selectTestBtn.onValueChanged.AddListener(OnToggleSelectTest);
        if (tagTestBtn)
            tagTestBtn.onValueChanged.AddListener(OnToggleTagTest);
        if (groupTestBtn)
            groupTestBtn.onValueChanged.AddListener(OnToggleGroupTest);
        if (grammarTestBtn)
            grammarTestBtn.onValueChanged.AddListener(OnToggleGrammarTest);
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

        Load();
    }

    public void Load()
    {
        // default turn off window
        SetActiveWindow(false);

        // load tag ids
        List<EventTagId> tagIds = DataMgr.Instance.GetEventTags();

        // destroy template and get add btn
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
        var selectedObjs = CursorMgr.Instance.GetSelectedObjs();
        foreach (var element in selectedObjs)
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
                    TagPanelItem tmpEventTag = eventTags[i];
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
        TagPanelItem genEventTag = Instantiate(prefTagItem.gameObject, contTrans).GetComponent<TagPanelItem>();
        genEventTag.Init(_eventTagId);

        // register events
        genEventTag.actOnToggleApply += OnToggleApplyTag;

        eventTags.Add(genEventTag);
    }

    private bool RemoveEventTag(int _index)
    {
        if (_index < eventTags.Count)
        {
            // destroy and remove out of mgr list
            TagPanelItem eventTag = eventTags[_index];
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

    // === Apply tag ===
    #region event
    public void OnToggleApplyTag(TagPanelItem _eventTag, bool _isActive)
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

    public void OnToggleTagMenu()
    {
        // set position window
        bool isActive = !IsActive();

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
