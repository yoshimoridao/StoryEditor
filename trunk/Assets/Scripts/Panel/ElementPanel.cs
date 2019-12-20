using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementPanel : Panel
{
    //private List<Label> testingLabels = new List<Label>();

    // ========================================= GET/ SET =========================================
    //public List<Label> GetTestingLabels() { return testingLabels; }

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
    public override void Init(Board _board, string _key, string _title)
    {
        base.Init(_board, _key, _title);

        // Load (color, index,...)
        DataIndex dataIndex = DataMgr.Instance.GetData(dataType, _key);
        if (dataIndex != null)
        {
            Color = ((ColorBar.ColorType)dataIndex.colorId);
        }

        // refresh position of add button
        RefreshAddButtonPos();
    }
}
