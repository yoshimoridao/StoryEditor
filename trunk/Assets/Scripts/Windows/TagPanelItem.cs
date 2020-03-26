using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TagPanelItem : MonoBehaviour
{
    // effected all elements
    public System.Action<EventTagId> actOnErase;

    // effect selected elements
    public System.Action<TagPanelItem, bool> actOnToggleApply;

    [SerializeField]
    private Toggle applyToggle;

    [SerializeField]
    private TextMeshProUGUI inputField;

    [SerializeField]
    private Toggle visibleToggle;
    [SerializeField]
    private Toggle testToggle;
    [SerializeField]
    private Button eraseBtn;

    private EventTagId tagId;
    private bool isApply;

    // ========================================= PROPERTIES =========================================
    public EventTagId TagId
    {
        get { return tagId; }
        set { tagId = value; }
    }

    // --- apply ---
    public bool IsApplyTag
    {
        get { return isApply; }
        set
        {
            isApply = value;
            applyToggle.isOn = isApply;
        }
    }

    public Toggle VisibleToggle { get { return visibleToggle; } }
    public bool IsVisible { get { return visibleToggle.isOn; } }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {

    }

    void Start()
    {
        // register events
        if (applyToggle)
        {
            isApply = applyToggle.isOn;
            applyToggle.onValueChanged.AddListener(OnToggleApply);
        }

        if (visibleToggle)
            visibleToggle.onValueChanged.AddListener(OnToggleVisible);
        if (testToggle)
            testToggle.onValueChanged.AddListener(OnToggleTest);
        if (eraseBtn)
            eraseBtn.onClick.AddListener(OnEraseBtnPress);
    }

    void Update()
    {
    }

    public void Init(EventTagId _tagId)
    {
        tagId = _tagId;

        if (inputField)
            inputField.text = tagId.value;
        if (visibleToggle)
            visibleToggle.isOn = tagId.IsVisible;
        if (testToggle)
            testToggle.isOn = tagId.isTest;
    }
    // ========================================= PUBLIC FUNCS =========================================
    // --- apply selected btn ---
    public void OnToggleApply(bool _val)
    {
        isApply = _val;

        // call back event
        if (actOnToggleApply != null && tagId != null)
            actOnToggleApply.Invoke(this, isApply);
    }

    public void ToggleVisible(bool _val)
    {
        visibleToggle.isOn = _val;
    }

    // --- apply all btns ---
    public void OnTextValueChanged(string _val)
    {
        if (tagId == null)
            return;

        tagId.Value = _val;
    }

    public void OnToggleTest(bool _val)
    {
        tagId.isTest = _val;
    }

    public void OnToggleVisible(bool _val)
    {
        tagId.IsVisible = _val;
    }

    public void OnEraseBtnPress()
    {
        if (actOnErase != null && tagId != null)
            actOnErase.Invoke(tagId);
    }
}
