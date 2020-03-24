using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

// ===== store index info =====
[System.Serializable]
public class DataIndexer
{
    public string toolVersion = "v003";
    public enum DataType { Element, Story, Count };

    // --- store data ---
    public int genKey = 0;
    [SerializeField]
    public List<DataIndex> elements = new List<DataIndex>();
    [SerializeField]
    public List<DataIndex> stories = new List<DataIndex>();
    // event tags
    public DataEventTag dataEventTag = new DataEventTag();

    // test cases mode
    public bool isRdTest = true;
    // amount of random test cases
    public int rdTestCaseAmount = 1;
    // toolbar
    public int normalFontSize = 20;
    // default active all test mode
    public bool isActiveSelectTest = true;
    public bool isActiveTagTest = true;
    public bool isActiveGroupTest = true;
    public bool isActiveGrammarTest = true;

    // --- action ---
    private string lastLoadFile = "";

    // ============================================ PUBLIC ============================================
    public DataIndexer() { }

    // ====== Getter/ Setter ======
    public string LastLoadPath
    {
        get
        {
            if (PlayerPrefs.HasKey(DataDefine.save_key_last_path))
                return PlayerPrefs.GetString(DataDefine.save_key_last_path);
            return "";
        }
        set
        {
            PlayerPrefs.SetString(DataDefine.save_key_last_path, value);
        }
    }

    public string LastLoadFile
    {
        get { return lastLoadFile; }
        set { lastLoadFile = value; }
    }

    // ====== Data ======
    public void AddData(DataType _type, DataIndex _data)
    {
        List<DataIndex> datas = GetDatas(_type);
        datas.Add(_data);
    }

    public void RemoveData(DataType _type, string _key)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _key);
        if (findId != -1 && findId < datas.Count)
        {
            // remove the index
            datas[findId].OnDestroy();

            datas.RemoveAt(findId);
        }
    }

    public DataIndex GetData(DataType _type, string _key, bool _isFindByTitle = false)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = -1;
        // find data by title
        if (_isFindByTitle)
        {
            findId = datas.FindIndex(x => x.title.ToLower() == _key.ToLower());
        }
        // find data by generated key
        else
        {
            findId = datas.FindIndex(x => x.genKey == _key);
        }

        if (findId != -1 && findId < datas.Count)
            return datas[findId];

        return null;
    }

    public void SortData(DataType _type, List<DataIndex> _sortedDatas)
    {
        if (_type == DataType.Story)
            stories = _sortedDatas;
        else if (_type == DataType.Element)
            elements = _sortedDatas;
    }

    public DataIndex FindData(string _key, bool _isFindByTitle, out DataType _dataType)
    {
        _dataType = DataType.Element;

        for (int i = 0; i < 2; i++)
        {
            _dataType = i == 0 ? DataType.Element : DataType.Story;
            DataIndex result = GetData(_dataType, _key, _isFindByTitle);
            if (result != null)
                return result;
        }

        return null;
    }

    // ====== Test Case ======

    // ====== Tag ======

    // ====== Util ======
    public void Load(string _path)
    {
        string content = File.ReadAllText(_path);

        // clone data
        if (content.Length > 0)
        {
            DataIndexer loadData = null;

            if (content.Contains("toolVersion"))
            {
                // new format
                loadData = JsonUtility.FromJson<DataIndexer>(content);
            }
            else
            {
                // old format -> convert to new format
                loadData = Util.ConvertOldSaveFileToLastest(content);
            }

            // ***** CLONE data *****
            if (loadData != null)
            {
                // just clone key from lastest save format
                genKey = loadData.genKey;

                elements = loadData.elements;
                stories = loadData.stories;

                isRdTest = loadData.isRdTest;
                rdTestCaseAmount = loadData.rdTestCaseAmount;
                //testCaseIds = loadData.testCaseIds;

                // toolbar
                normalFontSize = loadData.normalFontSize;
                // test mode
                isActiveSelectTest = loadData.isActiveSelectTest;
                isActiveTagTest = loadData.isActiveTagTest;
                isActiveGroupTest = loadData.isActiveGroupTest;
                isActiveGrammarTest = loadData.isActiveGrammarTest;

                // event tags
                if (loadData.dataEventTag != null)
                    dataEventTag = loadData.dataEventTag;
            }
        }
    }

    public void Save(string _path)
    {
        string strOutput = JsonUtility.ToJson(this);

        if (File.Exists(_path))
        {
            File.WriteAllText(_path, strOutput);
        }
        else
        {
            StreamWriter writer = new StreamWriter(_path);
            writer.Write(strOutput);
            writer.Close();
        }
    }

    public List<DataIndex> GetDatas(DataType _type)
    {
        List<DataIndex> datas = new List<DataIndex>();
        switch (_type)
        {
            case DataType.Element:
                datas = elements;
                break;
            case DataType.Story:
                datas = stories;
                break;
            default:
                break;
        }
        return datas;
    }
}
