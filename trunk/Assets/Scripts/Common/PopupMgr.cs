using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMgr : Singleton<PopupMgr>
{
    public enum PopupType { NONE, SAVE, CHANGELANGUAGE };

    [SerializeField]
    private Image overlayImg;
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text content;
    private PopupType popupType = PopupType.NONE;

    private bool isActive;
    private Action actCallback;

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

    // ========================================== PUBLIC ==========================================
    public bool IsActive()
    {
        return isActive;
    }

    public void Init()
    {
        Load();
    }

    public void Load()
    {
        SetActive(false);
    }

    public void ShowPopup(PopupType _popupType, Action _actCallback = null)
    {
        actCallback = _actCallback;

        // set title & content popup
        string strTitle = "";
        string strContent = "";

        popupType = _popupType;
        if (_popupType == PopupType.SAVE || _popupType == PopupType.CHANGELANGUAGE)
        {
            strTitle = DataDefine.popup_title_save;
            strContent = DataDefine.popup_content_save;
        }

        // show popup
        SetActive(true);
        if (title && strTitle.Length > 0)
            title.text = strTitle;
        if (content && strContent.Length > 0)
            content.text = strContent;
    }

    public void OnYesButtonPress()
    {
        SetActive(false);

        // do response of popup
        DoAction(true);
    }

    public void OnNoButtonPress()
    {
        SetActive(false);

        // do response of popup
        DoAction(false);
    }

    // ========================================== PRIVATE ==========================================
    private void SetActive(bool _isActive)
    {
        isActive = _isActive;

        if (overlayImg)
            overlayImg.gameObject.SetActive(_isActive);
        if (bgImg)
            bgImg.gameObject.SetActive(_isActive);
    }

    private void DoAction(bool _isConfirm)
    {
        if (popupType == PopupType.SAVE)
        {
            if (_isConfirm)
                GameMgr.Instance.OpenSaveBrowserAndExit();
            else
                GameMgr.Instance.ExitApp();   // quit
        }
        else if (popupType == PopupType.CHANGELANGUAGE)
        {
            if (_isConfirm)
            {
                //// auto override current file
                //if (!DataMgr.Instance.OverrideSaveFile())
                //{
                //    // or asking where to save file if save fail
                //    GameMgr.Instance.OpenSaveBrowser(actCallback);
                //}
            }
            else
            {
                //DataMgr.Instance.LoadLastFile();
            }

            if (actCallback != null)
            {
                actCallback.Invoke();
                actCallback = null;
            }
        }
    }
}
