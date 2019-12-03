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

        public DataIndexer() { }

        public DataIndex GetIndex(string key)
        {
            int findId = dataIndexes.FindIndex(x => x.key == key);
            if (findId != -1 && findId < dataIndexes.Count)
                return dataIndexes[findId];

            return null;
        }

        public void AddIndex(DataIndex data)
        {
            dataIndexes.Add(data);

            // export save file
            Save();
        }

        public void ReplaceIndex(DataIndex data)
        {
            int findId = dataIndexes.FindIndex(x => x.key == data.key);
            if (findId != -1 && findId < dataIndexes.Count)
                dataIndexes[findId] = data;

            // export save file
            Save();
        }

        public void ReplaceIndexKey(string oldKey, string newKey)
        {
            // replace already indexer
            int findId = dataIndexes.FindIndex(x => x.key == oldKey);
            if (findId != -1 && findId < dataIndexes.Count)
            {
                dataIndexes[findId].key = newKey;
            }

            // export save file
            Save();
        }

        public void RemoveIndex(string key)
        {
            int findId = dataIndexes.FindIndex(x => x.key == key);
            if (findId != -1 && findId < dataIndexes.Count)
            {
                dataIndexes.RemoveAt(findId);
            }

            // export save file
            Save();
        }

        public void Reset()
        {
            dataIndexes = new List<DataIndex>();

            // export save file
            Save();
        }

        public bool IsContain(string key)
        {
            int findId = dataIndexes.FindIndex(x => x.key == key);
            if (findId != -1 && findId < dataIndexes.Count)
            {
                return true;
            }
            return false;
        }

        public void Load()
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
                DataIndexer newData = JsonUtility.FromJson<DataIndexer>(content);
                dataIndexes = newData.dataIndexes;
            }
        }

        public void Save()
        {
            string strOutput = JsonUtility.ToJson(this);
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
        return dataIndexer.GetIndex(key);
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

        dataIndexer.Load();
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
            SaveIndexData();

        isInitDone = true;
    }

    public void SaveDataInfo(CommonPanel panel)
    {
        if (!isInitDone)
            return;

        // determine panel is in which board (element or story)
        List<Label> labels = panel.GetLabels();
        if (labels.Count == 0)
            return;

        // save val
        DataType type = panel.GetBoard() is ElementBoard ? DataType.Element : DataType.Story;

        // get all text of label of panel
        List<string> vars = new List<string>();
        for (int i = 0; i < labels.Count; i++)
        {
            Label label = labels[i];
            string var = "";
            // element of Element Board
            if (type == DataType.Element)
            {
                if (label is LinkLabel)
                {
                    // store the connection
                    //AddLinkingVal(labelText, key);

                    var = "#" + label.GetText() + "#";
                }
                else
                {
                    var = label.GetText();
                }
            }
            // element of Story Board
            else
            {
                if (label is LinkLabel)
                {
                    // store the connection
                    //AddLinkingVal(labelText, key);

                    var = "#" + label.GetText() + "#";
                }
                else
                {
                    var = label.GetText();
                }
            }

            vars.Add(var);
        }

        // get key of panel
        string key = panel.GetTitle();
        if (IsContainDataInfo(type, key))
            AddDataInfo(type, key, vars);
        else
            ReplaceDataInfo(type, key, vars);

        // save index data
        SaveIndexData(panel);
    }

    public bool IsContainDataInfo(DataType type, string key)
    {
        // add new val in dic
        var dic = GetDic(type);
        if (!dic.ContainsKey(key))
            return true;

        return false;
    }

    public void AddDataInfo(DataType type, string key, List<string> vals)
    {
        // add new val in dic
        var dic = GetDic(type);
        dic.Add(key, vals);

        // save info data
        SaveInfoData();
    }

    public bool RemoveDataInfo(DataType type, string key)
    {
        // add new val in dic
        var dic = GetDic(type);
        if (dic.ContainsKey(key))
        {
            dic.Remove(key);

            // also removing data index
            dataIndexer.RemoveIndex(key);
            // remove this key in another linking
            RemoveLinkingDataInfo(key);

            // save info data
            SaveInfoData();

            return true;
        }

        return false;
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
            ReplaceLinkingDataInfo(oldKey, newKey);

            // export text file
            SaveInfoData();

            // replace key in index data
            dataIndexer.ReplaceIndexKey(oldKey, newKey);
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
            SaveInfoData();

            return true;
        }

        return false;
    }

    // ===== INDEX DATA =====
    public void SaveIndexData(Panel panel)
    {
        string key = panel.GetTitle();
        ColorBar.ColorType colorType = panel.GetColorType();

        DataIndex newData = new DataIndex(key, (int)colorType);

        // replace already have data
        if (dataIndexer.IsContain(key))
            dataIndexer.ReplaceIndex(newData);
        // add new data
        else
            dataIndexer.AddIndex(newData);
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
                // ignore origin part
                if (i == 0)
                {
                    string[] splitOrgPart = { "#\",\"" };
                    string[] orgParts = str.Split(splitOrgPart, System.StringSplitOptions.RemoveEmptyEntries);
                    if (orgParts.Length >= 1)
                        str = orgParts[1];
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
                        if (key.Contains(DataConfig.prefixOutPutStory) && elementObj.lElements.Count > 0)
                        {
                            // parse from first element to another
                            List<string> stories = new List<string>(elementObj.lElements[0].Split(' '));
                            dicStories.Add(key.Replace(DataConfig.prefixOutPutStory, ""), stories);
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

    private void SaveInfoData()
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
                List<string> vars = dic[key];
                // with Story: merge all vals to one string
                if (vars.Count > 0)
                {
                    string outputVar = "";
                    outputVar = vars[0];
                    for (int j = 1; j < vars.Count; j++)
                        outputVar += " " + vars[j];
                    data.lElements = new List<string>();
                    data.lElements.Add(outputVar);
                }
            }
            else
            {
                data.lElements = new List<string>(dic[key]);
            }

            // add null to output is [""] -> fix bug read element wrong
            if (data.lElements.Count == 0)
                data.lElements.Add("");

            string outputKey = isStory ? DataConfig.prefixOutPutStory + key : key;
            output += JsonUtility.ToJson(data).Replace("lElements", outputKey).Replace("{", ",").Replace("}", "");
        }
        strOrigin = strOrigin.Substring(0, strOrigin.Length - 1) + output + "}";
    }

    // ==== INDEX DATA ====
    private void SaveIndexData()
    {
        // clear indexer
        dataIndexer.Reset();

        List<Panel> panels = (CanvasMgr.Instance.GetBoard<ElementBoard>() as ElementBoard).GetPanels();
        panels.AddRange((CanvasMgr.Instance.GetBoard<StoryBoard>() as StoryBoard).GetPanels());

        for (int i = 0; i < panels.Count; i++)
        {
            Panel panel = panels[i];
            SaveIndexData(panel as CommonPanel);
        }
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

    private void ReplaceLinkingDataInfo(string oldKey, string newKey)
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

    private void RemoveLinkingDataInfo(string key)
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
                string tmpKey = keys[j];
                List<string> vals = dic[tmpKey];
                for (int k = 0; k < vals.Count; k++)
                {
                    vals[k] = vals[k].Replace("#" + key + "#", "");
                    if (vals[k].Length == 0)
                    {
                        vals.RemoveAt(k);
                        k--;
                    }
                }
                dic[tmpKey] = vals;
            }
        }
    }
}
