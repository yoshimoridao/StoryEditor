using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultZoneMgr : MonoBehaviour
{
    public Transform transCont;
    public Text resultText;
    public Sprite rdResultImg;
    public Sprite pickingResultImg;

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

    public void ShowResult(Panel panel)
    {

    }

    public void ShowResult(List<string> testCases, bool isRdMode)
    {
        //if (!prefResultRow || !transCont)
        //    return;

        //// generate new rows
        //if (rows.Count < testCases.Count)
        //{
        //    int turn = testCases.Count - rows.Count;
        //    for (int i = 0; i < turn; i++)
        //        rows.Add(Instantiate(prefResultRow, transCont));
        //}
        //// show using row || hide un-used rows
        //else
        //{
        //    for (int i = 0; i < rows.Count; i++)
        //        rows[i].SetActive(i < testCases.Count);
        //}

        //for (int i = 0; i < testCases.Count; i++)
        //{
        //    DataIndexer.DataType dataType;
        //    DataIndex dataIndex = DataMgr.Instance.GetIndex(testCases[i], out dataType);
        //    // get result content of the data index
        //    string val = ParseDataIndex(dataIndex, dataType);

        //    // show result of each row
        //    val = "<b>" + dataIndex.genKey + "</b>" + " = " + val;

        //    if (i < rows.Count)
        //    {
        //        GameObject row = rows[i];
        //        // change content of text
        //        row.GetComponentInChildren<Text>().text = val;

        //        // change icon for result row
        //        for (int j = 0; j < row.transform.childCount; j++)
        //        {
        //            Image rowImg = row.transform.GetChild(j).GetComponent<Image>();
        //            if (rowImg && rdResultImg && pickingResultImg)
        //            {
        //                rowImg.sprite = isRdMode ? rdResultImg : pickingResultImg;
        //                break;
        //            }
        //        }
        //    }
        //}

        //CanvasMgr.Instance.RefreshCanvas();
    }

    // ========================================= PRIVATE FUNCS =========================================
    private string ParseDataIndex(DataIndex dataIndex, DataIndexer.DataType dataType)
    {
        if (dataIndex == null)
            return "";

        string val = "";
        // pick random one element
        //if (dataType == DataIndexer.DataType.Element)
        //{
        //    // add color tag for another color (!= white)
        //    if ((ColorBar.Color)dataIndex.colorId != ColorBar.Color.WHITE)
        //        val += TextUtil.GetOpenColorTag((ColorBar.Color)dataIndex.colorId);

        //    if (dataIndex.elements.Count > 0)
        //    {
        //        List<string> elements = new List<string>();
        //        //// get testing elements
        //        //if (dataIndex.testingIndex.Count > 0)
        //        //    elements = dataIndex.GetTestingElement();
        //        //// unless get all elements
        //        //else
        //        //    elements = dataIndex.elements;

        //        // random pick 1 elements
        //        if (elements.Count > 0)
        //            val += elements[Random.Range(0, elements.Count)];
        //    }


        //    // close color tag
        //    if ((ColorBar.Color)dataIndex.colorId != ColorBar.Color.WHITE)
        //        val += TextUtil.GetCloseColorTag();
        //}
        //// merge all element value
        //else
        //{
        //    val = DataMgr.Instance.MergeAllElements(dataIndex);
        //}

        //List<string> linkKeys = DataMgr.Instance.ParseRetrieveLinkId(val);
        //for (int i = 0; i < linkKeys.Count; i++)
        //{
        //    string linkKey = linkKeys[i];
        //    DataIndexer.DataType linkType = DataIndexer.DataType.Element;
        //    DataIndex linkData = DataMgr.Instance.GetIndex(linkKeys[i], out linkType);
        //    if (linkData != null)
        //    {
        //        string linkVal = ParseDataIndex(linkData, linkType);
        //        if (linkVal.Length > 0)
        //            val = val.Replace("#" + linkKey + "#", linkVal);
        //    }
        //}

        return val;
    }
}
