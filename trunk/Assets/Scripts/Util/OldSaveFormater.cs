using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OldSaveFormater
{
    public int genKey = 0;

    // test cases mode
    public bool isRdTest = true;
    // amount of random test cases
    public int rdTestCaseAmount = 1;
    // list test cases
    public List<string> testCaseIds = new List<string>();

    // toolbar
    public int normalFontSize = 20;
}

[System.Serializable]
public class OldDataIndexV001
{
    public string genKey;
    public string title;
    public int colorId;
    public List<string> elements = new List<string>();
    public List<int> testElements = new List<int>();
}

[System.Serializable]
public class OldSaveFormaterV001 : OldSaveFormater
{
    [SerializeField]
    public List<OldDataIndexV001> elements = new List<OldDataIndexV001>();
    [SerializeField]
    public List<OldDataIndexV001> stories = new List<OldDataIndexV001>();
}

[System.Serializable]
public class OldDataIndexV002
{
    public string genKey;
    public string title;
    public string rgbaColor;
    public List<string> elements = new List<string>();
    public List<int> testElements = new List<int>();
}

[System.Serializable]
public class OldSaveFormaterV002 : OldSaveFormater
{
    [SerializeField]
    public List<OldDataIndexV002> elements = new List<OldDataIndexV002>();
    [SerializeField]
    public List<OldDataIndexV002> stories = new List<OldDataIndexV002>();
}
