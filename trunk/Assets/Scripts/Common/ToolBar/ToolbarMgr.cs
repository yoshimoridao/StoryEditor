using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarMgr : Singleton<ToolbarMgr>
{
    public FontSizeButton fontSizeButton;
    public Dropdown languageDropdown;

    // drop down
    [SerializeField]
    private List<Localization.Language> lanCodeIds = new List<Localization.Language>();
    [SerializeField]
    private int curLanguageId = -1;
    // path without Lan Code
    private string lastPathWithoutLan = "";
    private string nextLocFilePath = "";

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
        fontSizeButton.Init();

        if (languageDropdown)
        {
            languageDropdown.ClearOptions();

            // set default language for drop down (is English)
            List<string> defaultVal = new List<string>();
            defaultVal.Add(Localization.GetFullLanguageCode(Localization.Language.En));
            languageDropdown.AddOptions(defaultVal);

            lanCodeIds.Add(Localization.Language.En);
            curLanguageId = 0;
        }
    }

    public void Load()
    {
        fontSizeButton.Init();

        RefreshDropDownVals();
    }

    public void OnChangeLanguageVal()
    {
        if (languageDropdown.value != curLanguageId)
        {
            curLanguageId = languageDropdown.value;

            if (lastPathWithoutLan.Length == 0 || curLanguageId >= lanCodeIds.Count)
                return;

            string loadPath = lastPathWithoutLan + "_" + lanCodeIds[curLanguageId].ToString() + ".txt";
            if (File.Exists(loadPath))
            {
                // save loaded file
                //DataMgr.Instance.LastLoadFile = loadPath;
                nextLocFilePath = loadPath;

                PopupMgr.Instance.ShowPopup(PopupMgr.PopupType.CHANGELANGUAGE, LoadNextLocFile);
            }
        }
    }

    public void LoadNextLocFile()
    {
        if (nextLocFilePath.Length > 0)
        {
            DataMgr.Instance.Load(nextLocFilePath);
            nextLocFilePath = "";
        }
    }

    private void RefreshDropDownVals()
    {
        if (languageDropdown && File.Exists(DataMgr.Instance.LastLoadFile))
        {
            var splitVal = DataMgr.Instance.LastLoadFile.Split('\\');
            if (splitVal.Length == 0)
                return;

            string loadFile = splitVal[splitVal.Length - 1];
            string loadPath = DataMgr.Instance.LastLoadFile.Replace(loadFile, "");      // loaded path

            var tmpVals = loadFile.Split('_');

            List<Localization.Language> tmpLans = new List<Localization.Language>();  // list contain options of drop down menu
            int tmpLanId = -1;

            if (tmpVals.Length > 1)
            {
                // remove "_<lancode>.txt" part of file path
                string curLancode = tmpVals[tmpVals.Length - 1].Replace(".txt", "");    // current language code
                loadFile = tmpVals[tmpVals.Length - 2];             // loaded file (already remove lan code)

                // store path without language code
                lastPathWithoutLan = loadPath + loadFile;

                for (int i = 0; i < (int)Localization.Language.COUNT; i++)
                {
                    string tmpLanCode = ((Localization.Language)i).ToString();
                    string tmpFile = loadPath + (loadFile + "_" + tmpLanCode + ".txt");
                    if (File.Exists(tmpFile))
                    {
                        if (tmpLanCode == curLancode)
                            tmpLanId = tmpLans.Count;
                        tmpLans.Add((Localization.Language)i);
                    }
                }
            }

            // default with english language
            if (tmpLans.Count == 0)
            {
                tmpLans.Add(Localization.Language.En);
                tmpLanId = 0;
                lastPathWithoutLan = "";
            }

            // store option && cur option
            lanCodeIds = new List<Localization.Language>(tmpLans);
            curLanguageId = tmpLanId;

            // add options for drop down menu
            languageDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var lancode in lanCodeIds)
                options.Add(Localization.GetFullLanguageCode(lancode));
            languageDropdown.AddOptions(options);

            languageDropdown.value = curLanguageId;
        }
    }
}
