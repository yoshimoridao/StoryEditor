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

        // clear all old tags
        ResultZoneUtil.ClearActiveTagGroups();

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

        // show each of stories
        for (int i = 0; i < testCases.Count; i++)
        {
            DataIndexer.DataType dataType;
            DataIndex dataIndex = DataMgr.Instance.FindData(testCases[i], false, out dataType);

            // get result content of the data index
            if (dataIndex == null)
                continue;

            // list contains elements of this sentence
            List<string> _eventTagsInSentence = new List<string>();

            string val = TextUtil.AddBoldColorTag(dataIndex.RGBAColor, dataIndex.title);
            val += " = " + ResultZoneUtil.ParseToText(_eventTagsInSentence, dataIndex, dataType, false);

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

        GameMgr.Instance.RefreshCanvas();
    }

    public void ClearContent()
    {
        foreach (GameObject row in rows)
        {
            row.GetComponentInChildren<Text>().text = "";
            row.SetActive(false);
        }
    }
}
