using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class EventTagId
{
    public string genKey;
    public string value;

    public EventTagId()
    {
    }

    public EventTagId(EventTagId _copy)
    {
        genKey = _copy.genKey;
        value = _copy.value;
    }
}
