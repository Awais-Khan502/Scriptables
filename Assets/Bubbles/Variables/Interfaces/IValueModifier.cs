using System;
using UnityEngine;
public interface IValueModifier<T>
{
    public void ModifyValue(Action<T> modifier);
}
