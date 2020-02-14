using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMgr : Singleton<DisplayMgr>
{
    [SerializeField]
    private GameObject infoPanel;
    [SerializeField]
    private GameObject cheatPanel;

    private bool isEnableCheatPanel = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init()
    {
        Load();
    }

    public void Load()
    {
        ShowInfoPanel(false);
    }

    public void ShowInfoPanel(bool _isActive)
    {
        if (infoPanel)
        {
            infoPanel.SetActive(_isActive);
        }

        CanvasMgr.Instance.RefreshCanvas();
    }

    public void DisplayCheatPanel()
    {
        if (cheatPanel)
        {
            isEnableCheatPanel = !isEnableCheatPanel;
            cheatPanel.SetActive(isEnableCheatPanel);
        }
    }

    public void ToggleValExportFileForGame(Toggle toggle)
    {
        DataMgr.Instance.IsExportGameSave = toggle.isOn;
    }
}
