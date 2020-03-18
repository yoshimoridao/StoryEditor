using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagEditorFieldCont : Singleton<TagEditorFieldCont>
{
    private GameObject prefTagEditorField;
    [SerializeField]
    private Transform transCont;
    [SerializeField]
    private Button addTagBtn;

    private List<TagEditorField> tagFields = new List<TagEditorField>();


    private int maxFieldWidth;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnDestroy()
    {
        if (addTagBtn)
            addTagBtn.onClick.RemoveListener(AddNewEventTag);
    }

    public void Init()
    {
        // load prefab
        if (prefTagEditorField == null)
            prefTagEditorField = Resources.Load<GameObject>(DataDefine.pref_tag_editor_field);

        // register button event
        addTagBtn.onClick.AddListener(AddNewEventTag);

        // get max size of tag field
        maxFieldWidth = (int)(transform as RectTransform).sizeDelta.x - 40; // 40: offset of transconts
        VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();
        if (layoutGroup)
            maxFieldWidth = maxFieldWidth - (layoutGroup.padding.left + layoutGroup.padding.right);

        // delete template of container
        if (transCont)
        {
            for (int i = 0; i < transCont.childCount; i++)
            {
                Transform child = transCont.GetChild(i);
                if (child.GetComponent<TagEditorField>())
                    Destroy(transCont.GetChild(i).gameObject);
            }
        }

        Load();
    }

    public void Load()
    {
        // load event tag id
        List<EventTagId> tagIds = DataMgr.Instance.GetEventTags();
        for (int i = 0; i < tagIds.Count; i++)
        {
            EventTagId tagId = tagIds[i];
            TagEditorField tagField = null;

            // generate tag fields
            if (i >= tagFields.Count)
            {
                tagField = GenTagField(tagId);
            }
            else
            {
                tagField = tagFields[i];
                // set new tag id for existed field
                tagField.TagId = tagId;
            }
        }

        // remove sur-plus tag fields
        if (tagFields.Count > tagIds.Count)
        {
            for (int i = tagIds.Count; i < tagFields.Count; i++)
            {
                if (RemoveTagField(i))
                    i--;
            }
        }

        // refresh add btn position
        RefreshAddBtnPos();
    }

    private TagEditorField GenTagField(EventTagId _tagId)
    {
        if (prefTagEditorField == null)
            return null;

        TagEditorField genTagField = GameObject.Instantiate(prefTagEditorField, transCont).GetComponent<TagEditorField>();
        if (genTagField)
        {
            // init generated field
            genTagField.Init(_tagId, maxFieldWidth);

            // store in manager list
            tagFields.Add(genTagField);
            return genTagField;
        }

        return null;
    }

    public void AddNewEventTag()
    {
        // gen new tag in data mgr
        EventTagId newTagId = DataMgr.Instance.AddEventTag(DataDefine.default_event_tag_value);

        GenTagField(newTagId);
        // refresh position of add btn
        RefreshAddBtnPos();

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }

    public void RemoveEventTag(EventTagId _tagId)
    {
        if (_tagId != null)
        {
            // remove in data mgr
            DataMgr.Instance.RemoveEventTag(_tagId.genKey);

            // remove object
            RemoveTagField(_tagId);
        }
    }
    private bool RemoveTagField(EventTagId _tagId)
    {
        int findId = tagFields.FindIndex(x => x.TagId.genKey == _tagId.genKey);
        if (findId != -1)
        {
            RemoveTagField(findId);
            return true;
        }
        return false;
    }
    private bool RemoveTagField(int _index)
    {
        if (_index < tagFields.Count)
        {
            Destroy(tagFields[_index].gameObject);
            tagFields.RemoveAt(_index);
            return true;
        }
        return false;
    }

    private void RefreshAddBtnPos()
    {
        // refresh position of add btn
        addTagBtn.transform.SetAsLastSibling();
    }
}
