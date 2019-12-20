using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultBoard : Board
{
    public ResultZoneMgr resultZone;
    public Transform randomModePanel;
    public InputField rdCaseAmountText;
    // pick-up panel
    public Transform pickingModePanel;
    public InputField pickupAmountText;

    private int rdCaseAmount = 2;
    [SerializeField]
    private bool isRandom = true;

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

        // default de-active random mode
        isRandom = true;
        // active panel
        ActivePanel(isRandom);
    }

    public void OnOriginBtnPress()
    {
        if (!resultZone)
            return;

        List<string> testCases = new List<string>();
        if (isRandom)
        {
            // clone list data
            List<DataIndex> tmp = new List<DataIndex>(DataMgr.Instance.Stories());
            int turn = Mathf.Min(rdCaseAmount, tmp.Count);

            bool isRdIndex = tmp.Count > rdCaseAmount;
            for (int i = 0; i < turn; i++)
            {
                int id = isRdIndex ? Random.Range(0, tmp.Count) : i;
                if (id >= 0 && id < tmp.Count)
                {
                    // add test case for storage
                    testCases.Add(tmp[id].genKey);
                    // remove out of temp list
                    if (isRdIndex)
                        tmp.RemoveAt(id);
                }
            }
        }
        else
        {
            testCases = DataMgr.Instance.GetTestCases();
        }

        resultZone.ShowResult(testCases, isRandom);
    }

    // temp
    public void ShowResult(string text) { }
    // end temp

    public void RefreshPickupAmountText()
    {
        // update text
        if (pickupAmountText)
            pickupAmountText.text = DataMgr.Instance.GetTestCases().Count.ToString();
    }

    // ====== Event Button ======
    public void OnSwitchButtonPress()
    {
        // default de-active random mode
        isRandom = !isRandom;
        // active panel
        ActivePanel(isRandom);
    }

    // = Picking up panel =
    public void OnArrowButtonPress(bool isPlusArrow)
    {
        // update amount of random test cases
        rdCaseAmount += isPlusArrow ? 1 : -1;
        if (rdCaseAmount < 1)
            rdCaseAmount = 1;

        if (rdCaseAmountText)
            rdCaseAmountText.text = rdCaseAmount.ToString();

        // refresh random mode text
        RefreshRdAmountText();
    }

    public void OnClearAllBtnPress()
    {
        // disable all selected tag
        StoryBoard storyBoard = CanvasMgr.Instance.GetBoard<StoryBoard>() as StoryBoard;
        storyBoard.ClearAllPickedTestPanels();
        ElementBoard elementBoard = CanvasMgr.Instance.GetBoard<ElementBoard>() as ElementBoard;
        elementBoard.ClearAllPickedTestPanels();

        // clear all in data
        DataMgr.Instance.ClearTestCases();
    }

    // = Random mode panel =
    public void OnEditRdAmountTextDone()
    {
        rdCaseAmount = int.Parse(rdCaseAmountText.text);
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void ActivePanel(bool isActiveRandom)
    {
        isRandom = isActiveRandom;

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
            rdCaseAmountText.text = rdCaseAmount.ToString();
    }
}
