using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intellisence : Singleton<Intellisence>
{
    private List<string> linkDict = new List<string>();
    private List<string> tagDict = new List<string>();
    [SerializeField]
    private GameObject window;
    [SerializeField]
    private Transform transCont;
    private GameObject templateText;

    [SerializeField]
    private InputField editingField;
    private string oldText;

    // ========================================= UNITY FUNCS =========================================
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // default hide window
        SetActiveWindow(false);

        if (transCont && transCont.childCount > 0)
        {
            templateText = transCont.GetChild(0).gameObject;
            templateText.transform.parent = null;
        }

        linkDict.Add("environment");
        linkDict.Add("character");

        tagDict.Add("env_ocean");
        tagDict.Add("env_savannah");
        tagDict.Add("evt_hikeandseek");
        tagDict.Add("evt_explore");
    }

    private void Update()
    {
        if (!window || !IsActiveWindow())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            GameObject handleObj = CursorMgr.Instance.GetHandleObjOnTop();
            // if user select input field
            if (handleObj && handleObj.GetComponent<InputField>())
            {
                if (handleObj.GetComponent<InputField>() != editingField)
                    editingField = null;
            }
            else if (editingField)
            {
                editingField = null;
            }
        }

        // de active window
        if (IsActiveWindow())
        {
            if (editingField == null || Input.GetKeyDown(KeyCode.Escape))
                SetActiveWindow(false);
        }
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void OnSelectField(Text _text)
    {

    }

    public void OnSelectInputField(InputField _field)
    {
        // check store selecting field
        if (!editingField || (editingField && editingField.gameObject != _field.gameObject))
        {
            // if select another field
            if (editingField)
            {
                editingField.onValueChanged.RemoveListener(OnTextFieldChangeVal);

                // turn off window if it's activating
                if (IsActiveWindow())
                    SetActiveWindow(false);
            }

            editingField = _field;
            editingField.onValueChanged.AddListener(OnTextFieldChangeVal);
        }

        if (editingField)
            oldText = editingField.text;
    }

    public void OnTextFieldChangeVal(string _val)
    {
        //char[] splitter = { ' ' };
        //var splits = _val.Split(splitter);
        //if (splits.Length > 0 && splits[splits.Length - 1].Length > 0 && splits[splits.Length - 1][0] == '@')
        //{
        //    tmpActiveWindow = true;
        //}

        if (_val != oldText)
        {
            bool tmpActiveWindow = false;

            // if player type any text except space
            if (!Input.GetKeyDown(KeyCode.Space))
            {
                var splitTexts = _val.Split(' ');
                var splitOlds = oldText.Split(' ');

                for (int i = 0; i < splitTexts.Length; i++)
                {
                    // detect difference character
                    if (i >= splitOlds.Length || splitTexts[i].Length != splitOlds[i].Length)
                    {
                        if (splitTexts[i].Length > 0 && splitTexts[i][0] == '@')
                            tmpActiveWindow = true;

                        break;
                    }
                }
            }

            oldText = _val;
            SetActiveWindow(tmpActiveWindow);
        }
    }

    public bool IsActiveWindow()
    {
        if (window)
            return window.active;

        return false;
    }

    private void SetActiveWindow(bool _isActive)
    {
        if (window && window.active == _isActive)
            return;

        window.SetActive(_isActive);

        if (_isActive && editingField)
            window.transform.position = editingField.transform.position;
    }
}
