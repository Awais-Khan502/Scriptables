using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public abstract class ValueVariable<T> : Base<T> 
{
    protected virtual JsonConverter GetConverter() => null;
    JsonSerializerSettings settings = null;
    public override void SaveValue()
    {
        if (!PersistValue) return;
        if (string.IsNullOrEmpty(Path)) { Debug.Log("Path is Empty"); return; }

        string json = JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        File.WriteAllText(Path, json);
        Debug.Log("Data Saved : " + json);
    }

    public override void LoadValue()
    {
        if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
        {
            SetValue(DefaultValue);
        }
        string json = File.ReadAllText(Path);
        SetValue(JsonConvert.DeserializeObject<T>(json, settings));
    }

    protected override void OnEnable()
    {
        if(settings == null)
        {
            var converter = GetConverter();
            if (converter != null)
            {
                settings = new JsonSerializerSettings();
                settings.Converters.Add(converter);
            }  
        }
        base.OnEnable();
    }
}