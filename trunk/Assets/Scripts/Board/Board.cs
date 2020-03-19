using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField]
    private RectTransform panelviewRt;

    public enum BoardType { Element, Story, Result };
    public BoardType boardType;

    public Transform transPanelCont;
    public Transform transPlusPanel;

    protected GameObject prefPanel;
    protected GameObject prefPanelSpace;

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
            if (panel && panel.actOnChangeSiblingId != null)
                panel.actOnChangeSiblingId -= OnChildChangeSiblingId;
        }
    }
    // ========================================= PUBLIC FUNCS =========================================
    public void Init()
    {
        // load prefab
        string prefPanelPath = (boardType == BoardType.Story) ? DataDefine.pref_path_storyPanel : DataDefine.pref_path_elementPanel;
        prefPanel = Resources.Load<GameObject>(prefPanelPath);
        prefPanelSpace = Resources.Load<GameObject>(DataDefine.pref_path_panel_space);

        // clear all template panel
        for (int i = 0; i < transPanelCont.childCount; i++)
        {
            if (transPanelCont.GetChild(i) != transPlusPanel)
                Destroy(transPanelCont.GetChild(i).gameObject);
        }

        // load data
        Load();

        // scale height for all ratio
        //float canvasHeight = (CanvasMgr.Instance.transform as RectTransform).sizeDelta.y;
        float canvasHeight = (GameMgr.Instance.CurEditor as RectTransform).sizeDelta.y;

        RectTransform rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (rt.sizeDelta.y / 1080) * canvasHeight);
        // scale height for panel's view
        if (panelviewRt)
            panelviewRt.sizeDelta = new Vector2(panelviewRt.sizeDelta.x, (panelviewRt.sizeDelta.y / 1080) * canvasHeight);
    }

    public virtual void Load()
    {

    }

    public Panel AddPanel(DataIndex _dataIndex)
    {
        if (!prefPanel)
            return null;

        // create new panel
        Panel panel = Instantiate(prefPanel, transPanelCont).GetComponent<Panel>();
        if (panel)
        {
            panel.Init(_dataIndex);
            // register action when panel is destroyed
            panel.actOnDestroy += RemovePanel;
            panel.actOnChangeSiblingId += OnChildChangeSiblingId;

            panels.Add(panel);

            // set index of adding element panel as last child
            if (transPlusPanel)
                transPlusPanel.transform.SetAsLastSibling();

            return panel;
        }

        return null;
    }

    public void RemovePanel(Panel _panel)
    {
        int panelId = panels.FindIndex(x => x.Genkey == _panel.Genkey);
        // remove panel
        if (panelId > -1 && panelId < panels.Count)
        {
            panels.RemoveAt(panelId);

            // remove in data storage
            DataMgr.Instance.RemoveData(boardType == BoardType.Element ? DataIndexer.DataType.Element : DataIndexer.DataType.Story, _panel.Genkey);

            // refresh content
            StartCoroutine("RefreshContent");

            // also refresh canvas
            GameMgr.Instance.RefreshCanvas();
        }
    }

    protected IEnumerator RefreshContent()
    {
        List<PanelSpace> spaces = new List<PanelSpace>(transPanelCont.GetComponentsInChildren<PanelSpace>());
        if (panels.Count == 0)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            var genAmount = panels.Count + 1;
            // gen more objs
            if (spaces.Count < genAmount)
            {
                genAmount -= spaces.Count;
                for (int i = 0; i < genAmount; i++)
                {
                    var genSpace = Instantiate(prefPanelSpace, transPanelCont);
                    spaces.Add(genSpace.GetComponent<PanelSpace>());
                }
            }
            // destroy surplus objs
            else if (spaces.Count > genAmount)
            {
                for (int i = genAmount; i < spaces.Count; i++)
                    Destroy(spaces[i].gameObject);

                spaces.RemoveRange(genAmount, spaces.Count - genAmount);
            }

            // order space
            for (int i = 0; i < spaces.Count; i++)
            {
                int siblingId = i * 2;
                //if (i != 0 && i - 1 < panels.Count)
                //    siblingId = panels[i - 1].transform.GetSiblingIndex() + 1;

                spaces[i].transform.SetSiblingIndex(siblingId);
            }
        }
    }

    public void ClearAllTestCases()
    {
        foreach (Panel ePanel in panels)
        {
            if (ePanel.IsTesting)
                ePanel.IsTesting = false;
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
        // refresh content
        StartCoroutine("RefreshContent");

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }

    public void OnChildChangeSiblingId()
    {
        // refresh list panels
        panels = new List<Panel>(transform.GetComponentsInChildren<Panel>());
        // refresh content
        StartCoroutine("RefreshContent");

        DataIndexer.DataType datatype = (boardType == BoardType.Element) ? DataIndexer.DataType.Element : DataIndexer.DataType.Story;
        // refresh in data
        DataMgr.Instance.SortIndexes(datatype, panels);
    }
}
