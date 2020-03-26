using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GracesGames.SimpleFileBrowser.Scripts;

public class GameMgr : Singleton<GameMgr>
{
    // --- window editor ---
    public Transform cvStoryEditor;
    public Transform cvTagEditor;
    public enum EditorType { StoryEditor, TagEditor }

    private EditorType curEditorType = EditorType.StoryEditor;
    private Transform curEditor;

    // --- Story
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

    public Transform CurEditor
    {
        get { return curEditor; }
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

        // default open Story Editor
        OpenEditor(EditorType.StoryEditor);

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
        //EventTagMgr.Instance.Init();
        TagPanelMgr.Instance.Init();

        // init notice bar
        NoticeBarMgr.Instance.Init();

        // init popup mgr
        PopupMgr.Instance.Init();
        DisplayMgr.Instance.Init();

        // init cursor
        CursorMgr.Instance.Init();

        // init tag editor
        if (TagEditorMgr.Instance)
            TagEditorMgr.Instance.Init();

        // refresh canvas
        RefreshCanvas();
    }

    private void Update()
    {
        if (refreshCanvasDt.x > 0 && curEditor != null)
        {
            refreshCanvasDt.x -= Time.deltaTime;
            if (refreshCanvasDt.x <= 0)
                refreshCanvasDt.x = 0;

            var v = curEditor.GetComponentsInChildren<VerticalLayoutGroup>();
            foreach (var comp in v)
            {
                comp.enabled = false;
                comp.enabled = true;
            }
            var h = curEditor.GetComponentsInChildren<HorizontalLayoutGroup>();
            foreach (var comp in h)
            {
                comp.enabled = false;
                comp.enabled = true;
            }
            var c = curEditor.GetComponentsInChildren<ContentSizeFitter>();
            foreach (var comp in c)
            {
                comp.enabled = false;
                comp.enabled = true;
            }
        }
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
        //EventTagMgr.Instance.Load();
        TagPanelMgr.Instance.Load();
        // load notice bar
        NoticeBarMgr.Instance.Load();

        // load popup mgr
        PopupMgr.Instance.Load();
        DisplayMgr.Instance.Load();

        // load cursor
        CursorMgr.Instance.Load();

        // refresh canvas
        RefreshCanvas();
    }

    public void ExitApp()
    {
        Application.Quit();

        // clear last file path after exit application (dont cache last save file)
        if (PlayerPrefs.HasKey(DataDefine.save_key_last_load_file))
            PlayerPrefs.DeleteKey(DataDefine.save_key_last_load_file);
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

    public void OpenEditor(EditorType _editorType)
    {
        Transform nextEditor = null;
        // open story editor
        if (_editorType == EditorType.StoryEditor && cvStoryEditor)
        {
            curEditorType = _editorType;
            nextEditor = cvStoryEditor;
        }
        // open tag editor
        if (_editorType == EditorType.TagEditor && cvTagEditor)
        {
            curEditorType = _editorType;
            nextEditor = cvTagEditor;
        }

        if (nextEditor != null)
        {
            // disable old editor
            if (curEditor != null)
                curEditor.gameObject.SetActive(false);

            // show next editor
            curEditor = nextEditor;
            nextEditor.gameObject.SetActive(true);
        }
    }

    // =================== Story Editor ===================
    public void OpenSaveBrowser()
    {
        if (fileBrowserCaller)
            fileBrowserCaller.OpenFileBrowser(true);
    }

    public void OpenSaveBrowser(System.Action _callback)
    {
        if (fileBrowserCaller)
            fileBrowserCaller.OpenFileBrowser(true, _callback);
    }

    public void OpenSaveBrowserAndExit()
    {
        if (fileBrowserCaller)
            fileBrowserCaller.OpenFileBrowser(true, ExitApp);
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

    public void ClearAllTestCases()
    {
        for (int i = 0; i < boards.Count; i++)
        {
            boards[i].ClearAllTestCases();
        }
    }
}
