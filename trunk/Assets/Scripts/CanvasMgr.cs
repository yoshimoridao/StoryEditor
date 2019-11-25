using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMgr : MonoBehaviour
{
    public static float refreshCanvasTime = 1.0f;

    private static float refreshCanvasDt;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        RefreshCanvas();
    }

    private void Update()
    {
        if (refreshCanvasDt > 0)
        {
            refreshCanvasDt -= Time.deltaTime;
            if (refreshCanvasDt <= 0)
                refreshCanvasDt = 0;

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
    public static void RefreshCanvas()
    {
        refreshCanvasDt = refreshCanvasTime;
    }
}
