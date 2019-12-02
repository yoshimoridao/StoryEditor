using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataMgr : Singleton<DataMgr>
{
    public enum DataType { Alias, Element, Story, Count, Linking };

    // ===== store index info =====
    [System.Serializable]
    public class DataIndex
    {
        public string key;
        public int colorId;

        public DataIndex() { }
        public DataIndex(string _key, int _colorId)
        {
            key = _key;
            colorId = _colorId;
        }

        public void SetColorIndex(ColorBar.ColorType type)
        {
            colorId = (int)type;
        }
    }
    [System.Serializable]
    public class DataIndexer
    {
        [SerializeField]
        public List<DataIndex> dataIndexes = new List<DataIndex>();
    }

    // ===== store element info =====
    [System.Serializable]
    public class DataElement
    {
        [SerializeField]
        public List<string> lElements = new List<string>();
    }

    [System.Serializable]
    public class DataStorage
    {
        public string origin = "#stories#";

        public DataStorage() { }

        public void ChangeStory(string key)
        {
            // add prefix to detect
            key = DataConfig.prefixOutPutStory + key;
            origin = origin.Replace("#stories#", "#" + key + "#");
        }
    }

    Dictionary<string, List<string>> dicAlias = new Dictionary<string, List<string>>();
    Dictionary<string, List<string>> dicElements = new Dictionary<string, List<string>>();
    Dictionary<string, List<string>> dicStories = new Dictionary<string, List<string>>();

    //// format -> 1:2,3,4 (these key (2,3,4) link to key 1)
    //Dictionary<string, List<string>> dicLinking = new Dictionary<string, List<string>>();
    [HideInInspector]
    [SerializeField]
    DataStorage dataStorage = new DataStorage();
    [SerializeField]
    DataIndexer dataIndexer = new DataIndexer();
    bool isInitDone = false;

    // ========================================= GET/ SET =========================================
    public DataIndex GetDataIndex(string key)
    {
        int findId = dataIndexer.dataIndexes.FindIndex(x => x.key == key);
        if (findId != -1 && findId < dataIndexer.dataIndexes.Count)
            return dataIndexer.dataIndexes[findId];

        return null;
    }

    public List<string> GetDataInfo(DataType type, string key)
    {
        var dic = GetDic(type);
        if (dic.ContainsKey(key))
            return dic[key];
        return null;
    }

    private Dictionary<string, List<string>> GetDic(DataType type)
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
        switch (type)
        {
            case DataType.Alias:
                dic = dicAlias;
                break;
            case DataType.Element:
                dic = dicElements;
                break;
            case DataType.Story:
                dic = dicStories;
                break;
            //case DataType.Linking:
            //    dic = dicLinking;
            //    break;
        }
        return dic;
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
        LoadInfoData();
        LoadIndexData();
    }

    public void InitElements()
    {
        // create objs for Element Board
        List<string> keys = new List<string>(dicElements.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            string panelTitle = keys[i];
            CommonPanel panel = (CanvasMgr.Instance.GetBoard<ElementBoard>() as ElementBoard).AddPanel(panelTitle) as CommonPanel;

            if (panel)
            {
                List<string> panelVars = dicElements[panelTitle];
                for (int j = 0; j < panelVars.Count; j++)
                {
                    string var = panelVars[j];
                    if (var.Contains("#"))
                    {
                        panel.AddLinkLabel(var.Replace("#", ""));
                    }
                    else
                    {
                        panel.AddInputLabel(var);
                    }
                }
            }
        }

        // create objs for Story Board
        keys = new List<string>(dicStories.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            string panelTitle = keys[i];
            CommonPanel panel = (CanvasMgr.Instance.GetBoard<StoryBoard>() as StoryBoard).AddPanel(panelTitle) as CommonPanel;

            if (panel)
            {
                if (dicStories[panelTitle].Count == 0)
                    continue;

                string[] panelVars = dicStories[panelTitle][0].Split(' ');
                for (int j = 0; j < panelVars.Length; j++)
                {
                    string var = panelVars[j];
                    if (var.Contains("#"))
                    {
                        panel.AddLinkLabel(var.Replace("#", ""));
                    }
                    else
                    {
                        panel.AddInputLabel(var);
                    }
                }
            }
        }
        
        // save index data for all panels (at first)
        if (dataIndexer.dataIndexes.Count == 0)
            SaveAllIndexData();

        isInitDone = true;
    }

    public bool AddDataInfo(DataType type, string key, List<string> val)
    {
        // add new val in dic
        var dic = GetDic(type);
        if (!dic.ContainsKey(key))
        {
            dic.Add(key, val);
            return true;
        }
        return false;
    }

    public void SaveDataInfo(CommonPanel panel)
    {
        if (!isInitDone)
            return;

        // determine panel is in which board (element or story)
        bool isElementBoard = panel.GetBoard() is ElementBoard;
        List<Label> labels = panel.GetLabels();

        // don't save if the panel doesn't have any labels
        if (labels.Count == 0)
            return;

        // save val
        DataType type = isElementBoard ? DataType.Element : DataType.Story;

        // get key of panel
        string key = panel.GetTitleLabel().GetText();

        // get all text of label of panel
        List<string> labelVars = new List<string>();
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            string labelText = "";
            // element of Element Board
            if (isElementBoard)
            {
                if (label is LinkLabel)
                {
                    // store the connection
                    //AddLinkingVal(labelText, key);

                    labelText = "#" + label.GetText() + "#";
                }
                else
                {
                    labelText = label.GetText();
                }
            }
            // element of Story Board
            else
            {
                if (label is LinkLabel)
                {
                    // store the connection
                    //AddLinkingVal(labelText, key);

                    labelText = "#" + label.GetText() + "#";
                }
                else
                {
                    labelText = label.GetText();
                }
            }

            labelVars.Add(labelText);
        }

        if (!AddDataInfo(type, key, labelVars))
            ReplaceDataInfo(type, key, labelVars);

        // save info data
        SaveDataInfo();
        // save index data
        SaveIndexData(panel);
    }

    public bool ReplaceDataInfoKey(DataType type, string oldKey, string newKey)
    {
        // check new key available
        if (type == DataType.Element || type == DataType.Story)
        {
            if (GetDic(DataType.Element).ContainsKey(newKey) || GetDic(DataType.Story).ContainsKey(newKey))
                return false;
        }

        var dic = GetDic(type);
        if (dic.ContainsKey(oldKey))
        {
            dic.Add(newKey, new List<string>(dic[oldKey]));
            dic.Remove(oldKey);

            // also replace old key by new key in [linking dictionary]
            //ReplaceLinkingKey(type, oldKey, newKey);
            ReplaceLinkingDataInfoKey(oldKey, newKey);

            // export text file
            SaveDataInfo();

            // replace key in index data
            ReplaceIndexDataKey(oldKey, newKey);
            return true;
        }

        return false;
    }

    public bool ReplaceDataInfo(DataType type, string key, List<string> vals)
    {
        var dic = GetDic(type);
        if (dic.ContainsKey(key))
        {
            dic[key] = vals;

            // export text file
            SaveDataInfo();
            return true;
        }

        return false;
    }

    // ===== INDEX DATA =====
    public void SaveIndexData(CommonPanel panel)
    {
        string title = panel.GetTitle();
        ColorBar.ColorType colorType = panel.GetColorType();

        // replace already indexer
        int findId = dataIndexer.dataIndexes.FindIndex(x => x.key == title);
        if (findId != -1 && findId < dataIndexer.dataIndexes.Count)
        {
            dataIndexer.dataIndexes[findId].key = title;
            dataIndexer.dataIndexes[findId].colorId = (int)colorType;
        }
        // add new indexer
        else
        {
            DataIndex dataIndex = new DataIndex(title, (int)colorType);
            dataIndexer.dataIndexes.Add(dataIndex);
        }

        SaveIndexData();
    }
    // ========================================= PRIVATE FUNCS =========================================
    private void LoadInfoData()
    {
        // create file save if not exist
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataConfig.storySaveFilePath))
            File.CreateText(DataConfig.storySaveFilePath);
#endif

        // load by Player Pref
        //if (!PlayerPrefs.HasKey(DataConfig.storyDataSaveKey))
        //    return;
        //string content = PlayerPrefs.GetString(DataConfig.storyDataSaveKey);

        string content = File.ReadAllText(DataConfig.storySaveFilePath);

        Debug.Log("Load Save File = " + content);
        if (content.Length > 0)
        {
            // I.plit 1. "]," 2. "]}
            string[] splitString = { "\"],\"", "\"]}" };
            string[] result = content.Split(splitString, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < result.Length; i++)
            {
                string str = result[i];
                // origin part
                if (i == 0)
                {
                    string[] splitOrgPart = { "#\",\"" };
                    string[] rOrgPart = str.Split(splitOrgPart, System.StringSplitOptions.RemoveEmptyEntries);
                    if (rOrgPart.Length >= 1)
                        str = rOrgPart[1];
                }
                // get key of this part
                string key = str.Split('\"')[0];
                // convert to DataElement format ->  add 1. {" 2. "]} -> replace "key" = "lElements"
                str = "{\"" + str + "\"]}";
                str = str.Replace(key, "lElements");
                try
                {
                    DataElement elementObj = JsonUtility.FromJson<DataElement>(str);
                    // add to dictionary
                    if (elementObj != null)
                    {
                        // add to Story dic
                        if (key.Contains(DataConfig.prefixOutPutStory))
                        {
                            dicStories.Add(key.Replace(DataConfig.prefixOutPutStory, ""), elementObj.lElements);
                        }
                        // add to Elements dic
                        else
                        {
                            dicElements.Add(key, elementObj.lElements);
                        }
                    }
                }
                catch
                {

                }
            }
        }
    }

    private void SaveDataInfo()
    {
        DataStorage originData = new DataStorage();
        // default test case 0
        if (dicStories.Count > 0)
        {
            List<string> keys = new List<string>(dicStories.Keys);
            originData.ChangeStory(keys[0]);
        }

        // root part of out put string for data info
        string strOrigin = JsonUtility.ToJson(originData);
        if (dicStories.Count > 0)
            AddElementJson(ref strOrigin, dicStories, true);
        if (dicElements.Count > 0)
            AddElementJson(ref strOrigin, dicElements);

        Debug.Log("Save = " + strOrigin);

        // create file if not exist
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataConfig.storySaveFilePath))
            File.CreateText(DataConfig.storySaveFilePath);
        // write new content
        File.WriteAllText(DataConfig.storySaveFilePath, strOrigin);
#endif

        // save to playerpref
        PlayerPrefs.SetString(DataConfig.storyDataSaveKey, strOrigin);
    }

    private void AddElementJson(ref string strOrigin, Dictionary<string, List<string>> dic, bool isStory = false)
    {
        List<string> keys = new List<string>(dic.Keys);
        string output = "";
        for (int i = 0; i < keys.Count; i++)
        {
            // add prefix for story
            string key = keys[i];
            DataElement data = new DataElement();
            if (isStory)
            {
                // with Story: merge all vals to one string
                List<string> vars = dic[key];
                string outputVar = vars[0];
                for (int j = 1; j < vars.Count; j++)
                    outputVar += " " + vars[j];

                data.lElements = new List<string>();
                data.lElements.Add(outputVar);
            }
            else
            {
                data.lElements = new List<string>(dic[key]);
            }

            string outputKey = isStory ? DataConfig.prefixOutPutStory + key : key;
            output += JsonUtility.ToJson(data).Replace("lElements", outputKey).Replace("{", ",").Replace("}", "");
        }
        strOrigin = strOrigin.Substring(0, strOrigin.Length - 1) + output + "}";
    }

    // ==== INDEX DATA ====
    private void LoadIndexData()
    {
        // create file save if not exist
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataConfig.indexSaveFilePath))
            File.CreateText(DataConfig.indexSaveFilePath);
#endif
        // load by Player Pref
        //if (!PlayerPrefs.HasKey(DataConfig.indexDataSaveKey))
        //    return;
        //string content = PlayerPrefs.GetString(DataConfig.indexDataSaveKey);

        string content = File.ReadAllText(DataConfig.indexSaveFilePath);

        Debug.Log("Load Index = " + content);
        if (content.Length > 0)
        {
            dataIndexer = JsonUtility.FromJson<DataIndexer>(content);
        }
    }

    private void SaveAllIndexData()
    {
        List<Panel> panels = (CanvasMgr.Instance.GetBoard<ElementBoard>() as ElementBoard).GetPanels();
        panels.AddRange((CanvasMgr.Instance.GetBoard<StoryBoard>() as StoryBoard).GetPanels());
        for (int i = 0; i < panels.Count; i++)
        {
            Panel panel = panels[i];
            SaveIndexData(panel as CommonPanel);
        }
    }

    private void SaveIndexData()
    {
        string strOutput = JsonUtility.ToJson(dataIndexer);
        Debug.Log("Save Indexer = " + strOutput);

        // create file if not exist
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataConfig.indexSaveFilePath))
            File.CreateText(DataConfig.indexSaveFilePath);
        // write new content
        File.WriteAllText(DataConfig.indexSaveFilePath, strOutput);
#endif

        // save to playerpref
        PlayerPrefs.SetString(DataConfig.indexDataSaveKey, strOutput);
    }

    private void ReplaceIndexDataKey(string oldKey, string newKey)
    {
        // replace already indexer
        int findId = dataIndexer.dataIndexes.FindIndex(x => x.key == oldKey);
        if (findId != -1 && findId < dataIndexer.dataIndexes.Count)
        {
            dataIndexer.dataIndexes[findId].key = newKey;
        }

        SaveIndexData();
    }

    // ==== INDEX DATA ====
    //private void AddLinkingVal(string key, string val)
    //{
    //    if (dicLinking.ContainsKey(key))
    //    {
    //        List<string> vals = dicLinking[key];
    //        // append in case didn't have
    //        if (!vals.Contains(val))
    //        {
    //            vals.Add(val);
    //            dicLinking[key] = vals;
    //        }
    //    }
    //    else
    //    {
    //        List<string> vals = new List<string>();
    //        vals.Add(val);
    //        dicLinking.Add(key, vals);
    //    }
    //}

    //private void ReplaceLinkingKey(DataType type, string oldKey, string newKey)
    //{
    //    // replace this key & replace all #oldkey# in (alias, elements, story)
    //    if (dicLinking.ContainsKey(oldKey))
    //    {
    //        // replace key linked to another keys
    //        List<string> linkKeys = dicLinking[oldKey];
    //        for (int i = 0; i < linkKeys.Count; i++)
    //        {
    //            string key = linkKeys[i];
    //            List<string> vals = GetDataInfo(type, key);
    //            // replace new linking key (for alias, elements, story)
    //            for (int j = 0; j < vals.Count; j++)
    //            {
    //                vals[j] = vals[j].Replace("#" + oldKey + "#", "#" + newKey + "#");
    //            }
    //            // replace val in storage
    //            ReplaceDataInfo(type, key, vals);
    //        }

    //        // replace old key in another key
    //        dicLinking.Add(newKey, new List<string>(dicLinking[oldKey]));
    //        dicLinking.Remove(oldKey);
    //    }
    //}

    private void ReplaceLinkingDataInfoKey(string oldKey, string newKey)
    {
        for (int i = 0; i < 2; i++)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            if (i == 0)
                dic = dicElements;
            else
                dic = dicStories;

            List<string> keys = new List<string>(dic.Keys);
            for (int j = 0; j < keys.Count; j++)
            {
                string key = keys[j];
                List<string> vals = dic[key];
                for (int k = 0; k < vals.Count; k++)
                {
                    vals[k] = vals[k].Replace("#" + oldKey + "#", "#" + newKey + "#");
                }
                dic[key] = vals;
            }
        }
    }
}
