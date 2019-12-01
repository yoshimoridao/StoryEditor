using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public enum BoardType { Element, Story, Origin };
    public BoardType boardType;
    public Label titlePanel;

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
        // init title
        titlePanel.Init(titlePanel.GetText());
    }
}
