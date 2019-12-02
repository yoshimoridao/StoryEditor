using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultBoard : Board
{
    public Text resultText;
    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init()
    {
        base.Init();
    }

    public void ShowResult(string text)
    {
        if (resultText)
        {
            resultText.text = text;
        }
    }
}
