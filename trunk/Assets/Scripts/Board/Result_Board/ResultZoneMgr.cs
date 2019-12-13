using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultZoneMgr : MonoBehaviour
{
    public Text resultText;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void ShowResult(CommonPanel panel)
    {

    }

    // ========================================= PRIVATE FUNCS =========================================
    public void ShowResult()
    {
        List<string> testCases = DataMgr.Instance.GetTestCases();

        string result = "";
        for (int i = 0; i < testCases.Count; i++)
        {
            DataIndexer.DataType dataType;
            DataIndex dataIndex = DataMgr.Instance.GetIndex(testCases[i], out dataType);
            string val = ParseDataIndex(dataIndex, dataType);
            if (val.Length > 0)
            {
                if (result.Length > 0)
                    result += "\n";
                result += "<b>" + dataIndex.key + "</b>" + " = " + val;
            }
        }

        resultText.text = result;
    }

    private string ParseDataIndex(DataIndex dataIndex, DataIndexer.DataType dataType)
    {
        if (dataIndex == null)
            return "";

        string val = "";
        // pick random one element
        if (dataType == DataIndexer.DataType.Element)
        {
            // add color tag for another color (!= white)
            if ((ColorBar.ColorType)dataIndex.colorId != ColorBar.ColorType.WHITE)
                val += ParseColorTag((ColorBar.ColorType)dataIndex.colorId);

            if (dataIndex.elements.Count > 0)
                val += dataIndex.elements[Random.Range(0, dataIndex.elements.Count)];

            // close color tag
            if ((ColorBar.ColorType)dataIndex.colorId != ColorBar.ColorType.WHITE)
                val += "</color>";
        }
        // merge all element value
        else
        {
            val = DataMgr.Instance.MergeAllElements(dataIndex);
        }

        List<string> linkKeys = DataMgr.Instance.ParseRetrieveLinkId(val);
        for (int i = 0; i < linkKeys.Count; i++)
        {
            string linkKey = linkKeys[i];
            DataIndexer.DataType linkType = DataIndexer.DataType.Element;
            DataIndex linkData = DataMgr.Instance.GetIndex(linkKeys[i], out linkType);
            if (linkData != null)
            {
                string linkVal = ParseDataIndex(linkData, linkType);
                if (linkVal.Length > 0)
                    val = val.Replace("#" + linkKey + "#", linkVal);
            }
        }

        return val;
    }

    private string ParseColorTag(ColorBar.ColorType colorType)
    {
        string val = "<color=";
        switch (colorType)
        {
            case ColorBar.ColorType.WHITE:
                break;
            case ColorBar.ColorType.BLACK:
            case ColorBar.ColorType.RED:
            case ColorBar.ColorType.CYAN:
            case ColorBar.ColorType.GREEN:
            case ColorBar.ColorType.BLUE:
            case ColorBar.ColorType.ORANGE:
            case ColorBar.ColorType.PURPLE:
                val += colorType.ToString().ToLower();
                break;
            default:
                break;
        }

        val += ">";

        return val;
    }
}
