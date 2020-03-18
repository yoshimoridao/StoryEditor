using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectElement
{
    Color originColor { get; }
    void OnSelect();
    void OnEndSelect();
}
