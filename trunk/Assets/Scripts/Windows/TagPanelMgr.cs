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

    [SerializeField] private Animator tagAnimator;
    [SerializeField] private Toggle visibleAllToggle;
    // tag objects've generated
    private List<TagPanelItem> tagItems = new List<TagPanelItem>();

    // these label're selected
    private List<Label> selectLabels = new List<Label>();
    private bool isInit;

    // test mode
    [SerializeField] private Transform testModeMenu;
    [SerializeField] private Toggle selectTestBtn;
    [SerializeField] private Toggle tagTestBtn;
    [SerializeField] private Toggle groupTestBtn;
    [SerializeField] private Toggle grammarTestBtn;

    private bool isVisibleAllByChild = false;

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
        if (visibleAllToggle)
            visibleAllToggle.onValueChanged.AddListener(OnToggleVisibleAll);
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
        // default toggle visible all = true
        visibleAllToggle.isOn = true;

        Load();
    }

    public void Load()
    {
        // default turn off window
        SetActiveWindow(false);

        // load tag ids
        List<EventTagId> tagIds = DataMgr.Instance.GetEventTags();

        // destroy template and get add btn
        int turn = Mathf.Max(tagItems.Count, tagIds.Count);
        for (int i = 0; i < tagIds.Count; i++)
        {
            EventTagId tagId = tagIds[i];
            // generate new tag
            if (i >= tagItems.Count)
            {
                GenNewEventTag(tagId);
            }
            // re-use existed tag
            else
            {
                tagItems[i].Init(tagId);
            }
        }

        // remove surplus tags
        if (tagIds.Count < tagItems.Count)
        {
            for (int i = tagIds.Count; i < tagItems.Count; i++)
            {
                // if remove event tag success
                if (RemoveEventTag(i))
                    i--;
            }
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    #region util
    private void GenNewEventTag(EventTagId _eventTagId)
    {
        if (_eventTagId == null)
            return;

        // gen new tag object
        TagPanelItem genEventTag = Instantiate(prefTagItem.gameObject, contTrans).GetComponent<TagPanelItem>();
        genEventTag.Init(_eventTagId);

        // register events
        genEventTag.actOnToggleApply += OnToggleApplyTag;
        genEventTag.VisibleToggle.onValueChanged.AddListener(OnTagItemToggleVisible);

        tagItems.Add(genEventTag);
    }

    private bool RemoveEventTag(int _index)
    {
        if (_index < tagItems.Count)
        {
            // destroy and remove out of mgr list
            TagPanelItem eventTag = tagItems[_index];
            // un-register action
            if (eventTag.actOnErase != null)
                eventTag.actOnErase -= OnEraseBtnPress;
            if (eventTag.actOnToggleApply != null)
                eventTag.actOnToggleApply -= OnToggleApplyTag;

            // destroy obj && remove in list obj
            Destroy(eventTag.gameObject);
            tagItems.RemoveAt(_index);

            return true;
        }
        return false;
    }

    public void AnimateIn()
    {
        // set position window
        if (!IsActive()) ;
        SetActiveWindow(true);

        tagAnimator.Play("In");
    }

    public void AnimateOut()
    {
        tagAnimator.Play("Out");
    }
    #endregion

    // ================= Event =================
    #region event
    public void OnEraseBtnPress(EventTagId _tagId)
    {
        // remove out of mgr list
        int findId = tagItems.FindIndex(x => x.TagId == _tagId);
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
                for (int i = 0; i < tagItems.Count; i++)
                {
                    TagPanelItem tmpEventTag = tagItems[i];
                    if (tmpEventTag.TagId == null)
                        continue;

                    tmpEventTag.IsApplyTag = elementDataIndex.IsContainEventTag(tmpEventTag.TagId.genKey);
                }
            }
        }
    }
    public void OnToggleApplyTag(TagPanelItem _eventTag, bool _isActive)
    {
        int findId = tagItems.FindIndex(x => x.gameObject == _eventTag.gameObject);

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

    public void OnTagItemToggleVisible(bool _isOn)
    {
        if (!visibleAllToggle.isOn && _isOn)
        {
            // mark the flag
            isVisibleAllByChild = true;
            visibleAllToggle.isOn = true;
        }
    }
    private void OnToggleVisibleAll(bool _isOn)
    {
        // revert the flag
        if (isVisibleAllByChild)
        {
            isVisibleAllByChild = false;
        }
        else
        {
            foreach (var item in tagItems)
            {
                if (item.IsVisible != _isOn)
                    item.ToggleVisible(_isOn);
            }
        }
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
