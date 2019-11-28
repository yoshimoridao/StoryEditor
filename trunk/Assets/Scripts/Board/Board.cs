using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board>
{
    public Label titlePanel;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        instance = this;
    }

    void Update()
    {

    }

    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init()
    {
        instance = this;

        // init title
        titlePanel.Init();
    }
}
