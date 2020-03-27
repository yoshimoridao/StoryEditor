using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class DataCSVSupporter
{
    // Section, String, Character Limit, Speaker > Audience, Context, Termino
    public enum Title { SCT = 0, STR, LIM, SPK, CTX, TRM, COUNT };
    string[] titles = { "SECTION", "STRING_ID", "Character Limit", "Speaker > Audience", "Context", "Termino" };

    public enum AppendTitle { RFN, COUNT };
    string[] appendTitles = { "Reference" };

    // constant
    const int characterLimit = 120;
    private DataIndexer dataIndexer;

    // localization (pair: data_key -> elements -> localizations)
    private Dictionary<DataIndex, List<List<string>>> loc = new Dictionary<DataIndex, List<List<string>>>();

    public bool IsLocAvailable { get { return loc.Count > 0; } }

    // ========================================= UNITY FUNCS =========================================
    public void Init(DataIndexer _dataIndexer)
    {
        dataIndexer = _dataIndexer;
    }

    #region export
    // export for excel file
    public void Export(string _path)
    {
        DataMgr.DataExportGame dataExportGame = new DataMgr.DataExportGame();
        var storiesData = dataIndexer.GetDatas(DataIndexer.DataType.Story);
        var elementsData = dataIndexer.GetDatas(DataIndexer.DataType.Element);

        // add STORY Title's val
        string storyTitle = GetNameSaveFile(_path);
        dataExportGame.elements.Add(new DataMgr.ElementExportGame("Story_Title", storyTitle));

        // list reference value of story (for append column of reference value)
        List<string> referStoryVals = new List<string>();

        // load data
        for (int i = 0; i < 2; i++)
        {
            DataIndexer.DataType dataType = i == 0 ? DataIndexer.DataType.Story : DataIndexer.DataType.Element;
            var dataIndexes = i == 0 ? storiesData : elementsData;
            for (int j = 0; j < dataIndexes.Count; j++)
            {
                DataIndex dataIndex = dataIndexes[j];

                // clone title
                DataMgr.ElementExportGame tmpElement = new DataMgr.ElementExportGame();
                tmpElement.title = dataIndex.title;

                // clone story value
                if (dataType == DataIndexer.DataType.Story)
                {
                    tmpElement.elements = new List<string>();
                    tmpElement.elements.Add(DataMgr.MergeAllElements(dataIndex));

                    // get reference value of story (for append column of reference value)
                    List<string> eventTagsInSentence = new List<string>();
                    referStoryVals.Add(ResultZoneUtil.ParseToText(eventTagsInSentence, dataIndex, DataIndexer.DataType.Story, false, false));
                }
                // clone element value
                else
                {
                    tmpElement.elements = new List<string>();
                    foreach (var tmp in dataIndex.elements)
                        tmpElement.elements.Add(tmp.value);
                }

                // add null to output is [""] -> fix bug read element wrong
                if (tmpElement.elements.Count == 0)
                    tmpElement.elements.Add("");

                if (tmpElement != null)
                    dataExportGame.elements.Add(tmpElement);

                // add pronouns row for story contain [Pronoun]
                if (dataType == DataIndexer.DataType.Story)
                    AddPronounRowForStory(dataExportGame, tmpElement);
            }
        }

        string output = "";
        // add header for row 1,2
        AddHeaderR1_2(ref output);

        // add elements's value (from row 4,...)
        for (int i = 0; i < dataExportGame.elements.Count; i++)
        {
            var exportData = dataExportGame.elements[i];
            string dataTitle = "";
            // --- add TITLE
            dataTitle = exportData.title;
            AddField(ref output, dataTitle.ToUpper());
            BreakDown(ref output);

            // --- add VAL
            // append reference value of story (-1 for first title)
            string referStoryVal = "";
            if (i >= 1 && (i - 1) < referStoryVals.Count)
                referStoryVal = referStoryVals[i - 1];

            // traverse each rows (value of element)
            FillValForEachColumns(ref output, exportData, dataTitle, (i != 0), referStoryVal);
        }

        // --- Save ---
        string savePath = _path.Replace(".txt", ".csv");
        if (File.Exists(savePath))
        {
            File.WriteAllText(savePath, output);
        }
        else
        {
            StreamWriter writer = new StreamWriter(savePath);
            writer.Write(output);
            writer.Close();
        }
    }

    private void FillValForEachColumns(ref string output, DataMgr.ElementExportGame exportData, string dataTitle, bool isAddNumbSuffix, string referStoryVal)
    {
        int titleCol = (int)Title.COUNT;
        int lanCol = (int)Localization.LanguageCode.COUNT;
        int appendCol = (int)AppendTitle.COUNT;
        int columns = titleCol + lanCol + appendCol;

        for (int j = 0; j < exportData.elements.Count; j++)
        {
            var val = exportData.elements[j];

            // traverse each columns
            for (int col = 0; col < columns; col++)
            {
                // title
                if (col == (int)Title.STR)
                {
                    string fieldVal = dataTitle;
                    // skip plus index for first story title
                    if (isAddNumbSuffix)
                        fieldVal += "_" + ((j + 1) <= 9 ? "0" : "") + (j + 1).ToString();
                    AddField(ref output, fieldVal);
                }
                // character limit
                else if (col == (int)Title.LIM)
                {
                    AddField(ref output, characterLimit.ToString());
                }
                // english field
                else if (col == titleCol + (int)Localization.LanguageCode.EN)
                {
                    AddField(ref output, ReplaceTitleOfHashKey(val));
                }
                // append title (reference)
                else if (col == titleCol + lanCol + (int)AppendTitle.RFN)
                {
                    // append reference value of story (-1 for first title)
                    if (referStoryVal.Length > 0 && j == 0)
                        AddField(ref output, referStoryVal);
                }
                else
                {
                    AddEmptyField(ref output, 1);
                }
            }
            BreakDown(ref output);
        }
    }
    private void AddPronounRowForStory(DataMgr.DataExportGame _dataExport, DataMgr.ElementExportGame _eExport)
    {
        if (_eExport == null || _eExport.elements.Count == 0)
            return;

        string rootExportVal = _eExport.elements[0];
        DataIndexer.DataType dataType;
        var pronounData = DataMgr.Instance.FindData("Pronoun", true, out dataType);
        if (pronounData == null || !rootExportVal.Contains(pronounData.genKey))
            return;

        string markStr = "++" + pronounData.genKey + "++"; // ++key++

        // Replace all #key# to mark key "++key++id"
        string pattern = "#" + pronounData.genKey + "#";
        int refCount = 0;
        int traverseId = 0;
        while ((traverseId = rootExportVal.IndexOf(pattern, traverseId)) != -1)
        {
            string tmpMark = markStr + refCount;
            // replace key "#key# by mark "++key++id"
            rootExportVal = rootExportVal.Remove(traverseId, pattern.Length).Insert(traverseId, tmpMark);
            traverseId += tmpMark.Length;
            refCount++;
        }
        // replace #key# to result [value]
        rootExportVal = ReplaceTitleOfHashKey(rootExportVal);

        // Gen vals for each Pronoun
        List<int> indexes = new List<int>();
        for (int i = 0; i < refCount; i++)
            indexes.Add(0);
        traverseId = indexes.Count - 1;

        int eCount = pronounData.elements.Count;
        int loopTurn = Mathf.FloorToInt(Mathf.Pow(eCount, refCount));
        for (int i = 0; i < loopTurn; i++)
        {
            // gen export data
            string cloneExportVal = rootExportVal;
            for (int j = 0; j < refCount; j++)
            {
                int eId = indexes[j];
                string eValue = pronounData.elements[eId].value;
                // replace ++key++id -> element's value
                cloneExportVal = cloneExportVal.Replace(markStr + j.ToString(), eValue);
            }
            _eExport.elements.Add(cloneExportVal);

            // update id
            for (int j = indexes.Count - 1; j >= 0; j--)
            {
                indexes[j]++;
                if (indexes[j] >= eCount)
                {
                    indexes[j] = 0;
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
    }
    private int CountStringOccurrences(string text, string pattern)
    {
        // Loop through all instances of the string 'text'.
        int count = 0;
        int i = 0;
        while ((i = text.IndexOf(pattern, i)) != -1)
        {
            i += pattern.Length;
            count++;
        }
        return count;
    }

    private void AddHeaderR1_2(ref string _cont)
    {
        // row 1 + 2
        for (int r = 0; r < 2; r++)
        {
            // add titles
            for (int col = 0; col < (int)Title.COUNT; col++)
            {
                if (col < titles.Length)
                    AddField(ref _cont, r == 0 ? titles[col] : "[" + ((Title)col).ToString() + "]");
            }
            // add languages
            for (int col = 0; col < (int)Localization.LanguageCode.COUNT; col++)
            {
                AddField(ref _cont, r == 0 ? Localization.GetLanguage(col) : "[" + ((Localization.LanguageCode)col).ToString() + "]");
            }
            // add append
            for (int col = 0; col < (int)AppendTitle.COUNT; col++)
            {
                AddField(ref _cont, r == 0 ? appendTitles[col] : "[" + ((AppendTitle)col).ToString() + "]");
            }
            BreakDown(ref _cont);
        }
    }
    private void AddEmptyField(ref string _cont, int _amount)
    {
        for (int i = 0; i < _amount; i++)
            AddField(ref _cont, "");
    }
    private void AddField(ref string _cont, string _val) { _cont += "\"" + _val + "\","; }
    private void BreakDown(ref string _cont) { _cont += "\n"; }

    private string ReplaceTitleOfHashKey(string _val)
    {
        string result = _val;
        char[] splitter = { '#' };
        string[] eKeys = result.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

        foreach (string eKey in eKeys)
        {
            DataIndexer.DataType findDataType;
            DataIndex eData = dataIndexer.FindData(eKey, false, out findDataType);
            if (eData != null)
                result = result.Replace("#" + eData.genKey + "#", "[" + eData.title + "]");
        }
        return result;
    }

    private string GetNameSaveFile(string _path)
    {
        var splitStr = Util.SplitString(_path, '\\');
        if (splitStr.Length > 0)
            return splitStr[splitStr.Length - 1].Replace(".csv", "");
        return "";
    }
    #endregion

    #region import
    // import for excel file
    public void Import(string _path)
    {
        // clear all LOC before import new LOC
        loc.Clear();

        if (_path.Length == 0 || !File.Exists(_path))
            return;

        var lines = File.ReadAllLines(_path);
        char[] splitter = { '\"' };
        DataIndexer.DataType readingDataType = DataIndexer.DataType.Story;
        DataIndex readingDataIndex = null;
        List<List<string>> elemntLoc = new List<List<string>>();
        // start read from 3rd line
        for (int i = 3; i < lines.Length; i++)
        {
            List<string> fields = new List<string>(lines[i].Split(splitter));
            // remove fst and last item empty
            RemoveFstAndLstEmptyItem(fields);
            // remove all comma
            fields.RemoveAll(x => x == ",");

            if (fields.Count == 0)
                continue;

            // fst field -> title
            string fieldVal = GetFieldValue(fields, (int)Title.SCT);
            if (fieldVal.Length > 0)
            {
                var findData = dataIndexer.FindData(fieldVal, true, out readingDataType);
                // store reading data index
                if (findData != null)
                {
                    // store localization of last element
                    if (readingDataIndex != null && !loc.ContainsKey(readingDataIndex) && elemntLoc.Count > 0)
                    {
                        loc.Add(readingDataIndex, new List<List<string>>(elemntLoc));
                        elemntLoc.Clear();
                    }

                    readingDataIndex = findData;
                    continue;
                }
            }
            // snd field -> element's value
            fieldVal = GetFieldValue(fields, (int)Title.STR);
            if (readingDataIndex != null && fieldVal.Length > 0 && fieldVal.ToLower().Contains(readingDataIndex.title.ToLower()))
            {
                // add element's value
                List<string> locVal = new List<string>();
                int startId = (int)(Title.COUNT) + (int)(Localization.LanguageCode.EN);
                for (int j = startId; j < (int)(startId + Localization.LanguageCode.COUNT); j++)
                {
                    fieldVal = GetFieldValue(fields, j);
                    // if null for english
                    if (j == startId && fieldVal.Length == 0)
                        break;

                    // replace reference title to genkey [title] -> #genkey#
                    fieldVal = ReplaceKeyOfHashKey(fieldVal);
                    // add localization value
                    locVal.Add(fieldVal);
                }

                // add full loc for element
                elemntLoc.Add(locVal);
            }

            // store localization of last element
            if (i == lines.Length - 1 && readingDataIndex != null && !loc.ContainsKey(readingDataIndex) && elemntLoc.Count > 0)
            {
                loc.Add(readingDataIndex, new List<List<string>>(elemntLoc));
                elemntLoc.Clear();
            }
        }

        // override save data
        OverrideData();
    }

    private void OverrideData()
    {
        // default override for english
        List<DataIndex> keys = new List<DataIndex>(loc.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            DataIndex dataIndex = keys[i];
            // find data type
            DataIndexer.DataType readingDataType = DataIndexer.DataType.Story;
            dataIndexer.FindData(dataIndex.genKey, false, out readingDataType);

            var elemnts = loc[dataIndex];
            int neededElemnts = elemnts.Count;
            for (int j = 0; j < elemnts.Count; j++)
            {
                if (elemnts[j].Count <= (int)Localization.LanguageCode.EN)
                    continue;

                // get value for English language
                string eVal = elemnts[j][(int)Localization.LanguageCode.EN];

                // substring into small texts for Story
                if (readingDataType == DataIndexer.DataType.Story)
                {
                    var splitVals = SeparateText(eVal);
                    for (int k = 0; k < splitVals.Count; k++)
                    {
                        OverrideDataIndex(k, dataIndex, splitVals[k]);
                    }
                    neededElemnts = splitVals.Count;
                }
                // normal save for Element
                else
                {
                    OverrideDataIndex(j, dataIndex, eVal);
                }
            }
            // remove surplus elements
            if (dataIndex.elements.Count > neededElemnts)
                dataIndex.RemoveElements(neededElemnts, dataIndex.elements.Count - neededElemnts);
        }
    }

    private void OverrideDataIndex(int _index, DataIndex _dataIndex, string _val)
    {
        // override element's value
        if (_index < _dataIndex.elements.Count)
            _dataIndex.elements[_index].value = _val;   // default override english // add new element
        else
            _dataIndex.AddElement(_val);
    }
    private string GetFieldValue(List<string> fields, int _id)
    {
        if (_id < fields.Count)
        {
            if (fields[_id] != ",")
                return fields[_id];
        }

        return "";
    }
    private string ReplaceKeyOfHashKey(string _val)
    {
        string result = _val;
        char[] splitter = { '[', ']' };
        string[] eKeys = result.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

        foreach (string eKey in eKeys)
        {
            DataIndexer.DataType findDataType;
            DataIndex eData = dataIndexer.FindData(eKey, true, out findDataType);
            if (eData != null)
                result = result.Replace("[" + eData.title + "]", "#" + eData.genKey + "#");
        }
        return result;
    }
    private List<string> SeparateText(string _val)
    {
        List<string> result = new List<string>();
        char[] splitter = { '#', '#' };
        List<string> splitStr = new List<string>(_val.Split(splitter, StringSplitOptions.RemoveEmptyEntries));

        for (int i = 0; i < splitStr.Count; i++)
        {
            string str = splitStr[i];
            DataIndexer.DataType type = DataIndexer.DataType.Story;
            if (str == "error" || dataIndexer.FindData(str, false, out type) != null)
            {
                result.Add("#" + str + "#");
            }
            else if (str.Length > 0)
            {
                int findId = -1;
                do
                {
                    findId = -1;
                    for (int j = 0; j <= 4; j++)
                    {
                        char splitChar = '.';
                        switch (j)
                        {
                            case 0: splitChar = ' '; break;
                            case 1: splitChar = ','; break;
                            case 2: splitChar = '.'; break;
                            case 3: splitChar = '!'; break;
                            case 4: splitChar = '?'; break;
                        }
                        int tmpFindId = str.IndexOf(splitChar);
                        // just split for space char for fst index
                        if (tmpFindId == -1 || (splitChar == ' ' && tmpFindId > 0))
                            continue;

                        if (findId != -1)
                        {
                            if (tmpFindId < findId)
                                findId = tmpFindId;
                        }
                        else
                        {
                            findId = tmpFindId;
                        }
                    }

                    if (findId != -1)
                    {
                        result.Add(str.Substring(0, findId + 1));   // pre split
                        str = str.Substring(findId + 1);    // suff split
                    }
                    else if (str.Length > 0)
                    {
                        result.Add(str);    // add left text
                    }
                } while (findId != -1);
            }
        }

        return result;
    }

    private void RemoveFstAndLstEmptyItem(List<string> _vals)
    {
        // remove fst item empty
        if (_vals.Count > 0 && _vals[0].Length == 0)
            _vals.RemoveAt(0);
        // remove lst item empty
        if (_vals.Count > 0 && _vals[_vals.Count - 1].Length == 0)
            _vals.RemoveAt(_vals.Count - 1);
    }
    #endregion
}
