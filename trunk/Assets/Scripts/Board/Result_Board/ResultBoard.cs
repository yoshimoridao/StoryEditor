using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultBoard : Board
{
    public ResultZoneMgr resultZone;

    private int testCaseAmount = 2;
    [SerializeField]
    private bool isRandom = true;

    // ========================================= GET/ SET =========================================
    public bool IsRandomTestCases
    {
        get { return isRandom; }
        set { isRandom = value; }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {

    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init()
    {
        base.Init();

        if (resultZone)
            resultZone.Init();
    }

    public void OnOriginBtnPress()
    {
        if (!resultZone)
            return;

        if (isRandom)
        {
            // clear old test case
            DataMgr.Instance.ClearTestCases();

            // get random testcase 
            List<string> rdTestCases = new List<string>();
            // clone list data
            List<DataIndex> tmp = new List<DataIndex>(DataMgr.Instance.GetDataStories());
            int turn = Mathf.Min(testCaseAmount, tmp.Count);
            bool isRdIndex = tmp.Count > testCaseAmount;

            for (int i = 0; i < turn; i++)
            {
                int id = isRdIndex ? Random.Range(0, tmp.Count) : i;
                if (id >= 0 && id < tmp.Count)
                {
                    // add test case for storage
                    DataMgr.Instance.AddTestCase(tmp[id].key);
                    // remove out of temp list
                    if (isRdIndex)
                        tmp.RemoveAt(id);
                }
            }
        }
        else
        {
        }

        resultZone.ShowResult();
    }

    // temp
    public void ShowResult(string text)
    {
    }
}
