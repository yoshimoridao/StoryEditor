using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[System.Serializable]
public class DataMgr : Singleton<DataMgr>
{
    // ===== store element info =====
    [System.Serializable]
    public class ElementExportGame
    {
        public string title;
        [SerializeField]
        public List<string> elements = new List<string>();
    }

    [System.Serializable]
    public class DataExportGame
    {
        [SerializeField]
        public List<ElementExportGame> elements = new List<ElementExportGame>();
    }

    [System.Serializable]
    public class DataElement
    {
        [SerializeField]
        public List<string> elements = new List<string>();

        public DataElement() { }

        public void RemoveEmptyElements()
        {
            elements.RemoveAll(x => x.Length == 0);
        }
    }

    [System.Serializable]
    public class DataStorage
    {
        public List<string> origin = new List<string>();

        public DataStorage()
        {
            ClearNullVals();
        }

        public string ExportTracyFile(List<string> _testCases)
        {
            DataStorage clone = new DataStorage();
            // get picked test case || origin list
            clone.origin = new List<string>(_testCases.Count > 0 ? _testCases : origin);
            for (int i = 0; i < clone.origin.Count; i++)
            {
                clone.origin[i] = "#" + clone.origin[i] + "#";
            }
            return JsonUtility.ToJson(clone);
        }

        // [*] bug: remove all null value
        private void ClearNullVals()
        {
            for (int i = 0; i < origin.Count; i++)
            {
                if (origin[i].Length == 0)
                {
                    origin.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private DataStorage dataStorage = new DataStorage();
    private DataIndexer dataIndexer = new DataIndexer();
    private bool isModified = true;
    [SerializeField]
    private bool isExportGameSave = false;

    // ========================================= GET/ SET =========================================
    public List<DataIndex> Stories { get { return dataIndexer.stories; } }
    public List<DataIndex> Elements { get { return dataIndexer.elements; } }
    public bool IsModified
    {
        get { return isModified; }
    }

    public bool IsExportGameSave
    {
        set { isExportGameSave = value; }
    }

    // ================== Index ==================
    public string GenNewKey()
    {
        string newKey = "@" + dataIndexer.genKey;
        dataIndexer.genKey++;
        return newKey;
    }

    // ===== Properties =====
    public bool IsRandomTest
    {
        get { return dataIndexer.IsRandomTest; }
        set { dataIndexer.IsRandomTest = value; }
    }

    public int RdTestCaseAmount
    {
        get { return dataIndexer.RdTestCaseAmount; }
        set
        {
            if (value < 1)
                value = 1;
            dataIndexer.RdTestCaseAmount = value;
        }
    }

    public int NormalFontSize
    {
        get { return dataIndexer.normalFontSize; }
        set { dataIndexer.normalFontSize = value; }
    }

    public string LastLoadPath
    {
        get { return dataIndexer.LastLoadPath; }
        set { dataIndexer.LastLoadPath = value; }
    }

    public string LastLoadFile
    {
        get { return dataIndexer.LastLoadFile; }
        set { dataIndexer.LastLoadFile = value; }
    }

    // ====== Data Indexer ======
    public DataIndex GetData(DataIndexer.DataType _type, string _key) { return dataIndexer.GetData(_type, _key); }

    public void AddData(DataIndexer.DataType _type, Panel _panel)
    {
        DataIndex dataIndex = new DataIndex(_panel);
        if (dataIndexer.IsContain(_type, dataIndex.genKey))
        {
            // WARNING match key

        }
        else
        {
            dataIndexer.AddData(_type, dataIndex);
        }

        // export tracery file
        // ExportTraceryFile();
    }

    public void RemoveData(DataIndexer.DataType _type, string _key)
    {
        dataIndexer.RemoveData(_type, _key);

        // export tracery file
        // ExportTraceryFile();
    }

    public void ReplaceTitle(DataIndexer.DataType _type, string _key, string _title)
    {
        dataIndexer.ReplaceTitle(_type, _key, _title);

        // export tracery file
        // ExportTraceryFile();
    }

    public void SortIndexes(DataIndexer.DataType _type, List<Panel> _panels)
    {
        List<string> panelKeys = new List<string>();
        foreach (Panel panel in _panels)
        {
            if (panel)
                panelKeys.Add(panel.Genkey);
        }

        dataIndexer.SortData(_type, panelKeys);

        // export tracery file
        // ExportTraceryFile();
    }

    public DataIndex FindData(string _key, bool _isFindByTitle) { return dataIndexer.FindData(_key, _isFindByTitle); }
    public DataIndex FindData(string _key, bool _isFindByTitle, out DataIndexer.DataType _dataType) { return dataIndexer.FindData(_key, _isFindByTitle, out _dataType); }

    // ===== Test Case =====
    public Action ActModifiedTestCase
    {
        get { return dataIndexer.actModifiedTestCase; }
        set { dataIndexer.actModifiedTestCase += value; }
    }
    public List<string> TestCases
    {
        get { return dataIndexer.TestCases; }
    }

    public void ClearTestCases() { dataIndexer.ClearTestCases(); }

    // ===== TAG =====
    public List<EventTagId> GetEventTags()
    {
        return new List<EventTagId>(dataIndexer.eventTagIds);
    }

    public EventTagId GetEventTag(string _genKey)
    {
        return dataIndexer.GetEventTag(_genKey);
    }

    public EventTagId AddEventTag(string _val)
    {
        return dataIndexer.AddEventTag(_val);
    }

    public void RemoveEventTag(string _genKey)
    {
        dataIndexer.RemoveEventTag(_genKey);
    }

    public void ChangeEventTagVal(string _genKey, string _val)
    {
        dataIndexer.ChangeEventTagVal(_genKey, _val);
    }

    // ================== Element ==================
    public void AddElement(DataIndexer.DataType _type, string _key, string _val)
    {
        dataIndexer.AddElement(_type, _key, _val);

        // export tracery file
        // ExportTraceryFile();
    }

    public void RemoveElement(DataIndexer.DataType _type, string _key, int _eIndex)
    {
        dataIndexer.RemoveElement(_type, _key, _eIndex);

        // export tracery file
        // ExportTraceryFile();
    }

    public void ReplaceElement(DataIndexer.DataType _type, string _key, int _eIndex, string _val)
    {
        dataIndexer.ReplaceElement(_type, _key, _eIndex, _val);

        // export tracery file
        // ExportTraceryFile();
    }

    public void ReplaceElements(DataIndexer.DataType _type, string _key, List<string> _elements)
    {
        dataIndexer.ReplaceElements(_type, _key, _elements);

        // export tracery file
        // ExportTraceryFile();
    }

    public void ReplaceTestingIndex(DataIndexer.DataType _type, string _key, List<int> _testElements)
    {
        dataIndexer.ReplaceTestElements(_type, _key, _testElements);
    }

    public void SetColorIndexData(DataIndexer.DataType _type, string _Key, Color _color)
    {
        dataIndexer.SetColor(_type, _Key, _color);
    }

    public void SetTestPanel(DataIndexer.DataType _type, string _key, bool _isTest)
    {
        if (_isTest)
            dataIndexer.AddTestCase(_key);
        else
            dataIndexer.RemoveTestCase(_key);

        // export tracery file
        // ExportTraceryFile();
    }

    // ===== Tag =====
    public void ReplaceEventTagElement(DataIndexer.DataType _type, string _key, int _eIndex, List<string> _tagIds)
    {
        dataIndexer.ReplaceEventTagElement(_type, _key, _eIndex, _tagIds);
    }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //PlayerPrefs.DeleteAll();
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
    }

    public void ExportTraceryFile(string _path)
    {
        string output = "";
        // clear all dataStorage
        dataStorage.origin.Clear();

        for (int i = 0; i < 2; i++)
        {
            DataIndexer.DataType dataType = i == 0 ? DataIndexer.DataType.Story : DataIndexer.DataType.Element;
            List<DataIndex> dataIndexes = dataIndexer.GetDatas(dataType);
            for (int j = 0; j < dataIndexes.Count; j++)
            {
                DataIndex dataIndex = dataIndexes[j];

                DataElement dataElement = new DataElement();
                // parse for story element
                if (dataType == DataIndexer.DataType.Story)
                {
                    dataElement.elements = new List<string>();
                    dataElement.elements.Add(MergeAllElements(dataIndex));
                }
                else
                {
                    // clone elements
                    dataElement.elements = new List<string>(dataIndex.elements);
                }

                // add null to output is [""] -> fix bug read element wrong
                if (dataElement.elements.Count == 0)
                    dataElement.elements.Add("");

                string strElement = JsonUtility.ToJson(dataElement);

                strElement = strElement.Replace("elements", dataIndex.title).Replace("{", ",").Replace("}", "");     // merge string (json format)

                output += strElement;

                // add element to origin
                if (dataType == DataIndexer.DataType.Story)
                    dataStorage.origin.Add(dataIndex.title);
            }
        }

        output += "}";

        // export test cases for origin part
        output = dataStorage.ExportTracyFile(TestCases).Replace("}", "") + output;

        // Replace all of keys to title of refer obj
        output = ReplaceTitleOfHashKey(output);

        Debug.Log("Export Tracery File = " + output);

        // --- Save ---
        string savePath = _path.Replace(".txt", DataDefine.save_filename_suffix_tracery + ".txt");
        if (File.Exists(savePath))
        {
            File.WriteAllText(savePath, output);
        }
        else
        {
            StreamWriter writer = new StreamWriter(savePath);
            writer.Write(output);
            writer.Close();
        }
    }

    public void ExportForGameFile(string _path)
    {
        if (!isExportGameSave)
            return;

        DataExportGame dataExportGame = new DataExportGame();

        for (int i = 0; i < 2; i++)
        {
            DataIndexer.DataType dataType = i == 0 ? DataIndexer.DataType.Story : DataIndexer.DataType.Element;
            List<DataIndex> dataIndexes = dataIndexer.GetDatas(dataType);
            for (int j = 0; j < dataIndexes.Count; j++)
            {
                DataIndex dataIndex = dataIndexes[j];

                ElementExportGame tmpElement = new ElementExportGame();
                tmpElement.title = (dataType == DataIndexer.DataType.Story ? "@Page_" + j + "@" : "") + dataIndex.title;
                
                // parse elements
                if (dataType == DataIndexer.DataType.Story)
                {
                    tmpElement.elements = new List<string>();
                    tmpElement.elements.Add(MergeAllElements(dataIndex));
                }
                else
                {
                    // clone elements
                    tmpElement.elements = new List<string>(dataIndex.elements);
                }

                // add null to output is [""] -> fix bug read element wrong
                if (tmpElement.elements.Count == 0)
                    tmpElement.elements.Add("");

                if (tmpElement != null)
                    dataExportGame.elements.Add(tmpElement);

            }
        }

        string output = JsonUtility.ToJson(dataExportGame);

        // Replace all of keys to title of refer obj
        output = ReplaceTitleOfHashKey(output);

        // --- Save ---
        string savePath = _path.Replace(".txt", DataDefine.save_filename_suffix_game + ".txt");
        if (File.Exists(savePath))
        {
            File.WriteAllText(savePath, output);
        }
        else
        {
            StreamWriter writer = new StreamWriter(savePath);
            writer.Write(output);
            writer.Close();
        }
    }

    public string MergeAllElements(DataIndex _dataIndex)
    {
        string val = "";
        if (_dataIndex != null)
        {
            foreach (string element in _dataIndex.elements)
                val += element;
        }

        return val;
    }

    public void LoadLastFile()
    {
        if (LastLoadFile.Length > 0)
            Load(LastLoadFile);
    }

    public void Load(string _path)
    {
        if (File.Exists(_path))
        {
            // save loaded file
            LastLoadFile = _path;

            dataIndexer.Load(_path);
            // re-load canvas's elements
            CanvasMgr.Instance.Load();

            // show notice text && file name
            NoticeBarMgr.Instance.UpdateFileName();
            NoticeBarMgr.Instance.ShowNotice(DataDefine.notice_load_done);
        }
    }

    public bool SaveLastFile()
    {
        if (LastLoadFile.Length > 0 && File.Exists(LastLoadFile))
        {
            Save(LastLoadFile);
            return true;
        }
        return false;
    }

    public void Save(string _path)
    {
        // save and export tracery file
        dataIndexer.Save(_path);
        ExportTraceryFile(_path);
        ExportForGameFile(_path);

        // show notice text
        NoticeBarMgr.Instance.ShowNotice(DataDefine.notice_save_done);
    }

    // ========================================= PRIVATE FUNCS =========================================
    private string ReplaceTitleOfHashKey(string _val)
    {
        string result = _val;
        char[] splitter = { '#' };
        string[] eKeys = result.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        foreach (string eKey in eKeys)
        {
            DataIndex eData = FindData(eKey, false);
            if (eData != null)
                result = result.Replace("#" + eData.genKey + "#", "#" + eData.title + "#");
        }
        return result;
    }
}
