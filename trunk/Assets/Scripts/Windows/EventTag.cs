using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventTag : MonoBehaviour
{
    public System.Action<EventTagId> actOnErase;
    public System.Action<EventTagId> actOnValChange;

    public System.Action<EventTag> actOnToggleAply;
    public System.Action<EventTag> actOnToggleVisible;

    [SerializeField]
    private Button eraseBtn;
    [SerializeField]
    private InputField inputField;
    private EventTagId tagId;

    [SerializeField]
    private Toggle visibleToggle;
    [SerializeField]
    private bool isVisible;

    [SerializeField]
    private Toggle applyToggle;
    [SerializeField]
    private bool isApply;

    public EventTagId TagId
    {
        get { return tagId; }
        set { tagId = value; }
    }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {

    }

    void Start()
    {
        // register events
        if (visibleToggle)
        {
            isVisible = visibleToggle.isOn;
            visibleToggle.onValueChanged.AddListener(OnToggleVisible);
        }
        if (applyToggle)
        {
            isApply = applyToggle.isOn;
            applyToggle.onValueChanged.AddListener(OnToggleApply);
        }

        if (eraseBtn)
        {
            eraseBtn.onClick.AddListener(OnEraseBtnPress);
        }
        if (inputField)
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
        }
    }

    void Update()
    {
    }

    public void Init(EventTagId _tagId)
    {
        tagId = _tagId;
        inputField.text = tagId.value;
    }
    // ========================================= PUBLIC FUNCS =========================================
    public void OnEraseBtnPress()
    {
        if (actOnErase != null && tagId != null)
            actOnErase.Invoke(tagId);
    }

    public void OnValueChanged(string _val)
    {
        if (tagId == null)
            return;

        tagId.value = _val;
        if (actOnValChange != null)
            actOnValChange.Invoke(tagId);
    }

    // --- visible ---
    public void IsVisible(bool _isVisible)
    {
        isVisible = _isVisible;
        visibleToggle.isOn = isVisible;
    }

    public void OnToggleVisible(bool _isActive)
    {
        isVisible = _isActive;

        if (actOnToggleVisible != null)
            actOnToggleVisible.Invoke(this);
    }

    // --- apply ---
    public void IsApplyTag(bool _isApply)
    {
        isApply = _isApply;
        applyToggle.isOn = isApply;
    }

    public void OnToggleApply(bool _isActive)
    {
        isApply = _isActive;

        if (actOnToggleAply != null)
            actOnToggleAply.Invoke(this);
    }
}
