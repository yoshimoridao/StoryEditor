﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class DataCSVSupporter
{
    // Section, String, Character Limit, Speaker > Audience, Context, Termino
    public enum Title { SCT = 0, STR, LIM, SPK, CTX, TRM, COUNT };
    string[] titles = { "SECTION", "STRING_ID", "Character Limit", "Speaker > Audience", "Context", "Termino" };
    enum Language { EN = 0, FR, ES, DE, PT, RU, TH, TR, AR, ID, IT, COUNT };
    string[] languages = { "English", "French", "Spanish", "German", "Portuguese", "Russian", "Thai", "Turkish", "Arabic", "Indonesian", "Italian" };

    // constant
    const int characterLimit = 120;
    DataIndexer dataIndexer;

    // localization (pair: data_key -> elements -> localizations)
    Dictionary<DataIndex, List<List<string>>> loc = new Dictionary<DataIndex, List<List<string>>>();

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
            // traverse each rows (value of element)
            for (int j = 0; j < exportData.elements.Count; j++)
            {
                var val = exportData.elements[j];

                // traverse each columns
                int columns = (int)Title.COUNT + (int)Language.EN;
                for (int col = 0; col <= columns; col++)
                {
                    // title
                    if (col == (int)Title.STR)
                    {
                        string fieldVal = dataTitle;
                        // skip plus index for fist story title
                        if (i != 0)
                            fieldVal += "_" + ((j + 1) <= 9 ? "0" : "") + (j + 1).ToString();
                        AddField(ref output, fieldVal);
                    }
                    // character limit
                    else if (col == (int)Title.LIM)
                    {
                        AddField(ref output, characterLimit.ToString());
                    }
                    // english field
                    else if (col == columns)
                    {
                        AddField(ref output, ReplaceTitleOfHashKey(val));
                    }
                    else
                    {
                        AddEmptyField(ref output, 1);
                    }
                }
                BreakDown(ref output);
            }
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
            for (int col = 0; col < (int)Language.COUNT; col++)
            {
                if (col < languages.Length)
                    AddField(ref _cont, r == 0 ? languages[col] : "[" + ((Language)col).ToString() + "]");
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
                int startId = (int)(Title.COUNT) + (int)(Language.EN);
                for (int j = startId; j < (int)(startId + Language.COUNT); j++)
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
                if (elemnts[j].Count <= (int)Language.EN)
                    continue;

                // get value for English language
                string eVal = elemnts[j][(int)Language.EN];

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