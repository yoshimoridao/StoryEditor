using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataMgr : Singleton<DataMgr>
{
    public enum DataType { Alias, Element, Story, Count, Null, Linking };

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

        public DataStorage() {}

        public void ChangeStory(string key)
        {
            origin = origin.Replace("#stories#", "#" + key + "#");
        }
    }

    Dictionary<string, string> dicAlias = new Dictionary<string, string>();
    Dictionary<string, string> dicElements = new Dictionary<string, string>();
    Dictionary<string, string> dicStories = new Dictionary<string, string>();

    // format -> 1:2,3,4 (these key (2,3,4) link to key 1)
    Dictionary<string, string> dicLinking = new Dictionary<string, string>();
    [SerializeField]
    DataStorage dataStorage = new DataStorage();

    // ========================================= GET/ SET =========================================
    public string GetVal(string key, out DataType type)
    {
        type = DataType.Null;
        for (int i = (int)DataType.Alias; i < (int)DataType.Count; i++)
        {
            type = (DataType)i;
            var dic = GetDic(type);
            if (dic.ContainsKey(key))
                return dic[key];
        }
        return "";
    }

    public void AddVal(DataType type, string key, string val)
    {
        if (IsNewKeyAvailable(key))
        {
            // add new val in dic
            var dic = GetDic(type);
            dic.Add(key, val);

            Debug.Log("Save val vs key = " + key + ", val = " + val + ", type = " + type.ToString());
        }
        else
        {
            ReplaceVal(type, key, val);

            Debug.Log("Replace val vs key = " + key + ", val = " + val + ", type = " + type.ToString());
        }
    }

    public void ReplaceKey(string oldKey, string newKey)
    {
        // check this key is available
        if (!IsNewKeyAvailable(newKey))
        {
            Debug.Log("new key is not available");
            return;
        }

        // check the key in all of dics
        for (int i = (int)DataType.Alias; i < (int)DataType.Count; i++)
        {
            var dic = GetDic((DataType)i);

            // override key
            if (dic.ContainsKey(oldKey))
            {
                dic.Add(newKey, dic[oldKey]);
                dic.Remove(oldKey);
                break;
            }
        }

        // replace old key by new key in linking dictionary
        ReplaceValInLinkingDic(oldKey, newKey);
        // export text file
        ExportSaveFile();
    }

    public void ReplaceVal(DataType type, string key, string newVal)
    {
        var dic = GetDic(type);
        if (dic.ContainsKey(key))
        {
            dic[key] = newVal;
        }
    }

    public void ReplaceVal(string key, string newVal)
    {
        for (int i = (int)DataType.Alias; i < (int)DataType.Count; i++)
        {
            var dic = GetDic((DataType)i);
            if (dic.ContainsKey(key))
            {
                dic[key] = newVal;
                return;
            }
        }
    }

    public bool IsNewKeyAvailable(string key)
    {
        for (int i = (int)DataType.Alias; i < (int)DataType.Count; i++)
        {
            if (GetDic((DataType)i).ContainsKey(key))
                return false;
        }
        return true;
    }

    private Dictionary<string, string> GetDic(DataType type)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
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
        instance = this;

        //LoadSaveFile();
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
                    string linkingKey = label.GetText();
                    val += "#" + linkingKey + "#";

                    // store the connection
                    AddValForLinkingType(linkingKey, key);
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
                    string linkingKey = label.GetText();
                    val += "#" + linkingKey + "#";

                    // store the connection
                    AddValForLinkingType(linkingKey, key);
                }
                else
                {
                    val += label.GetText();
                }
            }
        }

        // save val
        DataType type = isElementBoard ? DataType.Element : DataType.Story;
        AddVal(type, key, val);

        ExportSaveFile();
    }

    // ========================================= PRIVATE FUNCS =========================================
    //private void ExportSaveFile()
    //{
    //    dataStorage.lAlias = new List<string>();
    //    foreach (var val in dicAlias)
    //    {
    //        dataStorage.lAlias.Add(val.Key + ":" + val.Value);
    //    }

    //    dataStorage.lElements = new List<string>();
    //    foreach (var val in dicElements)
    //    {
    //        dataStorage.lElements.Add(val.Key + ":" + val.Value);
    //    }

    //    dataStorage.lStories = new List<string>();
    //    foreach (var valPair in dicStories)
    //    {
    //        dataStorage.lStories.Add(valPair.Key + ":" + valPair.Value);
    //    }

    //    dataStorage.lLinking = new List<string>();
    //    foreach (var valPair in dicLinking)
    //    {
    //        dataStorage.lLinking.Add(valPair.Key + ":" + valPair.Value);
    //    }

    //    string content = JsonUtility.ToJson(dataStorage);
    //    Debug.Log("Save = " + content);
        
    //    // create file if not exist
    //    if (!File.Exists(DataConfig.saveFilePath))
    //        File.CreateText(DataConfig.saveFilePath);
    //    // write new content
    //    File.WriteAllText(DataConfig.saveFilePath, content);
    //}

    //private void LoadSaveFile()
    //{
    //    // create file save if not exist
    //    if (!File.Exists(DataConfig.saveFilePath))
    //        File.CreateText(DataConfig.saveFilePath);

    //    string content = File.ReadAllText(DataConfig.saveFilePath);
    //    Debug.Log("Load Save File = " + content);
    //    dataStorage = JsonUtility.FromJson<DataStorage>(content);
    //}

    private void ExportSaveFile()
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
            AddElementJson(ref strOrigin, dicStories);
        if (dicElements.Count > 0)
            AddElementJson(ref strOrigin, dicElements);

        Debug.Log("Save = " + strOrigin);

        // create file if not exist
        if (!File.Exists(DataConfig.saveFilePath))
            File.CreateText(DataConfig.saveFilePath);
        // write new content
        File.WriteAllText(DataConfig.saveFilePath, strOrigin);
    }
    private void AddElementJson(ref string strJson, Dictionary<string, string> dic)
    {
        List<string> keys = new List<string>(dic.Keys);
        string output = "";
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            List<string> a = new List<string>(dic[key].Split(','));
            DataElement data = new DataElement();
            data.lElements = a;
            output += JsonUtility.ToJson(data).Replace("lElements", key).Replace("{", ",").Replace("}", "");
        }
        strJson = strJson.Substring(0, strJson.Length - 1) + output + "}";
    }

    private void ReplaceValInLinkingDic(string oldKey, string newKey)
    {
        // replace this key & replace all #oldkey# in (alias, elements, story)
        if (dicLinking.ContainsKey(oldKey))
        {
            string val = dicLinking[oldKey];
            // replace key linked to another keys
            string[] linkKey = val.Split(',');
            for (int i = 0; i < linkKey.Length; i++)
            {
                DataType type = DataType.Null;
                string v = GetVal(linkKey[i], out type);
                // replace new linking key (for alias, elements, story)
                v = v.Replace("#" + oldKey + "#", "#" + newKey + "#");
                // replace val in storage
                ReplaceVal(type, linkKey[i], v);
            }

            // replace old key in another key
            List<string> keys = new List<string>(dicLinking.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];

                // replace old key = new key
                string[] a = dicLinking[key].Split(',');
                string tempVal = "";
                for (int j = 0; j < a.Length; j++)
                {
                    if (j > 0)
                        tempVal += ',';
                    if (a[j] == oldKey)
                        tempVal += newKey;
                    else
                        tempVal += a[j];
                }

                dicLinking[key] = tempVal;
            }

            // replace key
            dicLinking.Add(newKey, val);
            dicLinking.Remove(oldKey);
        }
    }

    private void AddValForLinkingType(string key, string val)
    {
        if (dicLinking.ContainsKey(key))
        {
            string[] a = dicLinking[key].Split(',');
            bool isAlreadyHave = false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == val)
                {
                    isAlreadyHave = true;
                    break;
                }
            }
            // append in case didn't have
            if (!isAlreadyHave)
                dicLinking[key] += "," + val;
        }
        else
        {
            dicLinking.Add(key, val);
        }
    }
}
