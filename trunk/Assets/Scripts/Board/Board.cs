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
        return panels.Find(x => x.Key == _key);
    }

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
        // clear all template panel
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            if (transPanelCont.GetChild(i) != transPlusPanel)
                Destroy(transPanelCont.GetChild(i).gameObject);
        }
    }

    public virtual Panel AddPanel(string _genKey) { return null; }
    public virtual void RemovePanel(Panel _panel)
    {
        int panelId = panels.FindIndex(x => x.Key == _panel.Key);
        // remove panel
        if (panelId > -1 && panelId < panels.Count)
        {
            panels.RemoveAt(panelId);

            // remove in data storage
            DataMgr.Instance.RemoveData(boardType == BoardType.Element ? DataIndexer.DataType.Element : DataIndexer.DataType.Story, _panel.Key);
            // also refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void OnAddBtnPressed()
    {
        // get generation key from data
        string genKey = DataMgr.Instance.GenNewKey();
        Panel genPanel = AddPanel(genKey);

        if (genPanel)
        {
            // save
            DataMgr.Instance.AddData(boardType == BoardType.Element ? DataIndexer.DataType.Element : DataIndexer.DataType.Story, genPanel);
            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    //public void ClearAllPickedTestPanels()
    //{
    //    // get list of picked up panels from data
    //    List<string> pickedPanel = DataMgr.Instance.GetTestCases();
    //    // un-pick these panels
    //    for (int i = 0; i < panels.Count; i++)
    //    {
    //        Panel panel = panels[i] as Panel;
    //        // if panel in list
    //        if (pickedPanel.FindIndex(x => x == panel.Title()) != -1 && panel.testTag)
    //        {
    //            panel.testTag.SetActiveTag(false);
    //        }
    //    }
    //}
}
