using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

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
    private Vector2 _scrollPosition;

    #endregion

    #region LIFECYCLE

    private void OnEnable()
    {
        RefreshEventTypes();
    }

    private void OnGUI()
    {
        InitStyles();
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawSectionHeader("Custom Events", new Color(0.90f, 0.50f, 1.00f));
        EditorGUILayout.Space(5);

        if (_derivedEventNames == null || _derivedEventNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No custom events found. Generate one first.", MessageType.Info);
            if (GUILayout.Button("Refresh")) RefreshEventTypes();
        }
        else
        {
            GUILayout.BeginHorizontal();
            _selectedIndex = EditorGUILayout.Popup("Event:", _selectedIndex, _derivedEventNames);
            if (GUILayout.Button("↺", GUILayout.Width(25))) RefreshEventTypes();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            DrawColoredButton($"Create {_derivedEventNames[_selectedIndex]} Asset",
                new Color(0.90f, 0.50f, 1.00f), () => CreateAsset(_derivedEventNames[_selectedIndex]));
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawPrimitiveEvents();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawUnityValueEvents();

        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region PRIMITIVE_EVENTS

    private static readonly Color IntEventColor    = new Color(0.40f, 0.95f, 0.55f);
    private static readonly Color FloatEventColor  = new Color(0.35f, 0.75f, 1.00f);
    private static readonly Color DoubleEventColor = new Color(0.25f, 0.60f, 0.95f);
    private static readonly Color BoolEventColor   = new Color(1.00f, 0.45f, 0.45f);
    private static readonly Color StringEventColor = new Color(1.00f, 0.80f, 0.25f);

    private void DrawPrimitiveEvents()
    {
        DrawSectionHeader("Primitive Events", new Color(1.00f, 0.80f, 0.25f));
        EditorGUILayout.Space(5);

        DrawEqualButtons(
            ("Int",    IntEventColor,    () => CreateAsset("IntEvent")),
            ("Float",  FloatEventColor,  () => CreateAsset("FloatEvent")),
            ("Double", DoubleEventColor, () => CreateAsset("DoubleEvent"))
        );
        DrawEqualButtons(
            ("Bool",   BoolEventColor,   () => CreateAsset("BoolEvent")),
            ("String", StringEventColor, () => CreateAsset("StringEvent")),
            ("",       Color.clear,      null)
        );
    }

    #endregion

    #region UNITY_VALUE_EVENTS

    private static readonly Color Vector2EventColor    = new Color(0.75f, 0.45f, 1.00f);
    private static readonly Color Vector3EventColor    = new Color(0.60f, 0.35f, 0.95f);
    private static readonly Color Vector2IntEventColor = new Color(0.90f, 0.40f, 0.90f);
    private static readonly Color Vector3IntEventColor = new Color(0.75f, 0.30f, 0.80f);

    private void DrawUnityValueEvents()
    {
        DrawSectionHeader("Unity Value Events", new Color(0.75f, 0.45f, 1.00f));
        EditorGUILayout.Space(5);

        DrawEqualButtons(
            ("Vector2",    Vector2EventColor,    () => CreateAsset("Vector2Event")),
            ("Vector3",    Vector3EventColor,    () => CreateAsset("Vector3Event")),
            ("Vector2Int", Vector2IntEventColor, () => CreateAsset("Vector2IntEvent"))
        );
        DrawEqualButtons(
            ("Vector3Int", Vector3IntEventColor, () => CreateAsset("Vector3IntEvent")),
            ("",           Color.clear,          null),
            ("",           Color.clear,          null)
        );
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

    private static readonly HashSet<string> BuiltInEventNames = new HashSet<string>
    {
        "IntEvent", "FloatEvent", "DoubleEvent", "BoolEvent", "StringEvent",
        "Vector2Event", "Vector3Event", "Vector2IntEvent", "Vector3IntEvent"
    };

    private void RefreshEventTypes()
    {
        _derivedEventNames = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.BaseType != null
                     && t.BaseType.IsGenericType
                     && t.BaseType.GetGenericTypeDefinition() == typeof(BaseEvent<>)
                     && !BuiltInEventNames.Contains(t.Name))
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

    private void DrawEqualButtons(params (string label, Color color, Action onClick)[] buttons)
    {
        float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / buttons.Length;

        GUILayout.BeginHorizontal();
        foreach (var (label, color, onClick) in buttons)
        {
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = label == "" ? Color.clear : color;
            GUI.enabled = label != "";
            if (GUILayout.Button(label, _buttonStyle, GUILayout.Width(buttonWidth)))
                onClick?.Invoke();
            GUI.backgroundColor = prev;
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
    }

    #endregion
}