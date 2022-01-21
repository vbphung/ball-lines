using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragSource<T> where T : class
{
    T GetItem();
    void RemoveItems();
    int GetIndex();
}