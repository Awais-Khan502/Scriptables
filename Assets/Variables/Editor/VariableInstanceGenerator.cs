using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public class VariableInstanceGenerator : EditorWindow
{
    #region MENU_ITEMS

    private string[] derivedTypeNames;
    private int selectedTypeIndex;

    [MenuItem("Bubbles/Variables/Create Instance")]
    public static void ShowWindow()
    {
        GetWindow<VariableInstanceGenerator>("Create Instance");
    }

    private void OnGUI()
    {
    
        InitStyles(); // moved here — GUI.skin is only valid during OnGUI

        GUILayout.Label("Create Scriptable Asset", EditorStyles.boldLabel);

        if (derivedTypeNames == null || derivedTypeNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No classes derived from Base<T> found.", MessageType.Info);
            if (GUILayout.Button("Refresh")) RefreshDerivedTypes();
            return;
        }

        GUILayout.BeginHorizontal();
        selectedTypeIndex = EditorGUILayout.Popup("Type:", selectedTypeIndex, derivedTypeNames);
        if (GUILayout.Button("↺", GUILayout.Width(25))) RefreshDerivedTypes();
        GUILayout.EndHorizontal();

        if (GUILayout.Button($"Create {derivedTypeNames[selectedTypeIndex]} Asset"))
        {
            CreateAsset(derivedTypeNames[selectedTypeIndex]);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DrawPrimitiveTypes();
    }

    #endregion

    #region LIFECYCLE

    private void OnEnable()
    {
        RefreshDerivedTypes();
    }

    #endregion

    #region PRIMITIVE_TYPES

    private static readonly Color IntColor        = new Color(0.31f, 0.78f, 0.47f);
    private static readonly Color FloatColor      = new Color(0.26f, 0.60f, 0.87f);
    private static readonly Color DoubleColor     = new Color(0.17f, 0.45f, 0.70f);
    private static readonly Color BoolColor       = new Color(0.91f, 0.36f, 0.36f);
    private static readonly Color StringColor     = new Color(0.95f, 0.67f, 0.23f);
    private static readonly Color Vector2Color    = new Color(0.60f, 0.35f, 0.85f);
    private static readonly Color Vector3Color    = new Color(0.45f, 0.25f, 0.75f);
    private static readonly Color Vector2IntColor = new Color(0.75f, 0.30f, 0.70f);
    private static readonly Color TransformColor  = new Color(0.85f, 0.45f, 0.15f);
    private static readonly Color QuaternionColor  = new Color(0.85f, 0.45f, 0.15f);

    private static readonly HashSet<string> PrimitiveTypeNames = new HashSet<string>
    {
        "IntVariable", "FloatVariable", "DoubleVariable", "BoolVariable",
        "StringVariable", "Vector2Variable", "Vector3Variable",
        "Vector2IntVariable", "TransformVariable" , "QuaternionVariable" 
    };

    private GUIStyle _buttonStyle;

    private void InitStyles()
    {
        if (_buttonStyle != null) return; // only initialize once
        _buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold,
            fontSize  = 12,
            fixedHeight = 30
        };
    }

    private void DrawPrimitiveTypes()
    {
        GUILayout.Label("Primitive Types", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        GUILayout.BeginHorizontal();
        DrawColoredButton("Int",    IntColor,    () => CreateAsset("IntVariable"));
        DrawColoredButton("Float",  FloatColor,  () => CreateAsset("FloatVariable"));
        DrawColoredButton("Double", DoubleColor, () => CreateAsset("DoubleVariable"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        DrawColoredButton("Bool",   BoolColor,   () => CreateAsset("BoolVariable"));
        DrawColoredButton("String", StringColor, () => CreateAsset("StringVariable"));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("Unity Types", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        GUILayout.BeginHorizontal();
        DrawColoredButton("Vector2",    Vector2Color,    () => CreateAsset("Vector2Variable"));
        DrawColoredButton("Vector3",    Vector3Color,    () => CreateAsset("Vector3Variable"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        DrawColoredButton("Vector2Int", Vector2IntColor, () => CreateAsset("Vector2IntVariable"));
        DrawColoredButton("Transform",  TransformColor,  () => CreateAsset("TransformVariable"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        DrawColoredButton("QuaternionColor", QuaternionColor, () => CreateAsset("QuaternionVariable"));
        GUILayout.EndHorizontal();
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
            "Save Scriptable Asset",
            className,
            "asset",
            "Choose location for the asset",
            "Assets/Variables/Instances"
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

    private void RefreshDerivedTypes()
    {
        derivedTypeNames = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.BaseType != null
                     && t.BaseType.IsGenericType
                     && t.BaseType.GetGenericTypeDefinition() == typeof(Base<>)
                     && !PrimitiveTypeNames.Contains(t.Name)) // exclude primitives from list
            .Select(t => t.Name)
            .ToArray();
    }

    #endregion
}