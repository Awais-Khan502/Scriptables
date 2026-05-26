#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Base<>), true)]
public class BaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        bool persistValue = serializedObject.FindProperty("persistValue").boolValue;

        if (persistValue)
        {
            ISaveValue saveValue       = (ISaveValue)target;
            IPathValidator pathValidator = (IPathValidator)target;
            ILoadValue loadValue       = (ILoadValue)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Save Value"))
                saveValue.SaveValue();

            if (GUILayout.Button("Load Value"))
                loadValue.LoadValue();

            if (GUILayout.Button("Validate Path"))
                pathValidator.ValidatePath();
        }
    }
}
#endif