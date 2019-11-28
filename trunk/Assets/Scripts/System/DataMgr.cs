using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataMgr : Singleton<DataMgr>
{
    [System.Serializable]
    public class DataStorage
    {
        [SerializeField]
        public List<string> lAlias = new List<string>();
        [SerializeField]
        public List<string> lElements = new List<string>();
        [SerializeField]
        public List<string> lStories = new List<string>();
    }

    Dictionary<string, string> dicAlias = new Dictionary<string, string>();
    Dictionary<string, string> dicElements = new Dictionary<string, string>();
    Dictionary<string, string> dicStories = new Dictionary<string, string>();
    [SerializeField]
    DataStorage dataStorage = new DataStorage();

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        instance = this;
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        instance = this;
        LoadSaveFile();
    }

    public void AddElement(string key, string val)
    {
        if (dicElements.ContainsKey(key))
        {
            // override element in dic
            dicElements[key] = val;
        }
        else
        {
            // add new element in dic
            dicElements.Add(key, val);
        }

        Debug.Log("Save Element vs key = " + key + ", val = " + val);
    }

    public void AddStory(string key, string val)
    {
        if (dicStories.ContainsKey(key))
        {
            // override story in dic
            dicStories[key] = val;
        }
        else
        {
            // add new story in dic
            dicStories.Add(key, val);
        }

        Debug.Log("Save Story vs key = " + key + ", val = " + val);
    }

    public void SaveData(CommonPanel panel)
    {
        // determine panel is in which board (element or story)
        bool isElementBoard = panel.GetBoard() is ElementBoard;

        List<Label> labels = panel.GetLabels();
        // get key
        string key = panel.GetTitleLabel().GetText();

        string val = "";
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            // element of Element Board
            if (isElementBoard)
            {
                if (i > 0)
                    val += ",";
                if (label is LinkLabel)
                {
                    val += "#" + label.GetText() + "#";
                }
                else
                {
                    val += label.GetText();
                }
            }
            // element of Story Board
            else
            {
                if (i > 0)
                    val += " ";
                if (label is LinkLabel)
                {
                    val += "#" + label.GetText() + "#";
                }
                else
                {
                    val += label.GetText();
                }
            }
        }

        if (isElementBoard)
            AddElement(key, val);
        else
            AddStory(key, val);

        ExportSaveFile();
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void ExportSaveFile()
    {
        dataStorage.lAlias = new List<string>();
        foreach (var val in dicAlias)
        {
            dataStorage.lAlias.Add(val.Key + ":" + val.Value);
        }

        dataStorage.lElements = new List<string>();
        foreach (var val in dicElements)
        {
            dataStorage.lElements.Add(val.Key + ":" + val.Value);
        }

        dataStorage.lStories = new List<string>();
        foreach (var valPair in dicStories)
        {
            dataStorage.lStories.Add(valPair.Key + ":" + valPair.Value);
        }

        string content = JsonUtility.ToJson(dataStorage);
        Debug.Log("Save = " + content);
        
        // create file if not exist
        if (!File.Exists(DataConfig.saveFilePath))
            File.CreateText(DataConfig.saveFilePath);
        // write new content
        File.WriteAllText(DataConfig.saveFilePath, content);
        
    }

    private void LoadSaveFile()
    {
        // create file save if not exist
        if (!File.Exists(DataConfig.saveFilePath))
            File.CreateText(DataConfig.saveFilePath);

        string content = File.ReadAllText(DataConfig.saveFilePath);
        Debug.Log("Load Save File = " + content);
        dataStorage = JsonUtility.FromJson<DataStorage>(content);
    }
}
