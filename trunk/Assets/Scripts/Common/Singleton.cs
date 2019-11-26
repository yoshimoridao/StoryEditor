using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }
}
