using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowLabelMgr : MonoBehaviour
{
    GameObject prefInputLabel;
    GameObject prefLinkLabel;

    CommonPanel contParent;
    List<Label> lLabels = new List<Label>();

    // ========================================= GET/ SET FUNCS =========================================
    public Panel GetParent()
    {
        return contParent;
    }

    public int ChildCount()
    {
        return lLabels.Count;
    }

    public List<Label> GetLabels()
    {
        return lLabels;
    }

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
    }

    void Update()
    {

    }

    public void Init(CommonPanel labelCont)
    {
        contParent = labelCont;

        // load prefabs
        prefInputLabel = Resources.Load<GameObject>(DataConfig.prefInputLabelPath);
        prefLinkLabel = Resources.Load<GameObject>(DataConfig.prefLinkLabelPath);

        // add template first row
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    // ========================================= PUBLIC FUNCS =========================================
    public void AddInputLabel()
    {
        if (prefInputLabel)
        {
            // gen new label
            InputLabel label = Instantiate(prefInputLabel, transform).GetComponent<InputLabel>();
            label.Init(this);
            lLabels.Add(label);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }
    public void AddLinkLabel(CommonPanel referPanel)
    {
        if (prefInputLabel)
        {
            // gen new label
            LinkLabel label = Instantiate(prefLinkLabel, transform).GetComponent<LinkLabel>();
            label.Init(this, referPanel);
            lLabels.Add(label);

            // refresh canvas
            CanvasMgr.Instance.RefreshCanvas();
        }
    }

    public void AddLabelAsFirst(Label label)
    {
        // set parent for label object  (transform)
        label.SetParent(this, true);

        // add new label at first index (in storage)
        if (lLabels.Count > 0)
        {
            Label temp = lLabels[0];
            lLabels[0] = label;
            lLabels.Add(temp);
        }
        else
        {
            lLabels.Add(label);
        }
    }

    public Label RetrieveLastLabel()
    {
        if (lLabels.Count == 0)
            return null;

        int lastId = lLabels.Count - 1;

        Label lastLabel = lLabels[lastId];
        lastLabel.SetParent(null);  // remove from parent
        lLabels.RemoveAt(lastId);   // remove in storage

        return lastLabel;
    }

    // ========== INPUT LABEL ==========
    public void OnChildLabelEditDone()
    {
        // call event to parent
        if (contParent)
            (contParent as CommonPanel).OnChildLabelEditDone();
    }
}
