using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementPanel : Panel
{
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
    public override void Init(DataIndex _dataIndex)
    {
        // load prefab label
        prefLabel = Resources.Load<GameObject>(DataDefine.pref_path_element_label);

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

    // ========================================= PRIVATE FUNCS =========================================
}
