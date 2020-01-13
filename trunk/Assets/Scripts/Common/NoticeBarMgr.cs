using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeBarMgr : Singleton<NoticeBarMgr>
{
    [SerializeField]
    private float showFieldTime = 3;

    [SerializeField]
    private InputField inputField;
    private float showFieldDt = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (showFieldDt > 0)
        {
            showFieldDt -= Time.deltaTime;

            if (showFieldDt <= 0)
            {
                showFieldDt = 0;
                gameObject.SetActive(false);
            }
        }
    }

    public void Init()
    {
        Load();
    }

    public void Load()
    {
        gameObject.SetActive(false);
    }

    public void ShowText(string _val)
    {
        // active obj
        gameObject.SetActive(true);

        inputField.text = _val;
        showFieldDt = showFieldTime;
    }
}
