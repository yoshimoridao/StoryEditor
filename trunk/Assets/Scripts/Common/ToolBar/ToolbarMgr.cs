using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarMgr : Singleton<ToolbarMgr>
{
    public FontSizeButton fontSizeButton;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ==================================== PUBLIC ====================================
    public void Init()
    {
        Load();
    }

    public void Load()
    {
        fontSizeButton.Init();
    }
}
