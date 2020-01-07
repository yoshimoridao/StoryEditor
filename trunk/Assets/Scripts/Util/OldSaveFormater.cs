using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OldDataIndex
{
    public string key;
    [SerializeField]
    public List<string> elements = new List<string>();
    public int colorId;
    public bool isStoryElement = false;
    [SerializeField]
    public List<int> testingIndex = new List<int>();
}

[System.Serializable]
public class OldSaveFormater
{
    [SerializeField]
    public List<OldDataIndex> elements = new List<OldDataIndex>();
    [SerializeField]
    public List<OldDataIndex> stories = new List<OldDataIndex>();
}
