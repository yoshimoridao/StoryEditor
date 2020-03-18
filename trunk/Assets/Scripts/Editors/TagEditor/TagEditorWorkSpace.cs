using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagEditorWorkSpace : MonoBehaviour, IDragZone
{
    public enum EditorType { Flow, Group };
    [SerializeField]
    private Transform transCont;
    [SerializeField]
    private Transform rootCont;

    [SerializeField]
    private GameObject prefTagGroupPanel;
    [SerializeField]
    private GameObject prefTagFlowPanel;

    [SerializeField]
    private Button groupBtn;
    [SerializeField]
    private Button flowBtn;
    [SerializeField]
    private Color activeBtnColor;
    [SerializeField]
    private Color deactiveBtnColor;
    [SerializeField]
    private Vector2 btnHeightRange = Vector2.zero;

    // main data
    private DataTagRelation data;

    // store all tag groups
    private List<GroupTagPanel> groupPanels = new List<GroupTagPanel>();
    private List<FlowTagPanel> flowPanels = new List<FlowTagPanel>();

    // current editor 
    private EditorType curEditorType = EditorType.Group;
    private RectTransform rt;

    public Color originColor { get; set; }

    void Start()
    {
        originColor = rootCont.GetComponent<Image>().color;

        groupBtn.onClick.AddListener(delegate { OnSwitchBtnPressed(EditorType.Group); });
        flowBtn.onClick.AddListener(delegate { OnSwitchBtnPressed(EditorType.Flow); });
    }

    void Update()
    {
    }

    #region interface
    public void OnMouseIn(GameObject obj)
    {
        if (obj.GetComponent<TagEditorField>())
            rootCont.GetComponent<Image>().color = DataDefine.highlight_drop_zone_color;
    }

    public void OnMouseOut()
    {
        rootCont.GetComponent<Image>().color = originColor;
    }

    public void OnMouseDrop(GameObject obj)
    {
        if (obj.GetComponent<TagEditorField>())
        {
            var tagField = obj.GetComponent<TagEditorField>();
            OnMouseDragAddPanel(tagField.TagId);
        }
    }
    #endregion

    #region common
    public void Init()
    {
        if (rt == null)
            rt = (transform as RectTransform);

        // clear all childs in content
        for (int i = 0; i < transCont.childCount; i++)
            Destroy(transCont.GetChild(i).gameObject);

        Load();
    }

    public void Load()
    {
        // load new data
        if (DataMgr.Instance.GetDataTagRelation() != null)
            data = DataMgr.Instance.GetDataTagRelation();

        // clear all tags before load new
        ClearAllTags();
        // load properly data for type
        if (curEditorType == EditorType.Group)
            LoadGroups();
        else if (curEditorType == EditorType.Flow)
            LoadFlowTags();
    }

    private void ClearAllTags()
    {
        foreach (var group in groupPanels)
            group.DestroySelf(false);
        // clear all list
        groupPanels.Clear();

        foreach (var flow in flowPanels)
            flow.DestroySelf(false);
        // clear all list
        flowPanels.Clear();
    }

    #region event
    public void OnSwitchBtnPressed(EditorType _type)
    {
        if (_type == curEditorType)
            return;

        curEditorType = _type;

        // clear all tags before load new
        ClearAllTags();

        // load properly data for type
        if (curEditorType == EditorType.Group)
            LoadGroups();
        else if (curEditorType == EditorType.Flow)
            LoadFlowTags();

        // set height btn
        Vector2 btnSize = (groupBtn.transform as RectTransform).sizeDelta;
        btnSize.y = curEditorType == EditorType.Group ? btnHeightRange.y : btnHeightRange.x;
        (groupBtn.transform as RectTransform).sizeDelta = btnSize;
        // set color btn
        groupBtn.GetComponent<Image>().color = curEditorType == EditorType.Group ? activeBtnColor : deactiveBtnColor;

        // set height btn
        btnSize = (flowBtn.transform as RectTransform).sizeDelta;
        btnSize.y = curEditorType == EditorType.Flow ? btnHeightRange.y : btnHeightRange.x;
        (flowBtn.transform as RectTransform).sizeDelta = btnSize;
        // set color btn
        flowBtn.GetComponent<Image>().color = curEditorType == EditorType.Flow ? activeBtnColor : deactiveBtnColor;

        // refresh canvas
        GameMgr.Instance.RefreshCanvas();
    }

    public void OnMouseDragAddPanel(EventTagId _tagId)
    {
        if (curEditorType == EditorType.Group)
            AddGroupPanel(_tagId);
        else if (curEditorType == EditorType.Flow)
            AddFlowPanel(_tagId);
    }
    #endregion

    #endregion

    // ================================ FLOW ================================
    #region flow_panel
    private void LoadFlowTags()
    {
        for (int i = 0; i < data.flows.Count; i++)
        {
            // load data for flow
            DataTagFlow dataFlow = data.flows[i];
            if (dataFlow == null)
                continue;

            // generate flow panels
            if (i >= flowPanels.Count)
            {
                FlowTagPanel genFlowPanel = AddEmptyFlowTag();
                genFlowPanel.Init(dataFlow);
            }
            // replace data for existed flow panel
            else
            {
                flowPanels[i].SetData(dataFlow);
            }
        }

        // remove surplus panels
        for (int i = data.flows.Count; i < flowPanels.Count; i++)
        {
            if (RemoveFlowPanel(i))
                i--;
        }
    }

    private FlowTagPanel AddFlowPanel(EventTagId _tagId)
    {
        // gen new tag group
        FlowTagPanel genFlowTag = AddEmptyFlowTag();
        if (genFlowTag != null)
        {
            // gen new default data
            DataTagFlow dataFlowTag = data.AddFlowTag(DataDefine.default_new_flow_tag);

            // init data for generated flow tag
            genFlowTag.Init(dataFlowTag);
            // add fst group
            genFlowTag.AddGroup(_tagId, true);
        }

        GameMgr.Instance.RefreshCanvas();
        return genFlowTag;
    }

    private FlowTagPanel AddEmptyFlowTag()
    {
        if (curEditorType != EditorType.Flow)
            return null;

        // gen new tag flow
        FlowTagPanel genFlowTag = Instantiate(prefTagFlowPanel, transCont).GetComponent<FlowTagPanel>();
        if (genFlowTag == null)
            return null;

        // register action
        genFlowTag.actOnDestroyPanel += OnFlowTagDestroyed;
        // store new flow
        flowPanels.Add(genFlowTag);

        return genFlowTag;
    }

    public void OnFlowTagDestroyed(GameObject _panel)
    {
        int findId = flowPanels.FindIndex(x => x.gameObject == _panel.gameObject);
        if (findId == -1)
            return;

        // remove out of data
        data.RemoveFlowTag(flowPanels[findId].Data.genKey);

        // remove out of manage list
        RemoveFlowPanel(findId);
    }

    public bool RemoveFlowPanel(int _id)
    {
        if (_id >= flowPanels.Count)
            return false;

        // un-register action
        FlowTagPanel flowPanel = flowPanels[_id];
        if (flowPanel.actOnDestroyPanel != null)
            flowPanel.actOnDestroyPanel -= OnFlowTagDestroyed;

        flowPanels.RemoveAt(_id);
        return true;
    }
    #endregion

    // ================================ GROUP ================================
    #region group_panel
    private void LoadGroups()
    {
        for (int i = 0; i < data.groups.Count; i++)
        {
            // load data for group
            DataTagGroup dataGroup = data.groups[i];
            if (dataGroup == null)
                continue;

            // generate group panels
            if (i >= groupPanels.Count)
            {
                GroupTagPanel genTagGroup = AddEmptyGroupPanel();
                // init group
                genTagGroup.Init(dataGroup);
            }
            // replace data for existed group panel
            else
            {
                groupPanels[i].SetData(dataGroup);
            }
        }

        // remove surplus panels
        for (int i = data.groups.Count; i < groupPanels.Count; i++)
        {
            if (RemoveGroupPanel(i))
                i--;
        }
    }

    private void AddGroupPanel(EventTagId _tagId)
    {
        // gen new tag group
        GroupTagPanel genTagGroup = AddEmptyGroupPanel();
        if (genTagGroup != null)
        {
            // gen default data for new group
            DataTagGroup dataTagGroup = data.AddTagGroup(DataDefine.default_new_group_tag);
            // init group
            genTagGroup.Init(dataTagGroup);
            // add element for tag group
            genTagGroup.AddTag(_tagId, true);
        }

        GameMgr.Instance.RefreshCanvas();
    }

    private GroupTagPanel AddEmptyGroupPanel()
    {
        if (curEditorType == EditorType.Group)
        {
            // gen new tag group
            GroupTagPanel genTagGroup = Instantiate(prefTagGroupPanel, transCont).GetComponent<GroupTagPanel>();
            //register action
            genTagGroup.actOnDestroyPanel += OnGroupPanelDestroyed;

            // store new group
            groupPanels.Add(genTagGroup);

            return genTagGroup;
        }

        return null;
    }

    public void OnGroupPanelDestroyed(GameObject _panel)
    {
        int findId = groupPanels.FindIndex(x => x.gameObject == _panel.gameObject);
        if (findId == -1)
            return;

        // remove out of data
        data.RemoveTagGroup(groupPanels[findId].DataGroup.genKey);

        // remove out of manage list
        RemoveGroupPanel(findId);
    }

    public bool RemoveGroupPanel(int _id)
    {
        if (_id < groupPanels.Count)
        {
            // un-register action
            GroupTagPanel groupPanel = groupPanels[_id];
            if (groupPanel.actOnDestroyPanel != null)
                groupPanel.actOnDestroyPanel -= OnGroupPanelDestroyed;

            groupPanels.RemoveAt(_id);
            return true;
        }
        return false;
    }
    #endregion
}
