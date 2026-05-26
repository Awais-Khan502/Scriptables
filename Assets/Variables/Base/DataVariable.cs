using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public abstract class DataVariable<T> : Base<T>
{
    public override void SaveValue()
    {
        if(!PersistValue) return;
        if(string.IsNullOrEmpty(Path) )
        {
            Debug.Log(" Path is Empty");
            return;
        }
        string json = JsonConvert.SerializeObject(value, Formatting.Indented);
        File.WriteAllText(Path, json);
        Debug.Log( " Data Saved : " + json); 
        Debug.Log( " Path :  " + Path );
    }
    public override void LoadValue()
    {
        if(string.IsNullOrEmpty(Path) || !File.Exists(Path))
        {
           SetValue(DefaultValue);
        }
        else
        {
            string json = File.ReadAllText(Path);
            T tempValue =  JsonConvert.DeserializeObject<T>(json);     
            SetValue(tempValue);
        }
    }
}