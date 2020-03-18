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

        public string ExportTracyFile()
        {
            DataStorage clone = new DataStorage();
            clone.origin = origin;

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
    private string GenNewKey()
    {
        string newKey = "@" + dataIndexer.genKey;
        dataIndexer.genKey++;
        return newKey;
    }

    // ===== Properties =====
    #region properties
    public bool IsRandomTest
    {
        get { return dataIndexer.isRdTest; }
        set { dataIndexer.isRdTest = value; }
    }

    public int RdTestCaseAmount
    {
        get { return dataIndexer.rdTestCaseAmount; }
        set
        {
            if (value < 1)
                value = 1;
            dataIndexer.rdTestCaseAmount = value;
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

    public bool IsActiveSelectTest
    {
        get { return dataIndexer.isActiveSelectTest; }
        set { dataIndexer.isActiveSelectTest = value; }
    }
    public bool IsActiveTagTest
    {
        get { return dataIndexer.isActiveTagTest; }
        set { dataIndexer.isActiveTagTest = value; }
    }
    public bool IsActiveGroupTest
    {
        get { return dataIndexer.isActiveGroupTest; }
        set { dataIndexer.isActiveGroupTest = value; }
    }
    public bool IsActiveGrammarTest
    {
        get { return dataIndexer.isActiveGrammarTest; }
        set { dataIndexer.isActiveGrammarTest = value; }
    }
    #endregion

    // ====== Data Indexer ======
    public DataIndex GetData(DataIndexer.DataType _type, string _key) { return dataIndexer.GetData(_type, _key); }

    public DataIndex AddData(DataIndexer.DataType _type)
    {
        DataIndex genData = new DataIndex();
        genData.genKey = GenNewKey();
        dataIndexer.AddData(_type, genData);

        return genData;
    }

    public void RemoveData(DataIndexer.DataType _type, string _key)
    {
        dataIndexer.RemoveData(_type, _key);
    }

    public void SortIndexes(DataIndexer.DataType _type, List<Panel> _panels)
    {
        List<DataIndex> tmpDataIndexes = new List<DataIndex>();

        foreach (Panel panel in _panels)
        {
            DataIndex dataIndex = panel.GetDataIndex();

            if (dataIndex != null)
                tmpDataIndexes.Add(dataIndex);
        }

        dataIndexer.SortData(_type, tmpDataIndexes);
    }

    public DataIndex FindData(string _key, bool _isFindByTitle, out DataIndexer.DataType _dataType) { return dataIndexer.FindData(_key, _isFindByTitle, out _dataType); }

    // ===== Test Case =====
    public List<string> GetTestingDataVals()
    {
        List<string> dataVals = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            List<DataIndex> datas = i == 0 ? dataIndexer.elements : dataIndexer.stories;
            foreach (DataIndex dataElement in datas)
            {
                if (dataElement.isTest)
                    dataVals.Add(dataElement.genKey);
            }
        }

        return dataVals;
    }

    // ===== TAG =====
    #region tag
    public List<DataTagFlow> GetFlowsContainTag(string _tag) { return dataIndexer.dataEventTag.tagRelation.GetFlowsContainTag(_tag); }
    public List<DataTagGroup> GetGroupsOfTag(string _tag) { return dataIndexer.dataEventTag.tagRelation.GetGroupsOfTag(_tag); }
    public DataTagRelation GetDataTagRelation() { return dataIndexer.dataEventTag.tagRelation; }
    public List<EventTagId> GetTestingEventTags() { return new List<EventTagId>(dataIndexer.dataEventTag.GetTestingTags()); }
    public bool IsTestingTag(string _genKey) { return dataIndexer.dataEventTag.IsTestingTag(_genKey); }

    public List<EventTagId> GetEventTags() { return new List<EventTagId>(dataIndexer.dataEventTag.eventTagIds); }
    public EventTagId GetEventTag(string _genKey) { return dataIndexer.dataEventTag.GetEventTag(_genKey); }
    public EventTagId AddEventTag(string _val) { return dataIndexer.dataEventTag.AddEventTag(_val); }
    public void RemoveEventTag(string _genKey) { dataIndexer.dataEventTag.RemoveEventTag(_genKey); }
    #endregion

    // ================== Element ==================
    // ===== Tag =====

    // ========================================= UNITY FUNCS =========================================
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
                    // clone element's value
                    dataElement.elements = new List<string>();
                    foreach (var tmp in dataIndex.elements)
                        dataElement.elements.Add(tmp.value);
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
        output = dataStorage.ExportTracyFile().Replace("}", "") + output;

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
                    // clone element's value
                    tmpElement.elements = new List<string>();
                    foreach (var tmp in dataIndex.elements)
                        tmpElement.elements.Add(tmp.value);
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
            foreach (var element in _dataIndex.elements)
                val += element.value;
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
            GameMgr.Instance.Load();

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
            DataIndexer.DataType findDataType;
            DataIndex eData = FindData(eKey, false, out findDataType);
            if (eData != null)
                result = result.Replace("#" + eData.genKey + "#", "#" + eData.title + "#");
        }
        return result;
    }
}
