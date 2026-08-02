using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class EventInstanceGenerator : EditorWindow
{
    #region MENU_ITEMS

    [MenuItem("Bubbles/Events/Create Event Instance")]
    public static void ShowWindow()
    {
        GetWindow<EventInstanceGenerator>("Create Event Instance");
    }

    #endregion

    #region FIELDS

    private string[] _derivedEventNames;
    private int _selectedIndex;

    #endregion

    #region LIFECYCLE

    private void OnEnable()
    {
        RefreshEventTypes();
    }

    private void OnGUI()
    {
        InitStyles();

        DrawSectionHeader("Create Event Asset", new Color(0.90f, 0.50f, 1.00f));
        EditorGUILayout.Space(5);

        if (_derivedEventNames == null || _derivedEventNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No classes derived from BaseEvent<T> found. Generate one first.", MessageType.Info);

            if (GUILayout.Button("Refresh")) RefreshEventTypes();
            return;
        }

        GUILayout.BeginHorizontal();
        _selectedIndex = EditorGUILayout.Popup("Event:", _selectedIndex, _derivedEventNames);
        if (GUILayout.Button("↺", GUILayout.Width(25))) RefreshEventTypes();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        DrawColoredButton($"Create {_derivedEventNames[_selectedIndex]} Asset", new Color(0.90f, 0.50f, 1.00f), () =>
        {
            CreateAsset(_derivedEventNames[_selectedIndex]);
        });
    }

    #endregion

    #region CORE

    private void CreateAsset(string className)
    {
        Type type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == className);

        if (type == null)
        {
            Debug.LogError($"Type {className} not found. Generate it first.");
            return;
        }

        string assetName = EditorUtility.SaveFilePanelInProject(
            "Save Event Asset",
            className,
            "asset",
            "Choose location for the asset",
            "Assets/Events/Instances"
        );

        if (string.IsNullOrEmpty(assetName)) return;

        ScriptableObject instance = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(instance, assetName);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = instance;

        Debug.Log($"{className} asset created at {assetName}");
    }

    #endregion

    #region HELPER

    private void RefreshEventTypes()
    {
        _derivedEventNames = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.BaseType != null
                     && t.BaseType.IsGenericType
                     && t.BaseType.GetGenericTypeDefinition() == typeof(BaseEvent<>))
            .Select(t => t.Name)
            .ToArray();
    }

    #endregion

    #region STYLES

    private GUIStyle _buttonStyle;
    private GUIStyle _headerStyle;

    private void InitStyles()
    {
        if (_buttonStyle != null) return;

        _buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontStyle   = FontStyle.Bold,
            fontSize    = 12,
            fixedHeight = 30
        };

        _headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize  = 13,
            fontStyle = FontStyle.Bold
        };
    }

    private void DrawSectionHeader(string title, Color color)
    {
        GUIStyle colored = new GUIStyle(_headerStyle);
        colored.normal.textColor = color;
        GUILayout.Label(title, colored);
    }

    private void DrawColoredButton(string label, Color color, Action onClick)
    {
        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = color;
        if (GUILayout.Button(label, _buttonStyle))
            onClick?.Invoke();
        GUI.backgroundColor = prev;
    }

    #endregion
}