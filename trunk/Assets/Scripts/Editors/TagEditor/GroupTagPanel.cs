using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupTagPanel : MonoBehaviour, IDragZone
{
    [SerializeField]
    private InputField title;
    [SerializeField]
    private GameObject prefTagRow;
    [SerializeField]
    private GameObject prefTagElement;
    [SerializeField]
    private Transform transCont;
    [SerializeField]
    private Button delBtn;

    public Action<GameObject> actOnDestroyPanel;

    // data
    private DataTagGroup dataTagGroup;

    private List<Transform> rows = new List<Transform>();
    List<TagElement> tagElements = new List<TagElement>();

    // logic
    private RectTransform rt;
    private VerticalLayoutGroup vLayoutGroup;

    public bool IsDragIn { get; set; }
    public Color originColor { get; set; }

    public DataTagGroup DataGroup
    {
        get { return dataTagGroup; }
    }

    void Start()
    {
        originColor = GetComponent<Image>().color;
    }

    void Update()
    {

    }

    #region interface
    public void OnMouseIn(GameObject obj)
    {
        if (obj.GetComponent<TagEditorField>())
        {
            IsDragIn = true;
            GetComponent<Image>().color = DataDefine.highlight_drop_zone_color;
        }
    }

    public void OnMouseOut()
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;

        GetComponent<Image>().color = originColor;
    }

    public void OnMouseDrop(GameObject obj)
    {
        if (!IsDragIn)
            return;

        IsDragIn = false;
        GetComponent<Image>().color = originColor;

        if (obj.GetComponent<TagEditorField>())
        {
            var tagField = obj.GetComponent<TagEditorField>();
            AddTag(tagField.TagId, true);
        }
    }
    #endregion

    #region common
    public void Init(DataTagGroup _data)
    {
        if (rt == null)
            rt = (transform as RectTransform);
        if (vLayoutGroup == null)
            vLayoutGroup = GetComponent<VerticalLayoutGroup>();

        // delete all child
        for (int i = 0; i < transCont.childCount; i++)
            Destroy(transCont.GetChild(i));

        // register listener for button
        if (delBtn)
            delBtn.onClick.AddListener(delegate { DestroySelf(true); });

        // load data tag group
        SetData(_data);
    }

    public void DestroySelf(bool _isInvokeCallback = true)
    {
        if (_isInvokeCallback && actOnDestroyPanel != null)
            actOnDestroyPanel.Invoke(gameObject);

        Destroy(gameObject);
    }

    public void LoadContentPanel()
    {
        if (dataTagGroup == null)
            return;

        char[] splitters = { '@' };
        var tmpKeys = dataTagGroup.val.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
        List<EventTagId> eventTagIds = new List<EventTagId>();
        foreach (string tagKey in tmpKeys)
        {
            EventTagId eventTagId = DataMgr.Instance.GetEventTag("@" + tagKey);
            if (eventTagId != null)
                eventTagIds.Add(eventTagId);
        }

        for (int i = 0; i < eventTagIds.Count; i++)
        {
            EventTagId eventTagId = eventTagIds[i];
            // gen new tag id
            if (i >= tagElements.Count)
            {
                AddTag(eventTagId, false);
            }
            // set tag id for existed tag
            else
            {
                tagElements[i].TagId = eventTagId;
            }
        }

        // destroy sur-plus tag elements
        if (tagElements.Count > eventTagIds.Count)
        {
            for (int i = eventTagIds.Count; i < tagElements.Count; i++)
            {
                if (RemoveTag(i))
                    i--;
            }
        }
    }
    #endregion

    // ======= Element Tag =======
    #region element_tag
    public void AddTag(EventTagId _tagId, bool _isUpdateData)
    {
        // gen new element
        Transform lastRow = GetLastRow();
        if (lastRow)
        {
            TagElement genTagElement = Instantiate(prefTagElement, lastRow).GetComponent<TagElement>();
            if (genTagElement)
            {
                // init new element and
                genTagElement.Init(_tagId);
                // register event
                genTagElement.actOnDestroyElement += RemoveTag;
                genTagElement.actOnElementChangeSize += OnOrderElements;

                // store generated element
                tagElements.Add(genTagElement);

                // order elements
                OnOrderElements();

                // update data's value
                if (_isUpdateData)
                    UpdateDataVal();
            }
        }
    }

    public void RemoveTag(GameObject _element)
    {
        int findId = tagElements.FindIndex(x => x.gameObject == _element.gameObject);
        if (findId != -1)
        {
            RemoveTag(findId);

            // update data's value
            UpdateDataVal();
            // order elements
            OnOrderElements();
        }
    }

    public bool RemoveTag(int _id)
    {
        if (_id < tagElements.Count)
        {
            // un-register event
            TagElement tagElement = tagElements[_id];
            if (tagElement.actOnDestroyElement != null)
                tagElement.actOnDestroyElement -= RemoveTag;
            if (tagElement.actOnElementChangeSize != null)
                tagElement.actOnElementChangeSize -= OnOrderElements;

            //// destroy obj
            //Destroy(tagElement.gameObject);

            // remove out of manage list
            tagElements.RemoveAt(_id);
            return true;
        }

        return false;
    }
    #endregion

    // ======= Data =======
    #region data
    public void SetData(DataTagGroup _val)
    {
        dataTagGroup = _val;

        // change title
        if (title)
        {
            title.text = dataTagGroup.title;
            // register action for title
            title.onEndEdit.AddListener(OnTitleEditEnd);
        }

        // load content for panel
        LoadContentPanel();
    }

    private void UpdateDataVal()
    {
        if (dataTagGroup == null)
            return;

        string dataVal = "";
        foreach (TagElement element in tagElements)
            dataVal += element.TagId.genKey;

        dataTagGroup.val = dataVal;
    }
    #endregion

    // ======= Title =======
    #region title
    public void OnTitleEditEnd(string _val)
    {
        if (dataTagGroup != null)
            dataTagGroup.title = _val;
    }
    #endregion

    // ======= Row =======
    #region row
    private Transform GetLastRow()
    {
        // add new row if empty
        if (rows.Count == 0)
            AddNewRow();

        if (rows.Count > 0)
            return rows[rows.Count - 1];

        return null;
    }

    private Transform AddNewRow()
    {
        if (prefTagRow)
        {
            Transform row = Instantiate(prefTagRow, transCont).transform;
            // destroy all child template
            for (int i = 0; i < row.GetChildCount(); i++)
                Destroy(row.GetChild(i).gameObject);

            // store label cont
            rows.Add(row);
            return row;
        }

        return null;
    }

    public void OnOrderElements()
    {
        float rowLength = 0;
        int rowId = 0;
        for (int i = 0; i < tagElements.Count; i++)
        {
            RectTransform eRt = (tagElements[i].transform as RectTransform);

            // break down
            if (rowLength + eRt.sizeDelta.x + vLayoutGroup.spacing >= rt.sizeDelta.x)
            {
                rowLength = 0;
                rowId++;
            }

            rowLength += eRt.sizeDelta.x + vLayoutGroup.spacing;
            // update row index
            Transform r = null;
            if (rowId >= rows.Count)
                r = AddNewRow();
            else
                r = rows[rowId];
            // append element as last child of row
            eRt.parent = r;
            eRt.SetAsLastSibling();
        }

        GameMgr.Instance.RefreshCanvas();
    }
    #endregion
}
