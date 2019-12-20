﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    // ============================================ PUBLIC ============================================
    public DataIndexer() { }

    // ====== data ======
    public DataIndex GetData(DataType _type, string _key)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _key);
        if (findId != -1 && findId < datas.Count)
            return datas[findId];

        return null;
    }

    public void AddData(DataType _type, DataIndex _data)
    {
        List<DataIndex> datas = GetDatas(_type);
        datas.Add(_data);

        // save
        Save();
    }

    public void RemoveData(DataType _type, string _key)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _key);
        if (findId != -1 && findId < datas.Count)
        {
            // remove the index
            datas.RemoveAt(findId);
        }

        // save
        Save();
    }

    public void ReplaceTitle(DataType _type, string _oldTitle, string _newTitle)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _oldTitle);
        if (findId != -1 && findId < datas.Count)
        {
            // replace key of the index
            datas[findId].genKey = _newTitle;
        }

        // export save file
        Save();
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

        // export save file
        Save();
    }

    public bool IsContain(DataType _type, string _key)
    {
        List<DataIndex> datas = GetDatas(_type);

        int findId = datas.FindIndex(x => x.genKey == _key);
        if (findId != -1 && findId < datas.Count)
            return true;

        return false;
    }

    // ====== element ======
    public void AddElement(DataType _type, string _key, string _val)
    {
        DataIndex dataIndex = GetData(_type, _key);
        if (dataIndex != null)
            dataIndex.AddElement(_val);

        // export save file
        Save();
    }

    public void ReplaceElement(DataType _type, string _key, int _eIndex, string _val)
    {
        DataIndex dataIndex = GetData(_type, _key);
        if (dataIndex != null)
            dataIndex.ReplaceElement(_eIndex, _val);

        // export save file
        Save();
    }

    public void ReplaceElements(DataType _type, string _key, List<string> _elements)
    {
        DataIndex dataIndex = GetData(_type, _key);
        // replace list of elements of data index
        if (dataIndex != null)
            dataIndex.elements = _elements;

        // export save file
        Save();
    }

    public void RemoveElement(DataType _type, string _key, int _eIndex)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
            dataIndex.RemoveElement(_eIndex);

        // export save file
        Save();
    }

    public void SetColor(DataType _type, string _key, ColorBar.ColorType _color)
    {
        DataIndex dataIndex = GetData(_type, _key);

        if (dataIndex != null)
            dataIndex.colorId = (int)_color;

        // export save file
        Save();
    }

    //public void ReplaceTestingIndex(DataType type, string indexKey, List<int> testingIndex)
    //{
    //    DataIndex dataIndex = GetData(type, indexKey);

    //    if (dataIndex != null)
    //    {
    //        dataIndex.testingIndex.Clear();
    //        dataIndex.testingIndex = testingIndex;
    //    }

    //    // export save file
    //    Save();
    //}

    // ====== util ======
    public void Load()
    {
        // load in editor
#if (IN_UNITY_EDITOR)
        if (!Directory.Exists(DataDefine.save_path_dataFolder))
            Directory.CreateDirectory(DataDefine.save_path_dataFolder);

        string content = "";
        if (File.Exists(DataDefine.save_path_dataFolder + DataDefine.save_fileName_indexData))
            content = File.ReadAllText(DataDefine.save_path_dataFolder + DataDefine.save_fileName_indexData);
#else
        // load from out-side folder (the folder where user can reach)
        if (!Directory.Exists(DataDefine.save_path_outputFolder))
            Directory.CreateDirectory(DataDefine.save_path_outputFolder);

        string content = "";
        if (File.Exists(DataDefine.save_path_outputFolder + DataDefine.save_fileName_indexData))
            content = File.ReadAllText(DataDefine.save_path_outputFolder + DataDefine.save_fileName_indexData);
        // load again from by Player Pref (backup)
        if (content.Length == 0 && PlayerPrefs.HasKey(DataDefine.save_key_indexData))
            content = PlayerPrefs.GetString(DataDefine.save_key_indexData); 
#endif

        Debug.Log("Load Index = " + content);
        if (content.Length > 0)
        {
            DataIndexer newData = JsonUtility.FromJson<DataIndexer>(content);
            elements = newData.elements;
            stories = newData.stories;
        }
    }

    public void Save()
    {
        string strOutput = JsonUtility.ToJson(this);
        Debug.Log("Save Indexer = " + strOutput);

#if (IN_UNITY_EDITOR)
        // save to data folder (for only on editor)
        if (!Directory.Exists(DataDefine.save_path_dataFolder))
            Directory.CreateDirectory(DataDefine.save_path_dataFolder);

        // write new content
        File.WriteAllText(DataDefine.save_path_dataFolder + DataDefine.save_fileName_indexData, strOutput);
#else
        // save to playerpref
        PlayerPrefs.SetString(DataDefine.save_key_indexData, strOutput);
#endif

        // save to out-side folder (save to folder where user can reach)
        if (!Directory.Exists(DataDefine.save_path_outputFolder))
            Directory.CreateDirectory(DataDefine.save_path_outputFolder);

        File.WriteAllText(DataDefine.save_path_outputFolder + DataDefine.save_fileName_indexData, strOutput);
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
