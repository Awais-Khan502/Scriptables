using System;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;

public class Base<T> : ScriptableObject, ISetValue<T>  , IGetValue<T>, ISaveValue, ILoadValue , IPathValidator
{
    [SerializeField] private T value;
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
        ValidatePath();
        LoadValue();
    }
    public virtual T GetValue(T value)
    {
        return value;
    }

    public bool LoadValue()
    {
        if(string.IsNullOrEmpty(_path) || !File.Exists(_path))
        {
            return false;
        }
        else
        {
            string json = File.ReadAllText(_path);
            value =  JsonConvert.DeserializeObject<T>(json);     
            return true;
        }
    }
    public void SaveValue()
    {
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
}


