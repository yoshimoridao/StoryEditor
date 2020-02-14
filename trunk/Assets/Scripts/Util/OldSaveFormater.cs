using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OldDataIndex
{
    public string genKey;
    public string title;
    public int colorId;
    public List<string> elements = new List<string>();
    public List<int> testElements = new List<int>();
}

[System.Serializable]
public class OldSaveFormater
{
    public int genKey = 0;
    [SerializeField]
    public List<OldDataIndex> elements = new List<OldDataIndex>();
    [SerializeField]
    public List<OldDataIndex> stories = new List<OldDataIndex>();

    // test cases mode
    public bool isRdTest = true;
    // amount of random test cases
    public int rdTestCaseAmount = 1;
    // list test cases
    public List<string> testCaseIds = new List<string>();

    // toolbar
    public int normalFontSize = 20;
}
