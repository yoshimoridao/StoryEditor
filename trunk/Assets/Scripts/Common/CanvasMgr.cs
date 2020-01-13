using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GracesGames.SimpleFileBrowser.Scripts;

public class CanvasMgr : Singleton<CanvasMgr>
{
    public List<Board> boards = new List<Board>();

    private Vector2 refreshCanvasDt = new Vector2(0, 0.5f);
    [SerializeField]
    private FileBrowserCaller fileBrowserCaller;

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

        // init tool bar
        ToolbarMgr.Instance.Init();

        // init notice bar
        NoticeBarMgr.Instance.Init();

        // init popup mgr
        PopupMgr.Instance.Init();

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
    public void OpenSaveBrowserAndExit()
    {
        if (fileBrowserCaller)
            fileBrowserCaller.OpenFileBrowser(true, ExitApp);
    }

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
        // show popup confirm exit
        if (DataMgr.Instance.IsModified)
            PopupMgr.Instance.ShowPopup(PopupMgr.PopupType.SAVE);

        //Application.Quit();

        // Save data
        //DataMgr.Instance.Save();
    }

    public void ExitApp()
    {
        Application.Quit();
    } 

    public void Load()
    {
        // load all of boards
        foreach (Board board in boards)
            board.Load();

        // load result window
        ResultWindow.Instance.Load();

        // load tool bar
        ToolbarMgr.Instance.Load();

        // load notice bar
        NoticeBarMgr.Instance.Load();

        // init popup mgr
        PopupMgr.Instance.Load();

        // refresh canvas
        RefreshCanvas();
    }
}
