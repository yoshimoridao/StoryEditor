using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IFloatingWindow
{
    List<GameObject> SelectedObjs { get; set; }
    bool IsActiveWindow { get; set; }
    Action ActOnWindowDisable { get; set; }

    void SetActiveWindow(bool _isActive, List<GameObject> _selectObjs);
}
