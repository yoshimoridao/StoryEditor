using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Util : MonoBehaviour
{
    // ========== PARSE OLD SAVE TO NEW SAVE ==========
    public static DataIndexer ConvertOldSaveFileToLastest(string _content)
    {
        string content = _content;
        int versionId = 0;
        // is old save file format
        OldSaveFormater oldSaveFormater = null;
        if (content.Contains("rgbaColor"))
            versionId = 2;
        else if (content.Contains("colorId"))
            versionId = 1;

        if (versionId == 1)
            oldSaveFormater = JsonUtility.FromJson<OldSaveFormaterV001>(content);
        else if (versionId == 2)
            oldSaveFormater = JsonUtility.FromJson<OldSaveFormaterV002>(content);

        DataIndexer newData = null;
        if (oldSaveFormater != null)
        {
            newData = new DataIndexer();
            newData.genKey = oldSaveFormater.genKey;
            newData.isRdTest = oldSaveFormater.isRdTest;
            newData.rdTestCaseAmount = oldSaveFormater.rdTestCaseAmount;
            newData.normalFontSize = oldSaveFormater.normalFontSize;

            // load elements & stories
            if (versionId == 1)
                ConvertOldSaveIndex_v001((oldSaveFormater as OldSaveFormaterV001), newData);
            else if(versionId == 2)
                ConvertOldSaveIndex_v002((oldSaveFormater as OldSaveFormaterV002), newData);
        }

        return newData;
    }

    private static void ConvertOldSaveIndex_v001(OldSaveFormaterV001 oldSaveFormater, DataIndexer newData)
    {
        // load elements & stories
        for (int i = 0; i < 2; i++)
        {
            var dataIndexes = i == 0 ? oldSaveFormater.elements : oldSaveFormater.stories;

            foreach (OldDataIndexV001 child in dataIndexes)
            {
                DataIndex newDataIndex = new DataIndex();
                newDataIndex.genKey = child.genKey;
                newDataIndex.title = child.title;

                // mark test data
                if (oldSaveFormater.testCaseIds.Contains(newDataIndex.genKey))
                    newDataIndex.isTest = true;

                // gen element
                for (int j = 0; j < child.elements.Count; j++)
                {
                    string elementVal = child.elements[j];
                    DataElementIndex genElement = new DataElementIndex();
                    genElement.value = elementVal;

                    // mark testing elements
                    if (child.testElements.Contains(j))
                        genElement.isTest = true;

                    newDataIndex.elements.Add(genElement);
                }

                // convert from old color format to new color format
                Color oldColor = Color.white;
                switch (child.colorId)
                {
                    case 0: oldColor = Color.white; break;
                    case 1: oldColor = Color.black; break;
                    case 2: oldColor = Color.red; break;
                    case 3: oldColor = Color.cyan; break;
                    case 4: oldColor = Color.green; break;
                    case 5: oldColor = Color.blue; break;
                    // orange
                    case 6: oldColor = new Color(1, 0.5f, 0.0f, 1.0f); break;
                    // purple
                    case 7: oldColor = new Color(1, 0.0f, 1, 1.0f); break;
                }
                newDataIndex.RGBAColor = oldColor;

                // add elements
                if (i == 0)
                    newData.elements.Add(newDataIndex);
                else
                    newData.stories.Add(newDataIndex);
            }
        }
    }

    private static void ConvertOldSaveIndex_v002(OldSaveFormaterV002 oldSaveFormater, DataIndexer newData)
    {
        // load elements & stories
        for (int i = 0; i < 2; i++)
        {
            var dataIndexes = i == 0 ? oldSaveFormater.elements : oldSaveFormater.stories;

            foreach (OldDataIndexV002 child in dataIndexes)
            {
                DataIndex newDataIndex = new DataIndex();
                newDataIndex.genKey = child.genKey;
                newDataIndex.title = child.title;

                // mark test data
                if (oldSaveFormater.testCaseIds.Contains(newDataIndex.genKey))
                    newDataIndex.isTest = true;

                // gen element
                for (int j = 0; j < child.elements.Count; j++)
                {
                    string elementVal = child.elements[j];
                    DataElementIndex genElement = new DataElementIndex();
                    genElement.value = elementVal;

                    // mark testing elements
                    if (child.testElements.Contains(j))
                        genElement.isTest = true;

                    newDataIndex.elements.Add(genElement);
                }

                // convert from old color format to new color format
                var rgba = child.rgbaColor.Replace("r:", "").Replace("g:", "").Replace("b:", "").Replace("a:", "").Split(',');
                Color oldColor = Color.white;
                for (int j = 0; j < 4; j++)
                {
                    if (j < rgba.Length)
                    {
                        switch (j)
                        {
                            case 0: oldColor.r = float.Parse(rgba[j]); break;
                            case 1: oldColor.g = float.Parse(rgba[j]); break;
                            case 2: oldColor.b = float.Parse(rgba[j]); break;
                            case 3: oldColor.a = float.Parse(rgba[j]); break;
                        }
                    }
                }

                newDataIndex.RGBAColor = oldColor;

                // add elements
                if (i == 0)
                    newData.elements.Add(newDataIndex);
                else
                    newData.stories.Add(newDataIndex);
            }
        }
    }

    // ========== PARSE COLOR ==========
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

    // ========== MOUSE ==========
    public static List<RaycastResult> GetRayCastResultsByMousePos()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // cast all obj 
        List<RaycastResult> ray = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, ray);

        return ray;
    }

    /// checking cursor's selecting the elements.
    public static bool IsHoverObjs(GameObject _obj)
    {
        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        for (int i = 0; i < rayCast.Count; i++)
        {
            if (rayCast[i].gameObject == _obj)
            {
                return true;
            }
        }
        return false;
    }

    /// checking cursor's selecting the elements.
    public static bool IsHoverObjs(params string[] tags)
    {
        List<string> checkingTags = new List<string>(tags);

        // get ray cast all objs
        var rayCast = GetRayCastResultsByMousePos();
        for (int i = 0; i < rayCast.Count; i++)
        {
            string touchedTag = rayCast[i].gameObject.tag;
            if (checkingTags.Contains(touchedTag))
            {
                return true;
            }
        }
        return false;
    }
}
