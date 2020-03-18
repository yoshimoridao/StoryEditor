using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagEditorMgr : Singleton<TagEditorMgr>
{
    [SerializeField]
    private TagEditorFieldCont tagEditorFieldCont;
    [SerializeField]
    private TagEditorWorkSpace tagEditorWorkSpace;

    private bool isInit = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (!isInit)
            Init();
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (isInit)
            Load();
    }

    public void ExitEditor()
    {
        GameMgr.Instance.OpenEditor(GameMgr.EditorType.StoryEditor);
    }

    public void Init()
    {
        isInit = true;

        // init
        if (tagEditorFieldCont != null)
            tagEditorFieldCont.Init();

        if (tagEditorWorkSpace != null)
            tagEditorWorkSpace.Init();

        // init cursor
        CursorTagEditor.Instance.Init();

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }

    public void Load()
    {
        // current do not need to load these content
        if (tagEditorFieldCont != null)
            tagEditorFieldCont.Load();

        if (tagEditorWorkSpace != null)
            tagEditorWorkSpace.Load();

        // load cursor
        CursorTagEditor.Instance.Load();

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }
}
