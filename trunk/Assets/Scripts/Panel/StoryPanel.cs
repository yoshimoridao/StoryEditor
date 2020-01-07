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
    public override void Init(string _key, string _title)
    {
        // load prefab label
        prefLabel = Resources.Load<GameObject>(DataDefine.pref_path_story_label);

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
}
