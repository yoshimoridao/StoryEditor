using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleLabel : Label
{
    // ========================================= PROPERTIES =========================================
    public override string PureText
    {
        get { return pureText; }
        set
        {
            pureText = value;
            inputField.text = pureText;
        }
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        base.Start();
    }

    public void Update()
    {
        base.Update();
    }

    // ========================================= PUBLIC FUNCS =========================================
    public override void Init(Panel _panel, string _text)
    {
        base.Init(_panel, _text);
    }

    public override void OnEditDone()
    {
        if (!isEditing)
            return;

        pureText = inputField.text;

        base.OnEditDone();

        //if (panel)
        //    panel.OnTitleEdited();
    }
}
