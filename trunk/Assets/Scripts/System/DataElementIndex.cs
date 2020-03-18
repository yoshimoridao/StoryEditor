using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class DataElementIndex
{
    public string value = "";        // -> key
    public bool isTest = false;
    // this mark using key of event tags (template: "@1,@2,@3","@4,@5,@6",...)
    public string eventTagKeys = "";

    public DataElementIndex()
    {
    }

    // ===== tag key =====
    public List<string> GetEventTagKeys()
    {
        char[] splitters = { ',' };
        return new List<string>(eventTagKeys.Split(splitters, StringSplitOptions.RemoveEmptyEntries));
    }

    public bool IsContainEventTag(string _key)
    {
        return eventTagKeys.Contains(_key);
    }

    public void AddEventTag(string _key)
    {
        if (!IsContainEventTag(_key))
        {
            // add comma
            if (eventTagKeys.Length > 0)
                eventTagKeys += ",";

            eventTagKeys += _key;
        }
    }

    public void RemoveEventTag(string _key)
    {
        if (IsContainEventTag(_key))
        {
            
            char[] splitters = { ',' };
            var splitText = eventTagKeys.Replace(_key, "").Split(splitters, StringSplitOptions.RemoveEmptyEntries);

            string newEventTagKeys = "";
            foreach (string tagKey in splitText)
            {
                if (tagKey.Length > 0)
                    newEventTagKeys += tagKey + ",";
            }

            // remove comma at last postion
            if (newEventTagKeys.Length > 0)
                newEventTagKeys = newEventTagKeys.Substring(0, newEventTagKeys.Length - 1);

            eventTagKeys = newEventTagKeys;
        }
    }
}
