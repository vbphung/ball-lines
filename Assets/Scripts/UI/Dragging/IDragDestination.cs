using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragDestination<T> where T : class
{
    bool Acceptable(int sourceIndex);
    void AddItems(T item);
}