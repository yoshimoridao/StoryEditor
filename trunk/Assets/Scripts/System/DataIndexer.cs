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

    // action
    public Action actModifiedTestCase;

    // ============================================ PUBLIC ============================================
    public DataIndexer() { }

    // ====== system ======
    public int RdTestCaseAmount
    {
        get { return rdTestCaseAmount; }
        set
        {
            rdTestCaseAmount = value;

            //// save
            //Save();
        }
    }

    public bool IsRandomTest
    {
        get { return isRdTest; }
        set
        {
            isRdTest = value;

            //// save
            //Save();
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

    public string LastSaveFile
    {
        get
        {
            if (PlayerPrefs.HasKey(DataDefine.save_key_last_save_file))
                return PlayerPrefs.GetString(DataDefine.save_key_last_save_file);
            return "";
        }
        set
        {
            PlayerPrefs.SetString(DataDefine.save_key_last_save_file, value);
        }
    }

    // ====== data ======
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

        //// save
        //Save();
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

        //// save
        //Save();
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

        //// export save file
        //Save();
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

        //// export save file
        //Save();
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

    public void SetColor(DataType _type, string _key, ColorBar.ColorType _color)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
            dataIndex.Color = (int)_color;

        //// export save file
        //Save();
    }

    // ====== element ======
    public void AddElement(DataType _type, string _key, string _val)
    {
        DataIndex dataIndex = GetData(_type, _key);
        if (dataIndex != null)
            dataIndex.AddElement(_val);

        //// export save file
        //Save();
    }

    public void ReplaceElement(DataType _type, string _key, int _eIndex, string _val)
    {
        DataIndex dataIndex = GetData(_type, _key);
        if (dataIndex != null)
            dataIndex.ReplaceElement(_eIndex, _val);

        //// export save file
        //Save();
    }

    public void ReplaceElements(DataType _type, string _key, List<string> _elements)
    {
        DataIndex dataIndex = GetData(_type, _key);
        // replace list of elements of data index
        if (dataIndex != null)
            dataIndex.elements = _elements;

        //// export save file
        //Save();
    }

    public void RemoveElement(DataType _type, string _key, int _eIndex)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
            dataIndex.RemoveElement(_eIndex);

        //// export save file
        //Save();
    }

    public void ReplaceTestElements(DataType _type, string _key, List<int> _testElements)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
        {
            dataIndex.testElements.Clear();
            dataIndex.testElements = new List<int>(_testElements);
        }

        //// export save file
        //Save();
    }

    // ====== pick test case ======
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

        //// save
        //Save();
    }

    public void RemoveTestCase(string _key)
    {
        if (testCaseIds.Contains(_key))
        {
            testCaseIds.Remove(_key);
            InvokeTestCaseAct();
        }

        //// save
        //Save();
    }

    public void ClearTestCases()
    {
        testCaseIds.Clear();
        InvokeTestCaseAct();

        //// save
        //Save();
    }

    // ====== util ======
    public void Load(string _path, out bool _isConvertOldSave)
    {
        _isConvertOldSave = false;

        string content = File.ReadAllText(_path);

        // clone data
        if (content.Length > 0)
        {
            DataIndexer loadData = null;

            // old format -> convert to new format
            if (Util.IsOldSaveFormat(content))
            {
                loadData = Util.ConvertOldSaveFileToLastest(content);

                // clone data
                if (loadData != null)
                {
                    elements = loadData.elements;
                    stories = loadData.stories;
                    _isConvertOldSave = true;

                    // replace all link titles to link keys
                    loadData = Util.ReplaceLinkTitlesToLinkKeys(loadData);
                }
            }
            // new format
            else
            {
                loadData = JsonUtility.FromJson<DataIndexer>(content);

                // clone data
                if (loadData != null)
                {
                    // just clone key from lastest save format
                    if (!_isConvertOldSave)
                        genKey = loadData.genKey;

                    elements = loadData.elements;
                    stories = loadData.stories;

                    isRdTest = loadData.isRdTest;
                    rdTestCaseAmount = loadData.rdTestCaseAmount;
                    testCaseIds = loadData.testCaseIds;

                    // toolbar
                    normalFontSize = loadData.normalFontSize;
                }
            }
        }
    }

    public void Save()
    {
        string strOutput = JsonUtility.ToJson(this);

        if (File.Exists(LastSaveFile))
        {
            File.WriteAllText(LastSaveFile, strOutput);
        }
        else
        {
            StreamWriter writer = new StreamWriter(LastSaveFile);
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

    private void InvokeTestCaseAct()
    {
        if (actModifiedTestCase != null)
            actModifiedTestCase();
    }
}
