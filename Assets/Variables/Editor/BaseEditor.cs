#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Base<>), true)] // true = applies to derived classes too
public class BaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // draws normal fields

        ISaveValue saveValue = (ISaveValue)target;
        IPathValidator pathValidator = (IPathValidator)target;
        ILoadValue loadValue = (ILoadValue)target;
        GUILayout.Space(10);

        if (GUILayout.Button("Save Value"))
        {
            saveValue.SaveValue();
        }
        if (GUILayout.Button("Load Value"))
        {
            loadValue.LoadValue();
        }
        if (GUILayout.Button("Validate path"))
        {
            pathValidator.ValidatePath();
        }
        
    }
}
#endif
