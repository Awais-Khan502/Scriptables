using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class EventGenerator : EditorWindow
{
    #region MENU_ITEMS

    [MenuItem("Bubbles/Events/New Event From Type")]
    public static void ShowWindow()
    {
        GetWindow<EventGenerator>("Create Event");
    }

    #endregion

    #region FIELDS

    private string _typeName;
    private const string Suffix = "Event";
    private const string OutputPath = "Assets/Bubbles/Events/Classes";

    #endregion

    #region LIFECYCLE

    private void OnGUI()
    {
        GUILayout.Label("Generate Event Class", EditorStyles.boldLabel);
        _typeName = EditorGUILayout.TextField("Type:", _typeName);

        GUI.enabled = !string.IsNullOrEmpty(_typeName);
        if (GUILayout.Button("Generate"))
            Generate();
        GUI.enabled = true;
    }

    #endregion

    #region CORE

    private void Generate()
    {
        if (string.IsNullOrEmpty(_typeName))
        {
            Debug.LogError("Type name is empty.");
            return;
        }

        // Find the type in loaded assemblies
        Type foundType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == _typeName);

        if (foundType == null)
        {
            Debug.LogError($"No class named {_typeName} found in project.");
            return;
        }

        if (!foundType.IsClass || foundType.IsAbstract)
        {
            Debug.LogError("Type must be a non-abstract class.");
            return;
        }

        string className = _typeName + Suffix;
        string path      = $"{OutputPath}/{className}.cs";

        if (!Directory.Exists(OutputPath))
            Directory.CreateDirectory(OutputPath);

        if (File.Exists(path))
        {
            Debug.LogError($"{className} already exists.");
            return;
        }

        string script =
$@"using UnityEngine;

[CreateAssetMenu(menuName = ""Bubbles/Events/{className}"")]
public class {className} : BaseEvent<{_typeName}>
{{
}}";

        File.WriteAllText(path, script);
        AssetDatabase.Refresh();

        Debug.Log($"{className} created at {path}");
    }

    #endregion
}