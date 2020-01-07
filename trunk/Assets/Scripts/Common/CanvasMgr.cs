using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMgr : Singleton<CanvasMgr>
{
    private Vector2 refreshCanvasDt = new Vector2(0, 0.5f);
    public List<Board> boards = new List<Board>();

    // ========================================= GET/ SET =========================================
    public Board GetBoard<T>()
    {
        for (int i = 0; i < boards.Count; i++)
            if (boards[i] is T)
                return boards[i];
        return null;
    }

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;       
    }

    void Start()
    {
        // init data
        DataMgr.Instance.Init();

        // init boards
        for (int i = 0; i < boards.Count; i++)
        {
            Board rootPanel = boards[i];
            rootPanel.Init();
        }

        // init result window
        ResultWindow.Instance.Init();

        // refresh canvas
        RefreshCanvas();
    }

    private void Update()
    {
        if (refreshCanvasDt.x > 0)
        {
            refreshCanvasDt.x -= Time.deltaTime;
            if (refreshCanvasDt.x <= 0)
                refreshCanvasDt.x = 0;

            var v = GetComponentsInChildren<VerticalLayoutGroup>();
            foreach (var comp in v)
            {
                comp.enabled = false;
                comp.enabled = true;
            }
            var h = GetComponentsInChildren<HorizontalLayoutGroup>();
            foreach (var comp in h)
            {
                comp.enabled = false;
                comp.enabled = true;
            }
            var c = GetComponentsInChildren<ContentSizeFitter>();
            foreach (var comp in c)
            {
                comp.enabled = false;
                comp.enabled = true;
            }
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public bool IsRefreshCanvas()
    {
        return refreshCanvasDt.x > 0;
    }

    public void RefreshCanvas()
    {
        refreshCanvasDt.x = refreshCanvasDt.y;
    }

    public void OnExitBtnPress()
    {
        Application.Quit();

        // Save data
        //DataMgr.Instance.Save();
    }

    public void Load()
    {
        // load all of boards
        foreach (Board board in boards)
            board.Load();

        // load result window
        ResultWindow.Instance.Load();

        // refresh canvas
        RefreshCanvas();
    }
}
