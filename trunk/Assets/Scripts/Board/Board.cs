﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public enum BoardType { Element, Story, Origin, Result };
    public BoardType boardType;
    public ScrollRect scrollRect;

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

    // ========================================= ACTIVATE =========================================
    public void ActiveScrollRect(bool isActive)
    {
        if (scrollRect)
            scrollRect.enabled = isActive;
    }
}
