using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultZoneMgr : MonoBehaviour
{
    public Transform transCont;
    public Text resultText;

    private List<GameObject> rows = new List<GameObject>();
    private GameObject prefResultRow;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        // load prefab
        prefResultRow = Resources.Load<GameObject>(DataDefine.pref_path_result_row);

        // delete template obj
        for (int i = 0; i < transCont.childCount; i++)
            Destroy(transCont.GetChild(i).gameObject);
    }

    public void ShowResult(CommonPanel panel)
    {

    }

    public void ShowResult()
    {
        if (!prefResultRow || !transCont)
            return;

        List<string> testCases = DataMgr.Instance.GetTestCases();

        // generate new rows
        if (rows.Count < testCases.Count)
        {
            int turn = testCases.Count - rows.Count;
            for (int i = 0; i < turn; i++)
                rows.Add(Instantiate(prefResultRow, transCont));
        }
        // show using row || hide un-used rows
        else
        {
            for (int i = 0; i < rows.Count; i++)
                rows[i].SetActive(i < testCases.Count);
        }

        for (int i = 0; i < testCases.Count; i++)
        {
            DataIndexer.DataType dataType;
            DataIndex dataIndex = DataMgr.Instance.GetIndex(testCases[i], out dataType);
            // get result content of the data index
            string val = ParseDataIndex(dataIndex, dataType);

            // show result of each row
            val = "<b>" + dataIndex.key + "</b>" + " = " + val;

            if (i < rows.Count)
            {
                GameObject row = rows[i];
                row.GetComponentInChildren<Text>().text = val;
            }
        }

        CanvasMgr.Instance.RefreshCanvas();
    }

    // ========================================= PRIVATE FUNCS =========================================
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
