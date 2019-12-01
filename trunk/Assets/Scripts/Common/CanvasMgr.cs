using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMgr : Singleton<CanvasMgr>
{
    private Vector2 refreshCanvasDt = new Vector2(0, 0.5f);

    public List<Board> lRootPanel = new List<Board>();

    // ========================================= GET/ SET =========================================
    public Board GetBoard<T>()
    {
        for (int i = 0; i < lRootPanel.Count; i++)
            if (lRootPanel[i] is T)
                return lRootPanel[i];
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

        // init root panel
        for (int i = 0; i < lRootPanel.Count; i++)
        {
            Board rootPanel = lRootPanel[i];
            rootPanel.Init();
        }

        DataMgr.Instance.InitElement();

        RefreshCanvas();
    }

    private void Update()
    {
        if (refreshCanvasDt.x > 0)
        {
            refreshCanvasDt.x -= Time.deltaTime;
            if (refreshCanvasDt.x <= 0)
                refreshCanvasDt.x = 0;

            var arr = GetComponentsInChildren<VerticalLayoutGroup>();
            foreach (var layout in arr)
            {
                layout.enabled = false;
                layout.enabled = true;
            }
            var harr = GetComponentsInChildren<HorizontalLayoutGroup>();
            foreach (var layout in harr)
            {
                layout.enabled = false;
                layout.enabled = true;
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
}
