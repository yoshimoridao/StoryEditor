using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class DataCSVExporter
{
    public enum Title { SCT = 0, STR, LIM, SPK, CTX, TRM, COUNT };
    string[] titles = { "SECTION", "STRING_ID", "Character Limit", "Speaker > Audience", "Context", "Termino" };
    enum Language { EN = 0, FR, ES, DE, PT, RU, TH, TR, AR, ID, IT, COUNT };
    string[] languages = { "English", "French", "Spanish", "German", "Portuguese", "Russian", "Thai", "Turkish", "Arabic", "Indonesian", "Italian" };
    const int characterLimit = 120;

    // ========================================= UNITY FUNCS =========================================
    #region save
    // export for excel file
    public void ExportCSVFile(string _path)
    {
        DataMgr.DataExportGame dataExportGame = new DataMgr.DataExportGame();
        var storiesData = DataMgr.Instance.DataIndexer.GetDatas(DataIndexer.DataType.Story);
        var elementsData = DataMgr.Instance.DataIndexer.GetDatas(DataIndexer.DataType.Element);

        // add STORY Title
        dataExportGame.elements.Add(new DataMgr.ElementExportGame("Story_Title", ""));

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
        string fileNameTitle = GetNameSaveFile(_path);
        // add header for row 1,2
        AddHeaderR1_2(ref output);

        // add elements's value (from row 4,...)
        for (int i = 0; i < dataExportGame.elements.Count; i++)
        {
            var exportData = dataExportGame.elements[i];
            string dataTitle = "";
            // --- add TITLE
            // story
            if (i >= 1 && i <= storiesData.Count)
            {
                dataTitle = fileNameTitle + "_Page_" + i.ToString();
                // add header for row 3 (title for stories)
                AddField(ref output, dataTitle.ToUpper());
                BreakDown(ref output);
            }
            // element
            else
            {
                // add title
                dataTitle = exportData.title;
                AddField(ref output, dataTitle.ToUpper());
                BreakDown(ref output);
            }

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
                        AddField(ref output,  fieldVal);
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
    #endregion

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
            DataIndex eData = DataMgr.Instance.FindData(eKey, false, out findDataType);
            if (eData != null)
                result = result.Replace("#" + eData.genKey + "#", "[" + eData.title + "]");
        }
        return result;
    }

    private string GetNameSaveFile(string _path)
    {
        var splitStr = Util.SplitString(_path, '/');
        if (splitStr.Length > 0)
            return splitStr[splitStr.Length - 1].Replace(".txt", "");
        return "";
    }
}
