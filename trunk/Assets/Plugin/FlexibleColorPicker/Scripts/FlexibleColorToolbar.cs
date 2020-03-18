using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleColorToolbar : MonoBehaviour
{
    public FlexibleColorPicker fcp;

    public Vector2 btnHeightRange = new Vector2(22, 28);

    public enum PickColorType { Standard, Custom};
    public PickColorType pickColorType = PickColorType.Standard;

    // windows
    public Transform transStandard;
    public Transform transCustom;
    // buttons
    public Button closeBtn;
    public Button standardBtn;
    public Button customBtn;
    // default color
    public Color standardColor = Color.white;

    void Start()
    {
        // register button event
        if (standardBtn)
            standardBtn.onClick.AddListener(OnStandardBtnPress);
        if (customBtn)
            customBtn.onClick.AddListener(OnCustomBtnPress);
        // register color button's event
        if (transStandard)
        {
            var standardBtns = transStandard.GetComponentsInChildren<Button>();
            foreach (var standardBtn in standardBtns)
                standardBtn.onClick.AddListener(() => OnStandardColorPress(standardBtn));
        }
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        HighlightPresBtn();
    }

    private void OnDestroy()
    {
        // un-register button event
        if (standardBtn)
            standardBtn.onClick.RemoveListener(OnStandardBtnPress);
        if (customBtn)
            customBtn.onClick.RemoveListener(OnCustomBtnPress);
    }

    // =================================== PUBLIC ===================================
    public void OnStandardBtnPress()
    {
        pickColorType = PickColorType.Standard;

        HighlightPresBtn();
    }

    public void OnCustomBtnPress()
    {
        pickColorType = PickColorType.Custom;

        HighlightPresBtn();
    }

    public void OnStandardColorPress(Button _btn)
    {
        if (_btn.GetComponent<Image>())
            standardColor = _btn.GetComponent<Image>().color;
    }

    // =================================== PRIVATE ===================================
    private void HighlightPresBtn()
    {
        RectTransform btnRt = null;
        switch (pickColorType)
        {
            case PickColorType.Custom:
                btnRt = customBtn.transform as RectTransform;
                break;
            case PickColorType.Standard:
                btnRt = standardBtn.transform as RectTransform;
                break;
        }

        Vector2 btnSize = (customBtn.transform as RectTransform).sizeDelta;
        btnSize.y = pickColorType == PickColorType.Custom ? btnHeightRange.y : btnHeightRange.x;
        (customBtn.transform as RectTransform).sizeDelta = btnSize;

        btnSize = (standardBtn.transform as RectTransform).sizeDelta;
        btnSize.y = pickColorType == PickColorType.Standard ? btnHeightRange.y : btnHeightRange.x;
        (standardBtn.transform as RectTransform).sizeDelta = btnSize;

        // active window
        transStandard.gameObject.SetActive(pickColorType == PickColorType.Standard);
        transCustom.gameObject.SetActive(pickColorType == PickColorType.Custom);

        // set color of fcp equal by first color of selected panel
        if (pickColorType == PickColorType.Custom && fcp)
            fcp.color = fcp.color;

        if (GameMgr.Instance)
            GameMgr.Instance.RefreshCanvas();
    }
}
