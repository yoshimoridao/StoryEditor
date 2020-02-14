using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataConfig : Singleton<DataConfig>
{
    public Color normalLabelColor;
    public Color spaceMarkLabelColor;

    private void Awake()
    {
        instance = this;
    }
}
