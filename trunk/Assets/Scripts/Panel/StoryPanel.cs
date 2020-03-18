using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : Panel
{
    // ========================================= GET/ SET =========================================

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }
    
    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(DataIndex _dataIndex)
    {
        // load prefab label
        prefLabel = Resources.Load<GameObject>(DataDefine.pref_path_story_label);

        base.Init(_dataIndex);

        // Load (color, index,...)
        //DataIndex dataIndex = DataMgr.Instance.GetData(dataType, _key);
        //if (dataIndex != null)
        //{
        //    RGBAColor = dataIndex.GetColor();
        //}

        //// refresh position of add button
        //RefreshAddButtonPos();
    }

    public override void OnAddButtonPress()
    {
        // auto add space for story element
        var labels = Labels;
        if (labels.Count > 0 && labels[labels.Count - 1].PureText != " ")
            AddSpaceMark();

        base.OnAddButtonPress();
    }

    public override void OnChildLabelEdited(Label _label)
    {
        // auto add space mark if last character = space mark
        //string labelVal = _label.PureText;
        //if (labelVal.Length > 0 && labelVal[labelVal.Length - 1] == ' ')
        //{
        //}

        base.OnChildLabelEdited(_label);
    }

    // ========================================= PRIVATE FUNCS =========================================
    private void AddSpaceMark()
    {
        AddLabel(" ");
    }
}
