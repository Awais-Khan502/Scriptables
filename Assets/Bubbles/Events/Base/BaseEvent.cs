using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseEvent<T> : ScriptableObject
{
    private event Action<T> _event;

    public void Subscribe(Action<T> listener)
    {
        _event += listener;
    }

    public void Unsubscribe(Action<T> listener)
    {
        _event -= listener;
    }
    //[Button]
    public void Raise(T value)
    {
        _event?.Invoke(value);
    }
}