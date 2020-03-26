using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UI.ModernUIPack;

public class ToolbarMgr : Singleton<ToolbarMgr>
{
    public FontSizeButton fontSizeButton;
    public UICustomDropdown languageDropdown;

    private Localization.LanguageCode curLanguage = Localization.LanguageCode.EN;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
    }

    void Update()
    {

    }

    // ==================================== PUBLIC ====================================
    public void Init()
    {
        //fontSizeButton.Init();

        //if (languageDropdown)
        //{
        //    // clear all items of dropdown menu
        //    languageDropdown.ClearAllItems();

        //    //// create default english language
        //    //languageDropdown.SetItemTitle(Localization.GetLanguage(curLanguage));
        //    //languageDropdown.CreateNewItem();
        //    //languageDropdown.SetupDropdown();

        //    // register event for drop down
        //    languageDropdown.dropdownEvent.AddListener(OnChangeLanguageVal);
        //}

        Load();
    }

    public void Load()
    {
        fontSizeButton.Init();

        RefreshDropDownVals();
    }

    private void RefreshDropDownVals()
    {
        if (languageDropdown)
        {
            int endLanCode = (DataMgr.Instance.IsLocAvailable) ? (int)Localization.LanguageCode.COUNT : (int)(Localization.LanguageCode.EN + 1);
            List<string> lanItems = new List<string>();
            for (int i = 0; i < endLanCode; i++)
            {
                string lanItem = Localization.GetLanguage(i);
                if (lanItem.Length > 0)
                    lanItems.Add(lanItem);
            }

            // clear all items of drop down
            languageDropdown.ClearAllItems();
            foreach (string item in lanItems)
            {
                languageDropdown.SetItemTitle(item);
                languageDropdown.CreateNewItem();
            }
            languageDropdown.SetupDropdown();
        }
    }

    // === Button Event ===
    public void OnChangeLanguageVal(int _index)
    {
        if (_index != (int)curLanguage)
        {
            curLanguage = (Localization.LanguageCode)_index;


        }
    }
    // === Button Event ===
    public void OnChangeTagEditorBtnPress()
    {
        GameMgr.Instance.OpenEditor(GameMgr.EditorType.TagEditor);
    }
}
