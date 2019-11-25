using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelMgr : MonoBehaviour
{
    public ContentSizeFitter contentSize;
    public Vector2 offset = new Vector2(5, 5);    // pixel

    // prop
    RectTransform rt;
    [SerializeField]
    Vector2 prevSizeDelta = Vector2.zero;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        
    }

    void Update()
    {
    }

    public void OnEditDone()
    {
        // if (rt.sizeDelta != prevSizeDelta)
        {
            contentSize.enabled = false;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x + offset.x, rt.sizeDelta.y + offset.y);
            // prevSizeDelta = rt.sizeDelta;
            Debug.Log("resize done");
        }
    }

    public void OnChangeValue()
    {
        if (!contentSize)
            return;
            
        contentSize.enabled = false;
        contentSize.enabled = true;
        // prevSizeDelta = rt.sizeDelta;

        Debug.Log("resize");
    }
}
