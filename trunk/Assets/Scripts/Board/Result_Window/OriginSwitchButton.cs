using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using System;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class OriginSwitchButton : MonoBehaviour
{
    public Vector2 routePos;
    public float lerpScale = 0.5f;

    // overlay layer
    public Image disableLayer;
    public float disableAlpha = 0.5f;
    private float enableAlpha = 0.0f;

    private RectTransform rt;
    private Image img;
    private bool isDoAnim = false;
    private bool isEnable = false;
    private Action actCallBack;

    private void Awake()
    {
        
    }

    void Start()
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (img == null)
            img = GetComponent<Image>();
    }
    
    void Update()
    {
        if (isDoAnim)
        {
            Vector2 pos = rt.anchoredPosition;
            // lerp color
            Color layerColor = disableLayer.color;
            float desAlpha = isEnable ? enableAlpha : disableAlpha;
            layerColor.a = Mathf.Lerp(layerColor.a, desAlpha, lerpScale);

            // lerp pos x
            float desPos = isEnable ? routePos.y : routePos.x;
            pos.x = Mathf.Lerp(pos.x, desPos, lerpScale);
            if (Mathf.Abs(desPos - pos.x) <= 0.1f)
            {
                isDoAnim = false;
                pos.x = desPos;
                layerColor.a = desAlpha;

                // call event
                if (actCallBack != null)
                {
                    actCallBack();
                    // clear action
                    actCallBack = null;
                }
            }

            disableLayer.color = layerColor;
            rt.anchoredPosition = pos;
        }
    }

    public void Init(bool _isEnable)
    {
        if (rt == null)
            rt = GetComponent<RectTransform>();
        if (img == null)
            img = GetComponent<Image>();

        isEnable = _isEnable;
        // set alpha for disable layer
        Color layerColor = disableLayer.color;
        layerColor.a = isEnable ? enableAlpha : disableAlpha;
        disableLayer.color = layerColor;

        // set position
        rt.anchoredPosition = new Vector2(isEnable ? routePos.y : routePos.x, rt.anchoredPosition.y);
    }

    public void SetActive(bool _isEnable, Action _act)
    {
        isDoAnim = true;
        isEnable = _isEnable;

        // add action
        if (_act != null)
            actCallBack += _act;
    }
}
