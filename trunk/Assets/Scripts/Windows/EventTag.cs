using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTag : MonoBehaviour
{
    // effected all elements
    public System.Action<EventTagId> actOnErase;

    // effect selected elements
    public System.Action<EventTag, bool> actOnToggleApply;

    [SerializeField]
    private Toggle applyToggle;
    
    [SerializeField]
    private InputField inputField;

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
        set
        {
            isApply = value;
            applyToggle.isOn = isApply;
        }
    }
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
        
        if (inputField)
            inputField.onEndEdit.AddListener(OnTextValueChanged);
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
