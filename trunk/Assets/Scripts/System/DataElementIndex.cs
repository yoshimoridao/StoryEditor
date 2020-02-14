using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DataElementIndex
{
    public string value;
    public bool isTest;
    // this mark using key of event tags (template: "@1,@2,@3","@4,@5,@6",...)
    public string eventTagKeys;

    public DataElementIndex() { }

    public List<string> GetTagKeys ()
    {
        List<string> tagKeys = new List<string>(eventTagKeys.Split('@'));
        return tagKeys;
    }
}
