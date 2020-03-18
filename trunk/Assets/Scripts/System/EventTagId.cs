using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EventTagId
{
    public string genKey;
    public string value;
    public bool isVisible = true;
    public bool isTest = true;

    public Action actOnModyingData;
    public Action<string> actOnDestroy;

    public string Value
    {
        get { return value; }
        set
        {
            this.value = value;

            if (actOnModyingData != null)
                actOnModyingData.Invoke();
        }
    }

    public bool IsVisible
    {
        get { return isVisible; }
        set
        {
            isVisible = value;

            if (actOnModyingData != null)
                actOnModyingData.Invoke();
        }
    }

    public void OnDestroy()
    {
        if (actOnDestroy != null)
            actOnDestroy.Invoke(genKey);
    }

    public EventTagId()
    {
        // default visible = true
        isVisible = true;
        // default test = true
        isTest = true;
    }
}

[System.Serializable]
public class DataEventTag
{
    public int tagGenKey = 0;
    [SerializeField]
    public DataTagRelation tagRelation = new DataTagRelation();
    [SerializeField]
    public List<EventTagId> eventTagIds = new List<EventTagId>();

    public DataEventTag()
    {
    }

    // ====== Tag ======
    public bool IsTestingTag(string _genKey)
    {
        List<EventTagId> testingEventTags = GetTestingTags();
        if (testingEventTags.FindIndex(x => x.genKey == _genKey) != -1)
            return true;

        return false;
    }

    public List<EventTagId> GetTestingTags()
    {
        List<EventTagId> testingTags = new List<EventTagId>();
        foreach (var eTag in eventTagIds)
        {
            if (eTag.isTest)
                testingTags.Add(eTag);
        }
        return testingTags;
    }

    public EventTagId GetEventTag(string _genKey)
    {
        int findId = eventTagIds.FindIndex(x => x.genKey == _genKey);

        if (findId != -1)
        {
            return eventTagIds[findId];
        }

        return null;
    }

    public EventTagId AddEventTag(string _val)
    {
        // gen new key
        string newKey = "@" + tagGenKey;
        tagGenKey++;

        // gen new tag
        EventTagId genTag = new EventTagId();
        genTag.genKey = newKey;
        genTag.Value = _val;

        eventTagIds.Add(genTag);

        return genTag;
    }

    public void RemoveEventTag(string _genKey)
    {
        int findId = eventTagIds.FindIndex(x => x.genKey == _genKey);

        if (findId != -1)
        {
            eventTagIds[findId].OnDestroy();

            // remove event tag in list
            eventTagIds.RemoveAt(findId);
        }
    }
}



[System.Serializable]
public class DataTagGroup
{
    public string genKey;
    public string title = "";
    // { @1@2@3@4,@5@6,@7@8 }
    public string val = "";

    public DataTagGroup() { }
    public DataTagGroup(string _genKey, string _title)
    {
        genKey = _genKey;
        title = _title;
    }

    public bool IsContainTag(string _tag)
    {
        if (val.Contains(_tag))
            return true;
        return false;
    }
}

[System.Serializable]
public class DataTagFlow
{
    public string genKey;
    public string title = "";
    // { @1@2_@3_@4_@5,@6_@7_@8,... }
    public string val = "";

    public DataTagFlow() { }
    public DataTagFlow(string _genKey, string _title)
    {
        genKey = _genKey;
        title = _title;
    }

    public List<string> GetConditionOfTag(string _tag)
    {
        string[] groupSplitter = { "_" };
        List<string> groups = new List<string>(val.Split(groupSplitter, StringSplitOptions.RemoveEmptyEntries));

        // split condition groups of the tag
        int findId = groups.FindIndex(x => x == _tag);
        if (findId != -1)
        {
            groups.RemoveRange(findId, groups.Count - findId);
        }
        else
        {
            return null;
        }

        // split from groups to tags
        string[] tagSplitter = { "@" };
        List<string> conditionTags = new List<string>();
        foreach (var groupVal in groups)
        {
            string[] tags = groupVal.Split(tagSplitter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in tags)
                conditionTags.Add("@" + tag);
        }

        return conditionTags;
    }

    public bool IsContainTag(string _tag)
    {
        if (val.Contains(_tag))
            return true;
        return false;
    }
}

[System.Serializable]
public class DataTagRelation
{
    public int genKey = 0;
    [SerializeField]
    public List<DataTagGroup> groups = new List<DataTagGroup>();
    [SerializeField]
    public List<DataTagFlow> flows = new List<DataTagFlow>();

    public DataTagRelation()
    {
    }

    #region common
    private string GenNewKey()
    {
        string newKey = "@" + genKey;
        genKey++;
        return newKey;
    }
    #endregion

    #region tag_group
    public DataTagGroup AddTagGroup(string _title)
    {
        DataTagGroup newGroupTag = new DataTagGroup(GenNewKey(), _title);
        groups.Add(newGroupTag);

        return newGroupTag;
    }

    public void RemoveTagGroup(string _genKey)
    {
        int findId = groups.FindIndex(x => x.genKey == _genKey);
        if (findId != -1)
        {
            groups.RemoveAt(findId);
        }
    }

    public List<DataTagGroup> GetGroupsOfTag(string _tag)
    {
        List<DataTagGroup> availGroups = new List<DataTagGroup>();
        for (int i = 0; i < groups.Count; i++)
        {
            DataTagGroup eGroup = groups[i];
            if (eGroup.IsContainTag(_tag))
                availGroups.Add(eGroup);
        }

        return availGroups;
    }
    #endregion

    #region tag_flow
    public List<DataTagFlow> GetFlowsContainTag(string _tag)
    {
        List<DataTagFlow> availFlows = new List<DataTagFlow>();
        for (int i = 0; i < flows.Count; i++)
        {
            DataTagFlow eFlow = flows[i];
            if (eFlow.IsContainTag(_tag))
                availFlows.Add(eFlow);
        }

        return availFlows;
    }

    public DataTagFlow AddFlowTag(string _title)
    {
        DataTagFlow newFlowTag = new DataTagFlow(GenNewKey(), _title);
        flows.Add(newFlowTag);

        return newFlowTag;
    }

    public void RemoveFlowTag(string _genKey)
    {
        int findId = flows.FindIndex(x => x.genKey == _genKey);
        if (findId != -1)
            flows.RemoveAt(findId);
    }

    //public List<DataTagGroup> GetFlowTags(string _tag)
    //{
    //    List<DataTagGroup> availGroups = new List<DataTagGroup>();
    //    for (int i = 0; i < groups.Count; i++)
    //    {
    //        DataTagGroup eGroup = groups[i];
    //        if (eGroup.IsContainTag(_tag))
    //            availGroups.Add(eGroup);
    //    }

    //    return availGroups;
    //}
    #endregion
}
