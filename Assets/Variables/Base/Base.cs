using System;
using UnityEngine;

public abstract class Base<T> : ScriptableObject, ISetValue<T>  , IGetValue<T>
{
    [SerializeField] private T value;
    public virtual T GetValue(T value)
    {
        return value;
        //Debug.Log();
    }

    public virtual void SetValue(T value)
    {
        //Debug.Log();
    }
}
