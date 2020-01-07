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
        if (!prefResultRow || !transCont)
            return;

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
            DataIndex dataIndex = DataMgr.Instance.FindData(testCases[i], false, out dataType);

            // get result content of the data index
            if (dataIndex == null)
                continue;

            string val = TextUtil.AddBoldColorTag((ColorBar.ColorType)dataIndex.Color, dataIndex.title);
            val += " = " + ParseToText(dataIndex, dataType, false);

            if (i < rows.Count)
            {
                GameObject row = rows[i];
                // change content of text
                row.GetComponentInChildren<Text>().text = val;

                // change icon for result row
                for (int j = 0; j < row.transform.childCount; j++)
                {
                    Image rowImg = row.transform.GetChild(j).GetComponent<Image>();
                    if (rowImg && rdResultImg && pickingResultImg)
                    {
                        rowImg.sprite = isRdMode ? rdResultImg : pickingResultImg;
                        break;
                    }
                }
            }
        }

        CanvasMgr.Instance.RefreshCanvas();
    }

    public void ClearContent()
    {
        foreach (GameObject row in rows)
        {
            row.GetComponentInChildren<Text>().text = "";
            row.SetActive(false);
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private string ParseToText(DataIndex _dataIndex, DataIndexer.DataType _dataType, bool _isRefer)
    {
        if (_dataIndex == null)
            return "";

        string val = "";
        if (_dataType == DataIndexer.DataType.Story)
        {
            val += DataMgr.Instance.MergeAllElements(_dataIndex);
        }
        else
        {
            List<string> pickedElements = _dataIndex.GetTestElements();
            // random value
            if (pickedElements.Count > 0)
                val = pickedElements[Random.Range(0, pickedElements.Count)];
        }

        bool isContainLinkObj = false;
        char[] splitters = { '#' };
        string[] aSplit = val.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        // link to another refer object
        for (int i = 0; i < aSplit.Length; i++)
        {
            DataIndexer.DataType eType;
            DataIndex eIndex = DataMgr.Instance.FindData(aSplit[i], false, out eType);
            if (eIndex != null)
            {
                isContainLinkObj = true;
                string ePart = ParseToText(eIndex, eType, true);
                val = val.Replace("#" + aSplit[i] + "#", ePart);
            }
        }

        // add bold tag && color tag for refer object
        if (_isRefer && !isContainLinkObj && ((ColorBar.ColorType)_dataIndex.Color) != ColorBar.ColorType.WHITE)
        {
            //val = TextUtil.OpenBoldTag() + val + TextUtil.CloseBoldTag();
            //val = TextUtil.OpenColorTag((ColorBar.ColorType)dataIndex.Color) + val + TextUtil.CloseColorTag();

            val = TextUtil.AddBoldColorTag((ColorBar.ColorType)_dataIndex.Color, val);
        }

        return val;
    }
}
