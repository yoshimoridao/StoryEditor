using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeBarMgr : Singleton<NoticeBarMgr>
{
    [SerializeField]
    private float showFieldTime = 3;

    [SerializeField]
    private Text fileText;
    [SerializeField]
    private InputField noticeField;
    private float showFieldDt = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (showFieldDt > 0)
        {
            showFieldDt -= Time.deltaTime;

            if (showFieldDt <= 0)
            {
                showFieldDt = 0;
                ActiveNoticeField(false);
            }
        }
    }

    public void Init()
    {
        Load();

        // default file name empty
        if (fileText)
            fileText.text = "";
    }

    public void Load()
    {
        ActiveNoticeField(false);
    }

    public void UpdateFileName()
    {
        if (fileText)
        {
            string fileName = DataMgr.Instance.LastLoadFile;
            var nameSplit = fileName.Split('\\');
            if (nameSplit.Length > 0)
            {
                fileText.text = nameSplit[nameSplit.Length - 1];
            }
        }
    }

    public void ShowNotice(string _val)
    {
        if (!noticeField)
            return;

        ActiveNoticeField(true);

        noticeField.text = _val;
        showFieldDt = showFieldTime;
    }

    private void ActiveNoticeField(bool _isActive)
    {
        if (!noticeField)
            return;

        // active notice field
        noticeField.gameObject.SetActive(_isActive);
    }
}
