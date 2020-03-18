using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragElement
{
    Color originColor { get; }
    void OnDragging();
    void OnEndDrag();
}
