using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultZoneMgr : MonoBehaviour
{
    public Transform transCont;
    public Text resultText;
    public Sprite rdResultImg;
    public Sprite pickingResultImg;

    private List<GameObject> rows = new List<GameObject>();
    private GameObject prefResultRow;

    private List<DataTagGroup> activeTagGroups = new List<DataTagGroup>();

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {

    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        // load prefab
        prefResultRow = Resources.Load<GameObject>(DataDefine.pref_path_result_row);

        // delete template obj
        for (int i = 0; i < transCont.childCount; i++)
            Destroy(transCont.GetChild(i).gameObject);
    }

    public void ShowResult(Panel panel)
    {

    }

    public void ShowResult(List<string> testCases, bool isRdMode)
    {
        if (!prefResultRow || !transCont)
            return;

        // clear all old tags
        activeTagGroups.Clear();

        // generate new rows
        if (rows.Count < testCases.Count)
        {
            int turn = testCases.Count - rows.Count;
            for (int i = 0; i < turn; i++)
                rows.Add(Instantiate(prefResultRow, transCont));
        }
        // show using row || hide un-used rows
        else
        {
            for (int i = 0; i < rows.Count; i++)
                rows[i].SetActive(i < testCases.Count);
        }

        // show each of stories
        for (int i = 0; i < testCases.Count; i++)
        {
            DataIndexer.DataType dataType;
            DataIndex dataIndex = DataMgr.Instance.FindData(testCases[i], false, out dataType);

            // get result content of the data index
            if (dataIndex == null)
                continue;

            // list contains elements of this sentence
            List<string> _eventTagsInSentence = new List<string>();

            string val = TextUtil.AddBoldColorTag(dataIndex.RGBAColor, dataIndex.title);
            val += " = " + ParseToText(_eventTagsInSentence, dataIndex, dataType, false);

            if (i < rows.Count)
            {
                GameObject row = rows[i];
                // change content of text
                row.GetComponentInChildren<Text>().text = val;

                // change icon for result row
                for (int j = 0; j < row.transform.childCount; j++)
                {
                    Image rowImg = row.transform.GetChild(j).GetComponent<Image>();
                    if (rowImg && rdResultImg && pickingResultImg)
                    {
                        rowImg.sprite = isRdMode ? rdResultImg : pickingResultImg;
                        break;
                    }
                }
            }
        }

        GameMgr.Instance.RefreshCanvas();
    }

    public void ClearContent()
    {
        foreach (GameObject row in rows)
        {
            row.GetComponentInChildren<Text>().text = "";
            row.SetActive(false);
        }
    }

    // ========================================= PRIVATE FUNCS =========================================
    private string ParseToText(List<string> _eventTagsInSentence, DataIndex _dataIndex, DataIndexer.DataType _dataType, bool _isRefer)
    {
        if (_dataIndex == null)
            return "";

        string val = "";
        // get value of Story (merge all childs to text)
        if (_dataType == DataIndexer.DataType.Story)
        {
            val += DataMgr.Instance.MergeAllElements(_dataIndex);
        }
        // pick value of Element
        else
        {
            string elementCont = GetContentOfElement(_eventTagsInSentence, _dataIndex);
            if (elementCont.Length > 0)
                val += elementCont;
        }

        // replace value of reference elements
        bool isContainLinkObj = false;
        char[] splitters = { '#' };
        string[] aSplit = val.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < aSplit.Length; i++)
        {
            DataIndexer.DataType eType;
            DataIndex eIndex = DataMgr.Instance.FindData(aSplit[i], false, out eType);
            if (eIndex != null)
            {
                isContainLinkObj = true;
                string ePart = ParseToText(_eventTagsInSentence, eIndex, eType, true);
                val = val.Replace("#" + aSplit[i] + "#", ePart);
            }
        }

        // add bold tag && color tag for refer object
        if (_isRefer && !isContainLinkObj && (_dataIndex.RGBAColor != Color.white))
        {
            //val = TextUtil.OpenBoldTag() + val + TextUtil.CloseBoldTag();
            //val = TextUtil.OpenColorTag((ColorBar.ColorType)dataIndex.Color) + val + TextUtil.CloseColorTag();

            val = TextUtil.AddBoldColorTag(_dataIndex.RGBAColor, val);
        }

        // replace escape character of value
        val = TextUtil.ReplaceEscapeCharacter(val);

        return val;
    }

    private string GetContentOfElement(List<string> _eventTagsInSentence, DataIndex _dataIndex)
    {
        string val = "";
        List<DataElementIndex> pickedElements = new List<DataElementIndex>();
        bool isFindElementBelongGroup = false;

        // pick elements of tag flows
        if (DataMgr.Instance.IsActiveGrammarTest)
            pickedElements = GetElementsBaseOnFlow(_eventTagsInSentence, _dataIndex);
        // pick element base on using tag groups
        if (DataMgr.Instance.IsActiveGroupTest && pickedElements.Count == 0 && activeTagGroups.Count > 0)
        {
            pickedElements = GetElementsOfGroupTag(_dataIndex, activeTagGroups);
            isFindElementBelongGroup = true;
        }
        // pick testing child elements
        if (pickedElements.Count == 0)
            pickedElements = _dataIndex.GetTestElements();
        // pick all child elements
        if (pickedElements.Count == 0)
            pickedElements = _dataIndex.elements;

        // random value
        if (pickedElements.Count > 0)
        {
            DataElementIndex pickElement = pickedElements[Random.Range(0, pickedElements.Count)];

            val = pickElement.value;

            // find group belong element
            if (!isFindElementBelongGroup)
                CheckAvailGroupForTag(pickElement);

            // append tags
            List<string> tagKeys = pickElement.GetEventTagKeys();
            if (tagKeys.Count > 0)
            {
                _eventTagsInSentence.AddRange(tagKeys);
            }
        }

        return val;
    }


    private void CheckAvailGroupForTag(DataElementIndex _dataElement)
    {
        List<string> tagKeys = _dataElement.GetEventTagKeys();
        for (int i = 0; i < tagKeys.Count; i++)
        {
            string tagKey = tagKeys[i];
            // find tag group which tag is belong
            List<DataTagGroup> taggroups = DataMgr.Instance.GetGroupsOfTag(tagKey);

            // add to active tag groups
            if (taggroups.Count > 0)
            {
                foreach (var taggroup in taggroups)
                    if (!activeTagGroups.Contains(taggroup))
                        activeTagGroups.Add(taggroup);
            }
        }
    }

    /// <summary>
    /// To get elements which contain tags are in tag GROUP
    /// </summary>
    /// <param name="_tagGroups">all of using tag groups</param>
    /// <returns></returns>
    private List<DataElementIndex> GetElementsOfGroupTag(DataIndex _dataIndex, List<DataTagGroup> _tagGroups)
    {
        List<DataElementIndex> vals = new List<DataElementIndex>();
        for (int i = 0; i < _dataIndex.elements.Count; i++)
        {
            var tmpElement = _dataIndex.elements[i];
            // get all mark tags of element
            List<string> eventTagKeys = tmpElement.GetEventTagKeys();

            // check these tags are in using any tag group
            foreach (string tagKey in eventTagKeys)
            {
                // skip unavailable testing tag
                if (!DataMgr.Instance.IsTestingTag(tagKey))
                    continue;

                if (_tagGroups.FindIndex(x => x.IsContainTag(tagKey)) != -1)
                {
                    vals.Add(tmpElement);
                    break;
                }
            }
        }

        return vals;
    }

    private List<DataElementIndex> GetElementsBaseOnFlow(List<string> _eventTagsInSentence, DataIndex _dataIndex)
    {
        List<DataElementIndex> availElemnt = new List<DataElementIndex>();
        for (int i = 0; i < _dataIndex.elements.Count; i++)
        {
            var elemnt = _dataIndex.elements[i];

            bool isAddElemnt = false;
            // check these tags are in of any flows
            List<string> tagKeys = elemnt.GetEventTagKeys();
            foreach (string tagKey in tagKeys)
            {
                // check any flows available for current tag key
                List<DataTagFlow> flowsContainTag = DataMgr.Instance.GetFlowsContainTag(tagKey);
                foreach (var flow in flowsContainTag)
                {
                    if (IsEventTagOfTagFlow(_eventTagsInSentence, tagKey, flow))
                    {
                        availElemnt.Add(elemnt);
                        isAddElemnt = true;
                        break;
                    }
                }
                if (isAddElemnt)
                    break;
            }
        }

        return availElemnt;
    }

    private bool IsEventTagOfTagFlow(List<string> _tagsInSentence, string _tag, DataTagFlow _flowContainTag)
    {
        if (_tagsInSentence.Count == 0)
            return false;

        // get condition flow of the tag
        List<string> conditionTags = _flowContainTag.GetConditionOfTag(_tag);
        if (conditionTags == null)
            return false;

        if (conditionTags.Count == 0)
            return false;

        foreach (var tag in conditionTags)
        {
            // if miss any condition -> false
            if (!_tagsInSentence.Contains(tag))
                return false;
        }

        return true;
    }
}
