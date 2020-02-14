using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static bool IsOldSaveFormat(string _content)
    {
        // is old save file format
        if (_content.Contains("\"colorId\":"))
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
            newData.genKey = oldSaveFormater.genKey;
            newData.isRdTest = oldSaveFormater.isRdTest;
            newData.rdTestCaseAmount = oldSaveFormater.rdTestCaseAmount;
            newData.testCaseIds = oldSaveFormater.testCaseIds;
            newData.normalFontSize = oldSaveFormater.normalFontSize;

            // load elements & stories
            for (int i = 0; i < 2; i++)
            {
                List<OldDataIndex> dataIndexes = i == 0 ? oldSaveFormater.elements : oldSaveFormater.stories;
                foreach (OldDataIndex child in dataIndexes)
                {
                    DataIndex newDataIndex = new DataIndex();
                    newDataIndex.genKey = child.genKey;
                    newDataIndex.title = child.title;
                    newDataIndex.elements = child.elements;
                    newDataIndex.testElements = child.testElements;

                    // convert from old color format to new color format
                    Color oldColor = Color.white;
                    switch (child.colorId)
                    {
                        case 0: oldColor = Color.white;
                            break;
                        case 1: oldColor = Color.black;
                            break;
                        case 2: oldColor = Color.red;
                            break;
                        case 3: oldColor = Color.cyan;
                            break;
                        case 4: oldColor = Color.green;
                            break;
                        case 5: oldColor = Color.blue;
                            break;
                            // orange
                        case 6: oldColor = new Color(1, 0.5f, 0.0f, 1.0f);
                            break;
                            // purple
                        case 7: oldColor = new Color(1, 0.0f, 1, 1.0f);
                            break;
                        default:
                            break;
                    }
                    newDataIndex.SetColor(oldColor);

                    // add elements
                    if (i == 0)
                        newData.elements.Add(newDataIndex);
                    else
                        newData.stories.Add(newDataIndex);
                }
            }
        }

        return newData;
    }

    // ========== Parse color ==========
    /// <summary>
    /// convert string (r:1,g:1,b:1,a:1) to color
    /// </summary>
    /// <param name="_val"></param>
    /// <returns></returns>
    public static Color ParseTextToColor(string _val)
    {
        char[] splitters = { ',' };
        string[] tmpSplit = _val.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);

        // white is default color
        Color resultColor = Color.white;
        for (int i = 0; i < tmpSplit.Length; i++)
        {
            string splitVal = tmpSplit[i];
            if (splitVal.Split(':').Length <= 1)
                break;

            string colorScale = tmpSplit[i].Split(':')[1];
            float colorParse = 0;
            // if parse color's value false
            if (!float.TryParse(colorScale, out colorParse))
                break;

            switch (i)
            {
                case 0:
                    resultColor.r = colorParse;
                    break;
                case 1:
                    resultColor.g = colorParse;
                    break;
                case 2:
                    resultColor.b = colorParse;
                    break;
                case 3:
                    resultColor.a = colorParse;
                    break;

            }
        }

        return resultColor;
    }

    /// <summary>
    /// convert color to string (r:1,g:1,b:1,a:1)
    /// </summary>
    /// <param name="_color"></param>
    /// <returns></returns>
    public static string ParseColorToText(Color _color)
    {
        string colorParse = "r:" + _color.r + "," +
            "g:" + _color.g + "," +
            "b:" + _color.b + "," +
            "a:" + _color.a;

        return colorParse;
    }
}
