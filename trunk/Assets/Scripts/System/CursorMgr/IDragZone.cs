using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragZone
{
    Color originColor { get; }
    void OnMouseIn(GameObject obj);
    void OnMouseOut();
    void OnMouseDrop(GameObject obj);
}
