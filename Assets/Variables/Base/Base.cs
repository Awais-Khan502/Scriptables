using System;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Overlays;

public abstract class Base<T> : ScriptableObject, ISetValue<T>  , IGetValue<T>, ISaveValue<T>, ILoadValue<T>
{
    [SerializeField] private T value;
    [SerializeField] private string _path;

    private void Awake()
    {
        _path = Application.persistentDataPath + _path;
    }
    public void OnValidate()
    {
#if UNITY_EDITOR
        CheckPath();
#endif
    SaveValue();
    }

#if UNITY_EDITOR
    private void CheckPath()
    {
        // if (string.IsNullOrEmpty(_path))
        //     return;

        // if (Directory.Exists(_path) || File.Exists(_path))
        // {
        //     UnityEditor.EditorUtility.DisplayDialog(
        //         "Path Exists",
        //         $"The path \"{_path}\" already exists.",
        //         "OK"
        //     );
        //     _path = "";
        // }
    }
#endif

    private void OnEnable()
    {
        value = LoadValue();
    }
    public virtual T GetValue(T value)
    {
        return value;
    }

    public T LoadValue()
    {
        if (!File.Exists(_path))
            return value;
        else
        {
            string json = File.ReadAllText(_path);
            return JsonConvert.DeserializeObject<T>(json);     
        }
    }
    public void SaveValue()
    {
        string json = JsonConvert.SerializeObject(value, Formatting.Indented);
        File.WriteAllText(_path, json);
        Debug.Log( " Data Saved : " + json); 
    }

    public virtual void SetValue(T value)
    {
        this.value = value;
        SaveValue();
    }
}


