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
            // remove all comma & fst element
            fields.RemoveAt(0);
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
    private void OverrideData()
    {
        // default override for english
        List<DataIndex> keys = new List<DataIndex>(loc.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            DataIndex dataIndex = keys[i];
            var elemnts = loc[dataIndex];
            for (int j = 0; j < elemnts.Count; j++)
            {
                string val = elemnts[j][(int)Language.EN];
                // override element's value
                if (j < dataIndex.elements.Count)
                    dataIndex.elements[j].value = val;   // default override english
                // add new element
                else
                    dataIndex.AddElement(val);
            }
            // remove surplus elements
            if (dataIndex.elements.Count > elemnts.Count)
                dataIndex.RemoveElements(elemnts.Count, dataIndex.elements.Count - elemnts.Count);
        }
    }
    #endregion
}
