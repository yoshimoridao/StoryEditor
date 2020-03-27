using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Michsky.UI.ModernUIPack;

public class ResultWindow : Singleton<ResultWindow>
{
    public ResultZoneMgr resultZone;

    [SerializeField]
    private SwitchManager switchBtn;

    [SerializeField]
    private HorizontalSelector rdAmountSelector;
    [SerializeField]
    private TextMeshProUGUI numbRd;

    [SerializeField]
    private Transform transTest;
    [SerializeField]
    private TextMeshProUGUI numbTest;

    [SerializeField]
    private bool isRandom = true;
    private int oldRd = -1;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        if (resultZone)
            resultZone.Init();

        Load();

        // scale height for all ratio
        //float canvasHeight = (CanvasMgr.Instance.transform as RectTransform).sizeDelta.y;
        float canvasHeight = (GameMgr.Instance.CurEditor as RectTransform).sizeDelta.y;

        RectTransform rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (rt.sizeDelta.y / 1080) * canvasHeight);

        // set size for result zone
        if (resultZone)
        {
            rt = (resultZone.transform as RectTransform);
            (resultZone.transform as RectTransform).sizeDelta = new Vector2(rt.sizeDelta.x, (rt.sizeDelta.y / 1080) * canvasHeight);
        }
    }

    public void Load()
    {
        // Load testing mode
        isRandom = DataMgr.Instance.IsRandomTest;

        if (switchBtn.isOn == isRandom)
        {
            switchBtn.AnimateSwitch();
        }

        // active panel
        ActivePanel();

        // refresh content
        resultZone.ClearContent();
    }

    public void OnDestroy()
    {
    }

    // ====== Event Button ======
    public void OnOriginBtnPress()
    {
        if (!resultZone)
            return;

        List<string> testCases = new List<string>();
        if (isRandom)
        {
            // clone list data
            List<DataIndex> stories = new List<DataIndex>(DataMgr.Instance.Stories);
            int turn = Mathf.Min(DataMgr.Instance.RdTestCaseAmount, stories.Count);

            bool isCanRd = stories.Count > DataMgr.Instance.RdTestCaseAmount;
            for (int i = 0; i < turn; i++)
            {
                int id = isCanRd ? UnityEngine.Random.Range(0, stories.Count) : i;
                if (id >= 0 && id < stories.Count)
                {
                    // add test case for storage
                    testCases.Add(stories[id].genKey);
                    // remove out of temp list
                    if (isCanRd)
                        stories.RemoveAt(id);
                }
            }
        }
        else
        {
            testCases = DataMgr.Instance.GetTestingDataVals();
        }

        resultZone.ShowResult(testCases, isRandom);
    }

    public void OnSwitchButtonPress(bool _isTestMode)
    {
        // default de-active random mode
        isRandom = !_isTestMode;
        // Save current testing mode
        DataMgr.Instance.IsRandomTest = isRandom;
        // active panel
        ActivePanel();
    }

    public void ForwardClick()
    {
        int storiesAmount = DataMgr.Instance.Stories.Count;

        oldRd = DataMgr.Instance.RdTestCaseAmount;
        DataMgr.Instance.RdTestCaseAmount++;

        if (rdAmountSelector.itemList.Count > 1)
        {
            rdAmountSelector.index = oldRd < storiesAmount ? 0 : 1;
            rdAmountSelector.itemList[0].itemTitle = oldRd.ToString();
            rdAmountSelector.itemList[1].itemTitle = DataMgr.Instance.RdTestCaseAmount.ToString();
        }
    }

    public void PreviousClick()
    {
        oldRd = DataMgr.Instance.RdTestCaseAmount;
        DataMgr.Instance.RdTestCaseAmount--;

        if (rdAmountSelector.itemList.Count > 1)
        {
            rdAmountSelector.index = oldRd <= 1 ? 0 : 1;
            rdAmountSelector.itemList[0].itemTitle = DataMgr.Instance.RdTestCaseAmount.ToString();
            rdAmountSelector.itemList[1].itemTitle = oldRd.ToString();
        }
    }

    //public void OnClearAllBtnPress()
    //{
    //    GameMgr.Instance.ClearAllTestCases();
    //}

    // = Random mode panel =
    public void OnEditRdAmountTextDone()
    {
        int inputAmount = -1;
        if (numbRd && int.TryParse(numbRd.text, out inputAmount) && inputAmount != -1)
        {
            DataMgr.Instance.RdTestCaseAmount = inputAmount;
        }
        else
        {
            numbRd.text = oldRd.ToString();
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void ActivePanel()
    {
        // test
        transTest.gameObject.SetActive(!isRandom);
        numbTest.gameObject.SetActive(!isRandom);
        numbTest.text = DataMgr.Instance.GetTestingDataVals().Count.ToString();

        // random
        rdAmountSelector.gameObject.SetActive(isRandom);
        numbRd.gameObject.SetActive(isRandom);
        numbRd.text = DataMgr.Instance.RdTestCaseAmount.ToString();
    }

    public void RefreshPickupAmountText()
    {
        // update text
        numbTest.text = DataMgr.Instance.GetTestingDataVals().Count.ToString();
    }
}
