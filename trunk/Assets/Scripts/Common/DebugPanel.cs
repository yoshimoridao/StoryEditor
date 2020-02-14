using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    public bool isTestReactLabel = true;
    [SerializeField]
    private Text debugText;

    public void ShowDebugRectTransform(RectTransform _rt)
    {
        if (!debugText)
            return;

        string content = "";
        content += "name = " + _rt.gameObject.name + "\n";
        content += "anchoredPosition_X = " + _rt.anchoredPosition.x + "\n";
        content += "anchoredPosition_Y = " + _rt.anchoredPosition.y + "\n";
        content += "anchorMax_X = " + _rt.anchorMax.x + "\n";
        content += "anchorMax_Y = " + _rt.anchorMax.y + "\n";
        content += "anchorMin_X = " + _rt.anchorMin.x + "\n";
        content += "anchorMin_Y = " + _rt.anchorMin.y + "\n";
        content += "sizeDelta_X = " + _rt.sizeDelta.x + "\n";
        content += "sizeDelta_Y = " + _rt.sizeDelta.y + "\n";
        content += "position_X = " + _rt.position.x + "\n";
        content += "position_Y = " + _rt.position.y + "\n";
        content += "localPosition_X = " + _rt.localPosition.x + "\n";
        content += "localPosition_Y = " + _rt.localPosition.y + "\n";

        debugText.text += content;
    }

    public void Update()
    {

        debugText.text = "mousepos_X = " + Input.mousePosition.x + "\n";
        debugText.text += "mousepos_Y = " + Input.mousePosition.y + "\n";

        var rayCast = GetRayCastResultsByMousePos();
        for (int i = 0; i < rayCast.Count; i++)
        {
            var ray = rayCast[i];
            if (ray.gameObject)
            {
                if (isTestReactLabel)
                {
                    if (ray.gameObject.GetComponent<StoryLabel>())
                    {
                        ShowDebugRectTransform(ray.gameObject.transform as RectTransform);
                        break;
                    }
                }
                else
                {
                    ShowDebugRectTransform(ray.gameObject.transform as RectTransform);
                    break;
                }
            }
        }
    }

    public void ClearPlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }

    private List<RaycastResult> GetRayCastResultsByMousePos()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // cast all obj 
        List<RaycastResult> ray = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, ray);

        return ray;
    }
}
