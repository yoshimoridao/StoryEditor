using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataStorage
{
    [SerializeField]
    List<string> lElements = new List<string>();
    [SerializeField]
    Dictionary<string, string> dTemp = new Dictionary<string, string>();

    public DataStorage(List<string> val)
    {
        lElements = val;
    }
}
