using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementPanel : Panel
{
    protected List<ElementLabel> testLabels = new List<ElementLabel>();

    // ========================================= GET/ SET =========================================

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
    public override void Init(string _key, string _title)
    {
        // load prefab label
        prefLabel = Resources.Load<GameObject>(DataDefine.pref_path_element_label);

        base.Init(_key, _title);

        // Load (color, index,...)
        DataIndex dataIndex = DataMgr.Instance.GetData(dataType, _key);
        if (dataIndex != null)
        {
            Color = ((ColorBar.ColorType)dataIndex.colorId);
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }

    public override Label AddLabel(string _var)
    {
        Label genLabel = base.AddLabel(_var);
        if (genLabel && genLabel is ElementLabel)
            (genLabel as ElementLabel).actOnActive += OnLabelActiveTest;

        return genLabel;
    }

    public override void UpdateOrderLabels()
    {
        base.UpdateOrderLabels();

        // save test labels (after order)
        SaveTestLabels();
    }

    public void OnLabelActiveTest(ElementLabel _label)
    {
        if (_label.IsTesting)
            AddTestLabel(_label);
        else
            RemoveTestLabel(_label);

        // set active highlight for all testing labels
        ActiveTestLabels();

        // save test labels
        SaveTestLabels();
    }

    public void ActiveTestLabels()
    {
        foreach (ElementLabel eLabel in labels)
            eLabel.ActiveTesting(testLabels.Contains(eLabel));
    }

    public void AddTestLabel(ElementLabel _label)
    {
        //if (!testLabels.Contains(_label))
        //    testLabels.Add(_label);

        // find label is the child of the panel
        int findId = testLabels.FindIndex(x => x.gameObject == _label.gameObject);
        if (findId == -1)
            testLabels.Add(_label);
    }

    public void RemoveTestLabel(ElementLabel _label)
    {
        //if (testLabels.Contains(_label))
        //    testLabels.Remove(_label);

        int findId = testLabels.FindIndex(x => x.gameObject == _label.gameObject);
        if (findId != -1)
            testLabels.RemoveAt(findId);
    }

    public void ClearTestLabels()
    {
        testLabels.Clear();
    }

    // ========================================= PRIVATE FUNCS =========================================
    protected void SaveTestLabels()
    {
        // find all index of testing labels
        List<int> tmpTestIds = new List<int>();
        for (int i = 0; i < testLabels.Count; i++)
        {
            int findId = labels.FindIndex(x => x.gameObject == testLabels[i].gameObject);
            if (findId != -1)
                tmpTestIds.Add(findId);
        }

        if (tmpTestIds.Count > 0)
            tmpTestIds.Sort();

        // save index of testing labels
        DataMgr.Instance.ReplaceTestingIndex(dataType, Key, tmpTestIds);
    }
}
