using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// ===== store index info =====
[System.Serializable]
public class DataIndexer
{
    public enum DataType { Element, Story, Count };

    [SerializeField]
    public List<DataIndex> elements = new List<DataIndex>();
    [SerializeField]
    public List<DataIndex> stories = new List<DataIndex>();

    // ============================================ PUBLIC ============================================
    public DataIndexer() { }
    public List<DataIndex> GetStories() { return stories; }

    // ====== INDEX ======
    public DataIndex GetIndex(string key, out DataIndexer.DataType type)
    {
        type = DataType.Element;
        for (int i = 0; i < 2; i++)
        {
            DataType dataType = i == 0 ? DataType.Element : DataType.Story;
            DataIndex findData = GetIndex(dataType, key);
            if (findData != null)
            {
                type = dataType;
                return findData;
            }
        }

        return null;
    }
    public DataIndex GetIndex(DataType type, string key)
    {
        List<DataIndex> datas = GetDatas(type);

        int findId = datas.FindIndex(x => x.key == key);
        if (findId != -1 && findId < datas.Count)
            return datas[findId];

        return null;
    }

    public void AddIndex(DataIndex data)
    {
        List<DataIndex> datas = GetDatas(data.isStoryElement ? DataType.Story : DataType.Element);
        datas.Add(data);

        // export save file
        Save();
    }

    public void RemoveIndex(DataType type, string key)
    {
        List<DataIndex> datas = GetDatas(type);

        int findId = datas.FindIndex(x => x.key == key);
        if (findId != -1 && findId < datas.Count)
        {
            // remove the index
            datas.RemoveAt(findId);
            // also remove the index in another elements which refer to it
            RemoveLinkingIndex(key);
        }

        // export save file
        Save();
    }

    public void ReplaceIndexKey(DataType type, string oldKey, string newKey)
    {
        List<DataIndex> datas = GetDatas(type);

        int findId = datas.FindIndex(x => x.key == oldKey);
        if (findId != -1 && findId < datas.Count)
        {
            // replace key of the index
            datas[findId].key = newKey;
            // also replace in another elements which refer to this index
            ReplaceLinkingIndex(oldKey, newKey);
        }

        // export save file
        Save();
    }

    public void SortIndexes(DataType type, List<string> panelKeys)
    {
        List<DataIndex> sortList = new List<DataIndex>();
        for (int i = 0; i < panelKeys.Count; i++)
        {
            string key = panelKeys[i];
            DataIndex index = GetIndex(type, key);
            if (index != null)
                sortList.Add(index);
        }
        if (sortList.Count > 0)
        {
            if (type == DataType.Story)
                stories = sortList;
            else if (type == DataType.Element)
                elements = sortList;
        }

        // export save file
        Save();
    }

    public bool IsContain(DataType type, string key)
    {
        List<DataIndex> datas = GetDatas(type);

        int findId = datas.FindIndex(x => x.key == key);
        if (findId != -1 && findId < datas.Count)
            return true;

        return false;
    }

    public void ClearAll()
    {
        elements.Clear();
        stories.Clear();

        // export save file
        Save();
    }

    // ====== VAL ======
    public void AddElement(DataType type, string key, Label label)
    {
        string val = label.GetText();
        if (label is LinkLabel)
            val = "#" + val + "#";

        DataIndex dataIndex = GetIndex(type, key);
        if (dataIndex != null)
            dataIndex.AddElement(val);

        // export save file
        Save();
    }

    public void ReplaceElement(DataType type, string indexKey, int elementIndex, Label label)
    {
        string val = label.GetText();
        if (label is LinkLabel)
            val = "#" + val + "#";

        DataIndex dataIndex = GetIndex(type, indexKey);
        if (dataIndex != null)
            dataIndex.ReplaceElement(elementIndex, val);

        // export save file
        Save();
    }

    public void ReplaceElements(DataType type, string indexKey, CommonPanel panel)
    {
        List<Label> labels = panel.GetLabels();

        DataIndex dataIndex = GetIndex(type, indexKey);
        // replace list of elements of data index
        if (dataIndex != null)
        {
            List<string> tmp = new List<string>();
            foreach (var label in labels)
            {
                if (label is LinkLabel)
                    tmp.Add("#" + label.GetText() + "#");
                else
                    tmp.Add(label.GetText());
            }

            if (tmp.Count > 0)
                dataIndex.elements = tmp;
        }

        // export save file
        Save();
    }

    public void RemoveElement(DataType type, string indexKey, int elementIndex)
    {
        DataIndex dataIndex = GetIndex(type, indexKey);

        if (dataIndex != null)
            dataIndex.RemoveElement(elementIndex);

        // export save file
        Save();
    }

    public void SetColorIndex(DataType type, string indexKey, ColorBar.ColorType colorType)
    {
        DataIndex dataIndex = GetIndex(type, indexKey);

        if (dataIndex != null)
        {
            dataIndex.colorId = (int)colorType;
        }

        // export save file
        Save();
    }

    // ====== UTIL ======
    public void Load()
    {
        // load in editor
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataDefine.save_path_indexData))
            File.CreateText(DataDefine.save_path_indexData);

        string content = File.ReadAllText(DataDefine.save_path_indexData);
#else
        // load by Player Pref
        if (!PlayerPrefs.HasKey(DataConfig.indexDataSaveKey))
            return;

        string content = PlayerPrefs.GetString(DataConfig.indexDataSaveKey);
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

        // create file if not exist
#if (IN_UNITY_EDITOR)
        if (!File.Exists(DataDefine.save_path_indexData))
            File.CreateText(DataDefine.save_path_indexData);
        // write new content
        File.WriteAllText(DataDefine.save_path_indexData, strOutput);
#else
        // save to playerpref
        PlayerPrefs.SetString(DataConfig.save_key_indexData, strOutput);
#endif
    }

    public void CreateElements(DataType type)
    {
        Board board = (type == DataType.Element) ? (CanvasMgr.Instance.GetBoard<ElementBoard>()) : (CanvasMgr.Instance.GetBoard<StoryBoard>());
        List<DataIndex> dataIndexes = GetDatas(type);

        for (int i = 0; i < dataIndexes.Count; i++)
        {
            DataIndex dataIndex = dataIndexes[i];
            CommonPanel panel = null;
            if (type == DataType.Element)
                panel = (board as ElementBoard).AddPanel(dataIndex.key) as CommonPanel;
            else
                panel = (board as StoryBoard).AddPanel(dataIndex.key) as CommonPanel;

            if (panel)
            {
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    string var = dataIndex.elements[j];
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
    }

    
    public List<DataIndex> GetDatas(DataType type)
    {
        List<DataIndex> datas = new List<DataIndex>();
        switch (type)
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

    // ============================================ PRIVATE ============================================
    private void RemoveLinkingIndex(string key)
    {
        for (int i = 0; i < 2; i++)
        {
            DataType type = i == 0 ? DataType.Story : DataType.Element;

            List<DataIndex> dataIndexes = GetDatas(type);
            foreach (DataIndex index in dataIndexes)
            {
                index.ReplaceElementPart("#" + key + "#", "");
            }
        }
    }

    private void ReplaceLinkingIndex(string oldkey, string newKey)
    {
        for (int i = 0; i < 2; i++)
        {
            DataType type = i == 0 ? DataType.Story : DataType.Element;

            List<DataIndex> dataIndexes = GetDatas(type);
            foreach (DataIndex index in dataIndexes)
            {
                index.ReplaceElementPart("#" + oldkey + "#", "#" + newKey + "#");
            }
        }
    }
}
