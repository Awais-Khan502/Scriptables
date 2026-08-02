using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Base<>), true)]
public class BaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Type type = target.GetType();

        if (type.GetCustomAttribute<RefVariableAttribute>() != null)
        {
            DrawExcluding("persistValue");
            EditorGUILayout.HelpBox("Ref type — no saving mechanics available.", MessageType.Info);
            return;
        }

        // if (type.GetCustomAttribute<UnityValueVariableAttribute>() != null)
        // {
        //     return;
        // }

        // Primitives and custom classes
        DrawDefaultInspector();

        SerializedProperty persistProp = serializedObject.FindProperty("persistValue");
        if (persistProp == null) return;

        if (persistProp.boolValue)
        {
            GUILayout.Space(10);

            if (GUILayout.Button("Save Value"))
                ((ISaveValue)target).SaveValue();

            if (GUILayout.Button("Load Value"))
                ((ILoadValue)target).LoadValue();

            if (GUILayout.Button("Validate Path"))
                ((IPathValidator)target).ValidatePath();
        }
    }
    private void DrawValueFieldOnly()
    {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();
        prop.NextVisible(true); // skip script field

        while (prop.NextVisible(false))
        {
            if (prop.name == "value")
            {
                EditorGUILayout.PropertyField(prop, true);
                break;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawExcluding(params string[] excludedFields)
    {
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();
        prop.NextVisible(true); // skip script field

        while (prop.NextVisible(false))
        {
            if (!Array.Exists(excludedFields, f => f == prop.name))
                EditorGUILayout.PropertyField(prop, true);
        }
        serializedObject.ApplyModifiedProperties();
    }
}