using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementPanel : CommonPanel
{
    private List<Label> testingLabels = new List<Label>();

    // ========================================= GET/ SET =========================================
    public List<Label> GetTestingLabels() { return testingLabels; }

    // ========================================= UNITY FUNCS =========================================
    public void Start()
    {
        base.Start();
    }

    public void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(Board board, string key)
    {
        base.Init(board, key);
    }

    public void AddTestingLabel(Label label)
    {
        // find label is the child of the panel
        if (!testingLabels.Contains(label))
            testingLabels.Add(label);

        // save testing index
        SaveTestingLabel();
    }

    public void RemoveTestingLabel(Label label)
    {
        if (testingLabels.Contains(label))
            testingLabels.Remove(label);

        // save testing index
        SaveTestingLabel();
    }

    // ========================================= PRIVATE FUNCS =========================================
    public void LoadTestingLabel(List<int> testingIndex)
    {
        List<Label> labels = GetLabels();
        for (int i = 0; i < testingIndex.Count; i++)
        {
            int labelId = testingIndex[i];
            if (labelId < labels.Count)
            {
                Label label = labels[labelId];
                // set active highlight for label (also stored the highlight panel)
                label.SetActiveHighlightPanel(true);
            }
        }
    }

    private void SaveTestingLabel()
    {
        List<int> testingIndex = new List<int>();
        List<Label> labels = GetLabels();
        for (int i = 0; i < testingLabels.Count; i++)
        {
            int findId = labels.FindIndex(x => x == testingLabels[i]);
            if (findId != -1)
                testingIndex.Add(findId);
        }

        // save testing index
        if (testingIndex.Count > 0)
            testingIndex.Sort();
        DataMgr.Instance.ReplaceTestingIndex(dataType, GetTitle(), testingIndex);
    }

    public override void OnArrangedLabels()
    {
        base.OnArrangedLabels();

        // save testing index after arrange
        SaveTestingLabel();
    }
}
