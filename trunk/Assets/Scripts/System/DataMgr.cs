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

    DataStorage dataStorage = new DataStorage();
    DataIndexer dataIndexer = new DataIndexer();

    // ========================================= GET/ SET =========================================
    public List<DataIndex> Stories { get { return dataIndexer.stories; } }
    public List<DataIndex> Elements { get { return dataIndexer.elements; } }

    // ================== Index ==================
    public string GenNewKey()
    {
        dataIndexer.genKey++;
        return dataIndexer.genKey.ToString();
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
        ExportTraceryFile();
    }

    public void RemoveData(DataIndexer.DataType _type, string _key)
    {
        dataIndexer.RemoveData(_type, _key);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceTitle(DataIndexer.DataType _type, string _key, string _title)
    {
        dataIndexer.ReplaceTitle(_type, _key, _title);

        // export tracery file
        ExportTraceryFile();
    }

    public void SortIndexes(DataIndexer.DataType _type, List<Panel> _panels)
    {
        List<string> panelKeys = new List<string>();
        foreach (Panel panel in _panels)
        {
            if (panel)
                panelKeys.Add(panel.Key);
        }

        dataIndexer.SortData(_type, panelKeys);

        // export tracery file
        ExportTraceryFile();
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

    // ================== Element ==================
    public void AddElement(DataIndexer.DataType _type, string _key, string _val)
    {
        dataIndexer.AddElement(_type, _key, _val);

        // export tracery file
        ExportTraceryFile();
    }

    public void RemoveElement(DataIndexer.DataType _type, string _key, int _eIndex)
    {
        dataIndexer.RemoveElement(_type, _key, _eIndex);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceElement(DataIndexer.DataType _type, string _key, int _eIndex, string _val)
    {
        dataIndexer.ReplaceElement(_type, _key, _eIndex, _val);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceElements(DataIndexer.DataType _type, string _key, List<string> _elements)
    {
        dataIndexer.ReplaceElements(_type, _key, _elements);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceTestingIndex(DataIndexer.DataType _type, string _key, List<int> _testElements)
    {
        dataIndexer.ReplaceTestElements(_type, _key, _testElements);
    }

    public void SetColorIndexData(DataIndexer.DataType _type, string _Key, ColorBar.ColorType _colorType)
    {
        dataIndexer.SetColor(_type, _Key, _colorType);
    }

    public void SetTestPanel(DataIndexer.DataType _type, string _key, bool _isTest)
    {
        if (_isTest)
            dataIndexer.AddTestCase(_key);
        else
            dataIndexer.RemoveTestCase(_key);

        // export tracery file
        ExportTraceryFile();
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
        // load data indexer first
        dataIndexer.Load();
    }

    public void ExportTraceryFile()
    {
        //string output = dataStorage.ExportTracyFile().Replace("}", "");
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

        // Parse pre-fix origin
        // get title all of test case
        //List<string> testCaseIds = new List<string>(TestCaseIds);
        //for (int i = 0; i < testCaseIds.Count; i++)
        //{
        //    DataIndex tmpData = FindData(testCaseIds[i], false);
        //    if (tmpData != null)
        //        testCaseIds[i] = tmpData.Title;
        //}
        // export test cases for origin part
        output = dataStorage.ExportTracyFile(TestCases).Replace("}", "") + output;

        // Replace all of keys to title of refer obj
        output = ReplaceTitleOfHashKey(output);

        Debug.Log("Export Tracery File = " + output);

        // --- Save ---
#if (IN_UNITY_EDITOR)
        // save to data folder (for only on editor)
        if (!Directory.Exists(DataDefine.save_path_dataFolder))
            Directory.CreateDirectory(DataDefine.save_path_dataFolder);
        // write new content
        File.WriteAllText(DataDefine.save_path_dataFolder + DataDefine.save_fileName_storyData, output);
#else
        // save to playerpref (for back up)
        PlayerPrefs.SetString(DataDefine.save_key_storyData, output);
#endif

        // save to out-side folder (save to folder where user can reach)
        if (!Directory.Exists(DataDefine.save_path_outputFolder))
            Directory.CreateDirectory(DataDefine.save_path_outputFolder);

        File.WriteAllText(DataDefine.save_path_outputFolder + DataDefine.save_fileName_storyData, output);
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

    public void Save()
    {
        // save and export tracery file
        dataIndexer.Save();
        ExportTraceryFile();
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
