using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultWindow : Singleton<ResultWindow>
{
    public ResultZoneMgr resultZone;
    public Transform randomModePanel;
    public InputField rdCaseAmountText;
    // pick-up panel
    public Transform pickingModePanel;
    public InputField pickupAmountText;
    public OriginSwitchButton switchPickModeBtn;
    public OriginSwitchButton switchRdModeBtn;

    [SerializeField]
    private bool isRandom = true;
    private int oldRdCaseAmounts = -1;

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

        // add modified action for picking mode
        DataMgr.Instance.ActModifiedTestCase += RefreshPickupAmountText;

        // scale height for all ratio
        float canvasHeight = (CanvasMgr.Instance.transform as RectTransform).sizeDelta.y;
        RectTransform rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (rt.sizeDelta.y / 1080) * canvasHeight);
    }

    public void Load()
    {
        // Load testing mode
        isRandom = DataMgr.Instance.IsRandomTest;

        // active mode btn
        if (switchPickModeBtn)
            switchPickModeBtn.Init(!isRandom);
        if (switchRdModeBtn)
            switchRdModeBtn.Init(isRandom);

        // active panel
        ActivePanel();

        // refresh content
        resultZone.ClearContent();
    }

    public void OnDestroy()
    {
        // add modified action for picking mode
        DataMgr.Instance.ActModifiedTestCase -= RefreshPickupAmountText;
    }

    public void RefreshPickupAmountText()
    {
        // update text
        if (pickupAmountText)
            pickupAmountText.text = DataMgr.Instance.TestCases.Count.ToString();
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
            testCases = DataMgr.Instance.TestCases;
        }

        resultZone.ShowResult(testCases, isRandom);
    }

    public void OnSwitchButtonPress(bool _isRdMode)
    {
        // default de-active random mode
        isRandom = _isRdMode;
        //// active panel
        //ActivePanel();

        // do active switch btn
        if (switchRdModeBtn)
            switchRdModeBtn.SetActive(isRandom, isRandom ? (Action)ActivePanel : null);
        if (switchPickModeBtn)
            switchPickModeBtn.SetActive(!isRandom, isRandom ? null : (Action)ActivePanel);

        // Save current testing mode
        DataMgr.Instance.IsRandomTest = isRandom;
    }

    // = Picking up panel =
    public void OnArrowButtonPress(bool isPlusArrow)
    {
        // update amount of random test cases
        DataMgr.Instance.RdTestCaseAmount += isPlusArrow ? 1 : -1;

        if (rdCaseAmountText)
        {
            rdCaseAmountText.text = DataMgr.Instance.RdTestCaseAmount.ToString();
            oldRdCaseAmounts = int.Parse(rdCaseAmountText.text);
        }

        // refresh random mode text
        RefreshRdAmountText();
    }

    public void OnClearAllBtnPress()
    {
        // disable all testing panels
        Board storyBoard = CanvasMgr.Instance.GetBoard<StoryBoard>();
        Board elementBoard = CanvasMgr.Instance.GetBoard<ElementBoard>();

        List<string> testCaseIds = DataMgr.Instance.TestCases;
        for (int i = 0; i < testCaseIds.Count; i++)
        {
            string testKey = testCaseIds[i];
            // find panel
            Panel testingPanel = storyBoard.GetPanel(testKey);
            if (testingPanel == null)
                testingPanel = elementBoard.GetPanel(testKey);

            // change testing flag
            if (testingPanel)
                testingPanel.IsTesting = false;
        }

        // clear all in data
        DataMgr.Instance.ClearTestCases();
    }

    // = Random mode panel =
    public void OnEditRdAmountTextDone()
    {
        int inputAmount = -1;
        if (rdCaseAmountText && int.TryParse(rdCaseAmountText.text, out inputAmount) && inputAmount != -1)
        {
            DataMgr.Instance.RdTestCaseAmount = inputAmount;
        }
        else
        {
            rdCaseAmountText.text = oldRdCaseAmounts.ToString();
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void ActivePanel()
    {
        // panel of picking mode
        if (pickingModePanel)
        {
            pickingModePanel.gameObject.SetActive(!isRandom);
            // refresh text
            RefreshPickupAmountText();
        }
        // panel of random mode
        if (randomModePanel)
        {
            randomModePanel.gameObject.SetActive(isRandom);
            // refresh text
            RefreshRdAmountText();
        }
    }

    private void RefreshRdAmountText()
    {
        // init text
        if (rdCaseAmountText)
        {
            rdCaseAmountText.text = DataMgr.Instance.RdTestCaseAmount.ToString();
            oldRdCaseAmounts = int.Parse(rdCaseAmountText.text);
        }
    }
}
