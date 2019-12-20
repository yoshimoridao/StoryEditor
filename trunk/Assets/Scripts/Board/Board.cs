using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public enum BoardType { Element, Story, Origin, Result };
    public BoardType boardType;

    // ========================================= GET/ SET =========================================

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init()
    {
    }
}
