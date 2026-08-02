using System;
using UnityEngine;
using System.IO;


public class Base<T> : ScriptableObject, ISetValue<T>  , IGetValue<T>, ISaveValue, ILoadValue , IPathValidator , IValueModifier<T>
{
    [field:SerializeField] public T value {get; private set;}
    [SerializeField] private T defaultValue;
    [SerializeField] private bool persistValue = false;

    private string _path;
    protected T DefaultValue  => defaultValue;
    protected bool PersistValue => persistValue;
    protected string Path => _path;

    public virtual void ValidatePath()
    {   
        string basePath = System.IO.Path.Combine(Application.persistentDataPath, name);
        _path = basePath + ".json";

        int counter = 1;
        while (File.Exists(_path))
        {
            _path = $"{basePath}_{counter}.json";
            counter++;
        }
        Debug.Log("Path Validated : " + _path);
    }//
    protected virtual void OnEnable()
    {
        if (!persistValue)
        {
            value = defaultValue;
        }
        else
        {
            //ValidatePath();
            LoadValue();
        } 
    }
    public virtual T GetValue() => value;

    public virtual void LoadValue() {}
    public virtual void SaveValue() {}
    public virtual void SetValue(T value)
    {
        this.value = value;
        SaveValue();
    }

    public virtual void ModifyValue(Action<T> modifier)
    {
        if(value == null) return;
        modifier(value);
        SaveValue();
    }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class RefVariableAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public class UnityValueVariableAttribute : System.Attribute { }
