using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public enum BoardType { Element, Story, Result };
    public BoardType boardType;

    public Transform transPanelCont;
    public Transform transPlusPanel;

    protected GameObject prefPanel;
    protected List<Panel> panels = new List<Panel>();

    // ========================================= GET/ SET =========================================
    public Panel GetPanel(string _key)
    {
        return panels.Find(x => x.Genkey == _key);
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnDestroy()
    {
        foreach (Panel panel in panels)
        {
            // clear all register action
            if (panel && panel.actOnDestroy != null)
                panel.actOnDestroy -= RemovePanel;
        }
    }
    // ========================================= PUBLIC FUNCS =========================================
    public virtual void Init()
    {
        // clear all template panel
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            if (transPanelCont.GetChild(i) != transPlusPanel)
                Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }

    public virtual void Load()
    {

    }

    public virtual Panel AddPanel(DataIndex _dataIndex) { return null; }
    public virtual void RemovePanel(Panel _panel)
    {
        int panelId = panels.FindIndex(x => x.Genkey == _panel.Genkey);
        // remove panel
        if (panelId > -1 && panelId < panels.Count)
        {
            panels.RemoveAt(panelId);

            // remove in data storage
            DataMgr.Instance.RemoveData(boardType == BoardType.Element ? DataIndexer.DataType.Element : DataIndexer.DataType.Story, _panel.Genkey);
            // also refresh canvas
            GameMgr.Instance.RefreshCanvas();
        }
    }

    public void OnAddBtnPressed()
    {
        // generate data index first
        DataIndex genDataIndex = DataMgr.Instance.AddData(boardType == BoardType.Element ? DataIndexer.DataType.Element : DataIndexer.DataType.Story);
        // set default title
        genDataIndex.Title = DataDefine.default_name_story_panel;

        // generate panel
        Panel genPanel = AddPanel(genDataIndex);

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }

    public void ClearAllTestCases()
    {
        foreach (Panel ePanel in panels)
        {
            if (ePanel.IsTesting)
                ePanel.IsTesting = false;
        }
    }
}
