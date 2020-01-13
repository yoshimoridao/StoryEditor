using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMgr : Singleton<PopupMgr>
{
    public enum PopupType { NONE, SAVE };

    [SerializeField]
    private Image overlayImg;
    [SerializeField]
    private Image bgImg;
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text content;
    private PopupType popupType = PopupType.NONE;

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
    public void Init()
    {
        Load();
    }

    public void Load()
    {
        SetActive(false);
    }

    public void ShowPopup(PopupType _popupType)
    {
        // set title & content popup
        string strTitle = "";
        string strContent = "";

        popupType = _popupType;
        if (_popupType == PopupType.SAVE)
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
            {
                // save for first time
                if (DataMgr.Instance.LastSaveFile.Length == 0)
                {
                    CanvasMgr.Instance.OpenSaveBrowserAndExit();
                }
                // if already save => override the save file
                else
                {
                    DataMgr.Instance.Save();
                    CanvasMgr.Instance.ExitApp();
                }
            }
            else
            {
                // quit
                CanvasMgr.Instance.ExitApp();
            }
        }
        else
        {

        }
    }
}
