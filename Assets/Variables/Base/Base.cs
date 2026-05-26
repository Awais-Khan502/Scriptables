using System;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;

public class Base<T> : ScriptableObject, ISetValue<T>  , IGetValue<T>, ISaveValue, ILoadValue , IPathValidator , IValueModifier<T>
{
    [field:SerializeField] public T value {get; private set;}
    [SerializeField] private T defaultValue;
    [SerializeField] private bool persistValue = false;

    private string _path;

    public void ValidatePath()
    {
         
        string basePath = Path.Combine(Application.persistentDataPath, name);
        _path = basePath + ".json";

        int counter = 1;
        while (File.Exists(_path))
        {
            _path = $"{basePath}_{counter}.json";
            counter++;
        }
        Debug.Log("Path Validated : " + _path);
    }//

    private void OnEnable()
    {
        if (!persistValue)
        {
            value = defaultValue;
        }
        else
        {
            ValidatePath();
            LoadValue();
        }
            
       
    }
    public virtual T GetValue(T value)
    {
        return value;
    }

    public virtual bool LoadValue()
    {
        if(string.IsNullOrEmpty(_path) || !File.Exists(_path))
        {
            value = defaultValue;
            return false;
        }
        else
        {
            string json = File.ReadAllText(_path);
            value =  JsonConvert.DeserializeObject<T>(json);     
            return true;
        }
    }
    public virtual void SaveValue()
    {
        if(!persistValue) return;
        if(string.IsNullOrEmpty(_path) )
        {
            Debug.Log(" Path is Empty");
            return;
        }
        string json = JsonConvert.SerializeObject(value, Formatting.Indented);
        File.WriteAllText(_path, json);
        Debug.Log( " Data Saved : " + json); 
        Debug.Log( " Path :  " + _path );
    }
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
