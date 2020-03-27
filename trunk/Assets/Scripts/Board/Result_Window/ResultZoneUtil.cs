using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ResultZoneUtil
{
    private static List<DataTagGroup> activeTagGroups = new List<DataTagGroup>();

    public static void ClearActiveTagGroups()
    {
        // clear all old tags
        activeTagGroups.Clear();
    }

    public static string ParseToText(List<string> _eventTagsInSentence, DataIndex _dataIndex, DataIndexer.DataType _dataType, bool _isRefer, bool _isAddColor = true)
    {
        if (_dataIndex == null)
            return "";

        string val = "";
        // get value of Story (merge all childs to text)
        if (_dataType == DataIndexer.DataType.Story)
        {
            val += DataMgr.MergeAllElements(_dataIndex);
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
                string ePart = ParseToText(_eventTagsInSentence, eIndex, eType, true, _isAddColor);
                val = val.Replace("#" + aSplit[i] + "#", ePart);
            }
        }

        // add bold tag && color tag for refer object
        if (_isAddColor && _isRefer && !isContainLinkObj && (_dataIndex.RGBAColor != Color.white))
        {
            //val = TextUtil.OpenBoldTag() + val + TextUtil.CloseBoldTag();
            //val = TextUtil.OpenColorTag((ColorBar.ColorType)dataIndex.Color) + val + TextUtil.CloseColorTag();

            val = TextUtil.AddBoldColorTag(_dataIndex.RGBAColor, val);
        }

        // replace escape character of value
        val = TextUtil.ReplaceEscapeCharacter(val);

        return val;
    }

    private static string GetContentOfElement(List<string> _eventTagsInSentence, DataIndex _dataIndex)
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


    private static void CheckAvailGroupForTag(DataElementIndex _dataElement)
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
    private static List<DataElementIndex> GetElementsOfGroupTag(DataIndex _dataIndex, List<DataTagGroup> _tagGroups)
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

    private static List<DataElementIndex> GetElementsBaseOnFlow(List<string> _eventTagsInSentence, DataIndex _dataIndex)
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

    private static bool IsEventTagOfTagFlow(List<string> _tagsInSentence, string _tag, DataTagFlow _flowContainTag)
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
