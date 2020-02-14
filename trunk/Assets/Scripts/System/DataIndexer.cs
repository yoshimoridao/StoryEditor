using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

// ===== store index info =====
[System.Serializable]
public class DataIndexer
{
    public enum DataType { Element, Story, Count };

    // --- store data ---
    public int genKey = 0;
    [SerializeField]
    public List<DataIndex> elements = new List<DataIndex>();
    [SerializeField]
    public List<DataIndex> stories = new List<DataIndex>();

    // test cases mode
    public bool isRdTest = true;
    // amount of random test cases
    public int rdTestCaseAmount = 1;
    // list test cases
    public List<string> testCaseIds = new List<string>();

    // toolbar
    public int normalFontSize = 20;

    // event tags
    public int tagGenKey = 0;
    [SerializeField]
    public List<EventTagId> eventTagIds = new List<EventTagId>();

    // --- action ---
    public Action actModifiedTestCase;
    private string lastLoadFile = "";

    // ============================================ PUBLIC ============================================
    public DataIndexer() { }

    // ====== Getter/ Setter ======
    public int RdTestCaseAmount
    {
        get { return rdTestCaseAmount; }
        set
        {
            rdTestCaseAmount = value;
        }
    }

    public bool IsRandomTest
    {
        get { return isRdTest; }
        set
        {
            isRdTest = value;
        }
    }

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
        //get
        //{
        //    if (PlayerPrefs.HasKey(DataDefine.save_key_last_load_file))
        //        return PlayerPrefs.GetString(DataDefine.save_key_last_load_file);
        //    return "";
        //}
        //set
        //{
        //    PlayerPrefs.SetString(DataDefine.save_key_last_load_file, value);
        //}

        get { return lastLoadFile; }
        set { lastLoadFile = value; }
    }

    // ====== Data ======
    public DataIndex GetData(DataType _type, string _key, bool _isFindByTitle = false)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = -1;
        // find data by title
        if (_isFindByTitle)
            findId = datas.FindIndex(x => x.title == _key);
        // find data by generated key
        else
            findId = datas.FindIndex(x => x.genKey == _key);

        if (findId != -1 && findId < datas.Count)
            return datas[findId];

        return null;
    }

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

    public void ReplaceTitle(DataType _type, string _key, string _title)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _key);
        if (findId != -1 && findId < datas.Count)
        {
            // replace key of the index
            datas[findId].Title = _title;
        }
    }

    public void SortData(DataType _type, List<string> _keys)
    {
        List<DataIndex> sortList = new List<DataIndex>();
        for (int i = 0; i < _keys.Count; i++)
        {
            string key = _keys[i];
            DataIndex index = GetData(_type, key);
            if (index != null)
                sortList.Add(index);
        }

        if (sortList.Count > 0)
        {
            if (_type == DataType.Story)
                stories = sortList;
            else if (_type == DataType.Element)
                elements = sortList;
        }
    }

    public DataIndex FindData(string _key, bool _isFindByTitle)
    {
        for (int i = 0; i < 2; i++)
        {
            DataIndex result = GetData(i == 0 ? DataType.Element : DataType.Story, _key, _isFindByTitle);
            if (result != null)
                return result;
        }

        return null;
    }
    public DataIndex FindData(string _key, bool _isFindByTitle, out DataType _dataType)
    {
        _dataType = DataType.Element;

        for (int i = 0; i < 2; i++)
        {
            _dataType = i == 0 ? DataType.Element : DataType.Story;
            DataIndex result = GetData(_dataType, _key, _isFindByTitle);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public bool IsContain(DataType _type, string _key)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _key);
        if (findId != -1 && findId < datas.Count)
            return true;

        return false;
    }

    // ====== Color ======
    public void SetColor(DataType _type, string _key, Color _color)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
            dataIndex.SetColor(_color);
    }

    // ====== Test Case ======
    public List<string> TestCases
    {
        get { return testCaseIds; }
    }

    public void AddTestCase(string _key)
    {
        if (_key.Length == 0)
            return;

        if (!testCaseIds.Contains(_key))
        {
            testCaseIds.Add(_key);
            InvokeTestCaseAct();
        }
    }

    public void RemoveTestCase(string _key)
    {
        if (testCaseIds.Contains(_key))
        {
            testCaseIds.Remove(_key);
            InvokeTestCaseAct();
        }
    }

    public void ClearTestCases()
    {
        testCaseIds.Clear();
        InvokeTestCaseAct();
    }

    private void InvokeTestCaseAct()
    {
        if (actModifiedTestCase != null)
            actModifiedTestCase();
    }

    // ====== Tag ======
    public EventTagId GetEventTag(string _genKey)
    {
        int findId = eventTagIds.FindIndex(x => x.genKey == _genKey);

        if (findId != -1)
        {
            return new EventTagId(eventTagIds[findId]);
        }

        return null;
    }

    public EventTagId AddEventTag(string _val)
    {
        // gen new key
        string newKey = "@" + tagGenKey;
        tagGenKey++;

        // gen new tag
        EventTagId genTag = new EventTagId();
        genTag.genKey = newKey;
        genTag.value = _val;

        eventTagIds.Add(genTag);

        return genTag;
    }

    public void RemoveEventTag(string _genKey)
    {
        int findId = eventTagIds.FindIndex(x => x.genKey == _genKey);

        if (findId != -1)
        {
            eventTagIds.RemoveAt(findId);
        }
    }

    public void ChangeEventTagVal(string _genKey, string _val)
    {
        int findId = eventTagIds.FindIndex(x => x.genKey == _genKey);

        if (findId != -1)
        {
            eventTagIds[findId].value = _val;
        }
    }



    // ====== Element's elements ======
    // --- elements ---
    public void AddElement(DataType _type, string _key, string _val)
    {
        DataIndex dataIndex = GetData(_type, _key);
        if (dataIndex != null)
            dataIndex.AddElement(_val);
    }

    public void ReplaceElement(DataType _type, string _key, int _eIndex, string _val)
    {
        DataIndex dataIndex = GetData(_type, _key);
        if (dataIndex != null)
            dataIndex.ReplaceElement(_eIndex, _val);
    }

    public void ReplaceElements(DataType _type, string _key, List<string> _elements)
    {
        DataIndex dataIndex = GetData(_type, _key);
        // replace list of elements of data index
        if (dataIndex != null)
            dataIndex.elements = _elements;
    }

    public void RemoveElement(DataType _type, string _key, int _eIndex)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
            dataIndex.RemoveElement(_eIndex);
    }

    // --- Test elements ---
    public void ReplaceTestElements(DataType _type, string _key, List<int> _testElements)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
        {
            dataIndex.testElements.Clear();
            dataIndex.testElements = new List<int>(_testElements);
        }
    }

    // --- Event tags elements ---
    public void ReplaceEventTagElement(DataType _type, string _key, int _eIndex, List<string> _tagIds)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
        {
            string tagIds = "";
            foreach (string tagId in _tagIds)
            {
                tagIds += tagId + ",";
            }
            // remove comma at last
            tagIds = tagIds.Substring(0, tagIds.Length - 1);

            dataIndex.ReplaceEventTagElement(_eIndex, tagIds);
        }
    }

    // ====== Util ======
    public void Load(string _path)
    {
        string content = File.ReadAllText(_path);

        // clone data
        if (content.Length > 0)
        {
            DataIndexer loadData = null;

            // old format -> convert to new format
            if (Util.IsOldSaveFormat(content))
            {
                loadData = Util.ConvertOldSaveFileToLastest(content);
            }
            else
            {
                // new format
                loadData = JsonUtility.FromJson<DataIndexer>(content);
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
                testCaseIds = loadData.testCaseIds;

                // toolbar
                normalFontSize = loadData.normalFontSize;

                // event tags
                tagGenKey = loadData.tagGenKey;
                eventTagIds = loadData.eventTagIds;
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
