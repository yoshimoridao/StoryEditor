using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
            ClearAllNullVal();
        }

        public List<string> TestCases
        {
            get { return origin; }
        }

        public void AddTestCase(string key)
        {
            if (key.Length == 0)
                return;

            ClearAllNullVal();

            if (!origin.Contains(key))
                origin.Add(key);
        }

        public void RemoveTestCase(string key)
        {
            if (origin.Contains(key))
                origin.Remove(key);
        }

        public void ClearTestCases()
        {
            origin.Clear();
        }

        public string ExportTracyFile()
        {
            DataStorage clone = new DataStorage();
            clone.origin = new List<string>(origin);
            for (int i = 0; i < clone.origin.Count; i++)
            {
                clone.origin[i] = "#" + clone.origin[i] + "#";
            }
            return JsonUtility.ToJson(clone);
        }

        // [*] bug: remove all null value
        private void ClearAllNullVal()
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

    //Dictionary<string, List<string>> dicLinking = new Dictionary<string, List<string>>();
    [HideInInspector]
    [SerializeField]
    DataStorage dataStorage = new DataStorage();
    [SerializeField]
    DataIndexer dataIndexer = new DataIndexer();
    bool isInitDone = false;

    // ========================================= GET/ SET =========================================
    // === Data storage ===
    public void ClearTestCases()
    {
        dataStorage.ClearTestCases();

        // export tracery file
        ExportTraceryFile();
    }
    public void AddTestCase(string testCase)
    {
        if (testCase.Length == 0)
            return;

        dataStorage.AddTestCase(testCase);

        // export tracery file
        ExportTraceryFile();
    }
    public void RemoveTestCase(string testCase)
    {
        dataStorage.RemoveTestCase(testCase);

        // export tracery file
        ExportTraceryFile();
    }
    public List<string> GetTestCases() { return dataStorage.TestCases; }

    public List<DataIndex> GetDataStories() { return dataIndexer.stories; }
    // === Index ===
    /// <summary>
    /// get index data with unknown data type
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public DataIndex GetIndex(string key, out DataIndexer.DataType type) { return dataIndexer.GetIndex(key, out type); }

    /// <summary>
    /// get index data with specific data type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public DataIndex GetIndex(DataIndexer.DataType type, string key)
    {
        return dataIndexer.GetIndex(type, key);
    }

    public void AddIndex(CommonPanel panel)
    {
        if (!isInitDone)
            return;

        DataIndex dataIndex = new DataIndex(panel);
        DataIndexer.DataType type = dataIndex.isStoryElement ? DataIndexer.DataType.Story : DataIndexer.DataType.Element;

        if (dataIndexer.IsContain(type, dataIndex.key))
        {
            // WARNING match key

        }
        else
        {
            dataIndexer.AddIndex(dataIndex);
        }

        // export tracery file
        ExportTraceryFile();
    }

    public void RemoveIndex(DataIndexer.DataType type, string key)
    {
        if (!isInitDone)
            return;

        dataIndexer.RemoveIndex(type, key);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceIndexKey(DataIndexer.DataType type, string oldKey, string newKey)
    {
        if (!isInitDone)
            return;

        dataIndexer.ReplaceIndexKey(type, oldKey, newKey);

        // export tracery file
        ExportTraceryFile();
    }

    public void SortIndexes(DataIndexer.DataType type, List<Panel> panels)
    {
        List<string> panelKeys = new List<string>();
        foreach (Panel panel in panels)
        {
            if (panel)
                panelKeys.Add(panel.GetTitle());
        }

        dataIndexer.SortIndexes(type, panelKeys);

        // export tracery file
        ExportTraceryFile();
    }

    // === Index's Val ===
    public void AddElement(DataIndexer.DataType type, string key, Label label)
    {
        if (!isInitDone)
            return;

        dataIndexer.AddElement(type, key, label);

        // export tracery file
        ExportTraceryFile();
    }

    public void RemoveElement(DataIndexer.DataType type, string indexKey, int elementIndex)
    {
        if (!isInitDone)
            return;

        dataIndexer.RemoveElement(type, indexKey, elementIndex);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceElement(DataIndexer.DataType type, string indexKey, int elementIndex, Label label)
    {
        if (!isInitDone)
            return;

        dataIndexer.ReplaceElement(type, indexKey, elementIndex, label);

        // export tracery file
        ExportTraceryFile();
    }

    public void ReplaceElements(CommonPanel panel)
    {
        if (!isInitDone)
            return;

        DataIndexer.DataType type = panel.GetDataType();
        string indexKey = panel.GetTitle();
        dataIndexer.ReplaceElements(type, indexKey, panel);

        // export tracery file
        ExportTraceryFile();
    }

    public void SetColorIndex(DataIndexer.DataType type, string indexKey, ColorBar.ColorType colorType)
    {
        if (!isInitDone)
            return;

        dataIndexer.SetColorIndex(type, indexKey, colorType);

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

    public void CreateElements()
    {
        // create objs for Element Board
        dataIndexer.CreateElements(DataIndexer.DataType.Element);
        dataIndexer.CreateElements(DataIndexer.DataType.Story);

        isInitDone = true;
    }

    public void ExportTraceryFile()
    {
        //string output = JsonUtility.ToJson(dataStorage).Replace("}", "");
        string output = dataStorage.ExportTracyFile().Replace("}", "");

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
                strElement = strElement.Replace("elements", dataIndex.key).Replace("{", ",").Replace("}", "");     // merge string (json format)

                output += strElement;
            }
        }

        output += "}";

        Debug.Log("Export Tracery File = " + output);

        // create file if not exist
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataDefine.save_path_storyData))
            File.CreateText(DataDefine.save_path_storyData);
        // write new content
        File.WriteAllText(DataDefine.save_path_storyData, output);
#else
        // save to playerpref
        PlayerPrefs.SetString(DataConfig.save_key_storyData, output);
#endif
    }

    public string MergeAllElements(DataIndex dataIndex)
    {
        string val = "";
        if (dataIndex != null)
        {
            for (int k = 0; k < dataIndex.elements.Count; k++)
            {
                if (k != 0)
                    val += " ";
                val += dataIndex.elements[k];
            }
        }

        return val;
    }

    // ========================================= PRIVATE FUNCS =========================================
    public List<string> ParseRetrieveLinkId(string val)
    {
        List<string> tmp = new List<string>();

        var parsedVal = val.Split('#');
        // iterate odd index
        for (int i = 1; i < parsedVal.Length; i += 2)
        {
            string linkKey = parsedVal[i];
            tmp.Add(linkKey);

            //DataIndexer.DataType dataType = DataIndexer.DataType.Element;

            //// check this link key exists
            //DataIndex foundIndex = GetIndex(linkKey, out dataType);
            //if (foundIndex != null)
            //    tmp.Add(foundIndex);
        }

        return tmp;
    }
}
