using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static bool IsOldSaveFormat(string _content)
    {
        // is old save file format
        if (!_content.Contains("isRdTest") && !_content.Contains("rdTestCaseAmount") && !_content.Contains("testCaseIds"))
            return true;
        
        return false;
    }

    // ========== Parse old save to new save ==========
    public static DataIndexer ConvertOldSaveFileToLastest(string _content)
    {
        string content = _content;
        // is old save file format
        OldSaveFormater oldSaveFormater = JsonUtility.FromJson<OldSaveFormater>(content);

        DataIndexer newData = null;
        if (oldSaveFormater != null)
        {
            newData = new DataIndexer();
            // clone title, colorId, elements, test Elements
            for (int i = 0; i < 2; i++)
            {
                List<OldDataIndex> dataIndexes = i == 0 ? oldSaveFormater.elements : oldSaveFormater.stories;
                foreach (OldDataIndex child in dataIndexes)
                {
                    DataIndex newDataIndex = new DataIndex();
                    newDataIndex.genKey = DataMgr.Instance.GenNewKey();
                    newDataIndex.title = child.key;
                    newDataIndex.colorId = child.colorId;
                    newDataIndex.elements = child.elements;
                    newDataIndex.testElements = child.testingIndex;

                    if (i == 0)
                        newData.elements.Add(newDataIndex);
                    else
                        newData.stories.Add(newDataIndex);
                }
            }
        }

        return newData;
    }

    public static DataIndexer ReplaceLinkTitlesToLinkKeys(DataIndexer _data)
    {
        if (_data == null)
            return null;

        // replace all link title to link key
        for (int i = 0; i < 2; i++)
        {
            List<DataIndex> dataIndexes = i == 0 ? _data.elements : _data.stories;
            foreach (DataIndex dataIndex in dataIndexes)
            {
                for (int j = 0; j < dataIndex.elements.Count; j++)
                {
                    string element = dataIndex.elements[j];
                    string[] splitter = { "#" };
                    string[] splitedStr = element.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (string eStr in splitedStr)
                    {
                        // find data though title
                        DataIndex findData = DataMgr.Instance.FindData(eStr, true);
                        if (findData != null)
                        {
                            // replace link title to link key
                            element = element.Replace("#" + eStr + "#", "#" + findData.genKey + "#");
                            dataIndex.elements[j] = element;
                        }
                    }
                }
            }
        }

        return _data;
    }
}
