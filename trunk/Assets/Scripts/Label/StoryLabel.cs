using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryLabel : ReactLabel
{
    // ========================================= PROPERTIES =========================================

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

    public override void RefreshContentSize()
    {
        base.RefreshContentSize();

        // set size & color for SPACE content only
        if (inputField && inputField.text == " ")
        {
            if (rt)
                rt.sizeDelta = Vector2.one * Mathf.Min(rt.sizeDelta.x, rt.sizeDelta.y);
            if (image)
                image.color = DataConfig.Instance.spaceMarkLabelColor;
        }
        else
        {
            // set size & color for not SPACE content
            if (image)
                image.color = DataConfig.Instance.normalLabelColor;
        }
    }
}
