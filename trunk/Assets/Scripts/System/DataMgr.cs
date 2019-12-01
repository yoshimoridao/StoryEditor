using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataMgr : Singleton<DataMgr>
{
    public enum DataType { Alias, Element, Story, Count, Linking };

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

    // format -> 1:2,3,4 (these key (2,3,4) link to key 1)
    Dictionary<string, List<string>> dicLinking = new Dictionary<string, List<string>>();
    [SerializeField]
    DataStorage dataStorage = new DataStorage();

    // ========================================= GET/ SET =========================================
    public List<string> GetVal(DataType type, string key)
    {
        var dic = GetDic(type);
        if (dic.ContainsKey(key))
            return dic[key];
        return null;
    }

    public bool AddVal(DataType type, string key, List<string> val)
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

    public bool ReplaceKey(DataType type, string oldKey, string newKey)
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
            ReplaceLinkingKey(oldKey, newKey);
            // export text file
            Export();
            return true;
        }

        return false;
    }

    public bool ReplaceVal(DataType type, string key, List<string> vals)
    {
        var dic = GetDic(type);
        if (dic.ContainsKey(key))
        {
            dic[key] = vals;

            // export text file
            Export();
            return true;
        }

        return false;
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
            case DataType.Linking:
                dic = dicLinking;
                break;
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
    }

    void Update()
    {
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        Load();
    }

    public void InitElement()
    {
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
                        //panel.AddLinkLabel();
                        Debug.Log("link");
                    }
                    else
                    {
                        panel.AddInputLabel(var);
                    }
                }
            }
        }
    }

    public void Save(CommonPanel panel)
    {
        // determine panel is in which board (element or story)
        bool isElementBoard = panel.GetBoard() is ElementBoard;
        List<Label> labels = panel.GetLabels();
        // save val
        DataType type = isElementBoard ? DataType.Element : DataType.Story;

        // get key of panel
        string key = panel.GetTitleLabel().GetText();
        key = (type == DataType.Story) ? DataConfig.prefixOutPutStory + key : key;

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
                    labelText = "#" + label.GetText() + "#";
                    // store the connection
                    AddLinkingVal(labelText, key);
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
                    labelText = "#" + label.GetText() + "#";
                    // store the connection
                    AddLinkingVal(labelText, key);
                }
                else
                {
                    labelText = label.GetText();
                }
            }

            labelVars.Add(labelText);
        }

        if (!AddVal(type, key, labelVars))
            ReplaceVal(type, key, labelVars);
        // export file text
        Export();
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void Load()
    {
        // create file save if not exist
        if (!File.Exists(DataConfig.saveFilePath))
            File.CreateText(DataConfig.saveFilePath);

        string content = File.ReadAllText(DataConfig.saveFilePath);
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
                DataElement elementObj = JsonUtility.FromJson<DataElement>(str);
                // add to dictionary
                if (elementObj != null)
                {
                    // add to Story dic
                    if (key.Contains(DataConfig.prefixOutPutStory))
                    {
                        dicStories.Add(key, elementObj.lElements);
                    }
                    // add to Elements dic
                    else
                    {
                        dicElements.Add(key, elementObj.lElements);
                    }
                }
            }
        }
    }

    private void Export()
    {
        DataStorage originData = new DataStorage();
        // default test case 0
        if (dicStories.Count > 0)
        {
            List<string> keys = new List<string>(dicStories.Keys);
            originData.ChangeStory(keys[0]);
        }

        string strOrigin = JsonUtility.ToJson(originData);
        if (dicStories.Count > 0)
            AddElementJson(ref strOrigin, dicStories, true);
        if (dicElements.Count > 0)
            AddElementJson(ref strOrigin, dicElements);

        Debug.Log("Save = " + strOrigin);

        // create file if not exist
        if (!File.Exists(DataConfig.saveFilePath))
            File.CreateText(DataConfig.saveFilePath);
        // write new content
        File.WriteAllText(DataConfig.saveFilePath, strOrigin);
    }

    private void AddElementJson(ref string strJson, Dictionary<string, List<string>> dic, bool isStory = false)
    {
        List<string> keys = new List<string>(dic.Keys);
        string output = "";
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            DataElement data = new DataElement();
            data.lElements = new List<string>(dic[key]);

            output += JsonUtility.ToJson(data).Replace("lElements", key).Replace("{", ",").Replace("}", "");
        }
        strJson = strJson.Substring(0, strJson.Length - 1) + output + "}";
    }

    private void AddLinkingVal(string key, string val)
    {
        if (dicLinking.ContainsKey(key))
        {
            List<string> vals = dicLinking[key];
            // append in case didn't have
            if (!vals.Contains(val))
            {
                vals.Add(val);
                dicLinking[key] = vals;
            }
        }
        else
        {
            List<string> vals = new List<string>();
            vals.Add(val);
            dicLinking.Add(key, vals);
        }
    }

    private void ReplaceLinkingKey(string oldKey, string newKey)
    {
        // replace this key & replace all #oldkey# in (alias, elements, story)
        if (dicLinking.ContainsKey(oldKey))
        {
            // replace key linked to another keys
            List<string> linkKeys = dicLinking[oldKey];
            for (int i = 0; i < linkKeys.Count; i++)
            {
                string key = linkKeys[i];
                DataType type = DataType.Element;
                if (key.Contains(DataConfig.prefixOutPutStory))
                    type = DataType.Story;
                List<string> vals = GetVal(type, key);
                // replace new linking key (for alias, elements, story)
                for (int j = 0; j < vals.Count; j++)
                {
                    vals[j] = vals[j].Replace("#" + oldKey + "#", "#" + newKey + "#");
                }
                // replace val in storage
                ReplaceVal(type, key, vals);
            }

            // replace old key in another key
            dicLinking.Add(newKey, new List<string>(dicLinking[oldKey]));
            dicLinking.Remove(oldKey);
        }
    }
}
