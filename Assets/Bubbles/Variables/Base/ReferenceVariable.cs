using UnityEngine;

public abstract class ReferenceVariable<T> : Base<T>
{
    public override void SaveValue()
    {
        Debug.LogWarning($"{name} is a Ref type — saving not supported.");
    }
    public override void LoadValue()
    {
        Debug.LogWarning($"{name} is a Ref type — loading not supported.");
    }   
    public override void ValidatePath()
    {
        Debug.LogWarning($"{name} is a Ref type — path validation not supported.");   
    }
       
}