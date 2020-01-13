using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAbleElement : MonoBehaviour
{
    public Label titleLabel;

    // ========================================= UNITY FUNCS =========================================
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ========================================= PUBLIC FUNCS =========================================
    public Label GetLabelObj()
    {
        if (titleLabel)
            return titleLabel;

        return null;
    }
}
