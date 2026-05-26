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
    private Vector2 _scrollPosition;

    [MenuItem("Bubbles/Variables/Create Instance")]
    public static void ShowWindow()
    {
        GetWindow<VariableInstanceGenerator>("Create Instance");
    }

    private void OnGUI()
    {
        InitStyles();
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawSectionHeader("Custom Classes", new Color(0.20f, 0.80f, 0.90f));
        EditorGUILayout.Space(5);

        if (derivedTypeNames == null || derivedTypeNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No custom classes derived from Base<T> found.", MessageType.Info);
            if (GUILayout.Button("Refresh")) RefreshDerivedTypes();
        }
        else
        {
            GUILayout.BeginHorizontal();
            selectedTypeIndex = EditorGUILayout.Popup("Type:", selectedTypeIndex, derivedTypeNames);
            if (GUILayout.Button("↺", GUILayout.Width(25))) RefreshDerivedTypes();
            GUILayout.EndHorizontal();

            DrawColoredButton($"Create {derivedTypeNames[selectedTypeIndex]} Asset", new Color(0.20f, 0.80f, 0.90f), () => CreateAsset(derivedTypeNames[selectedTypeIndex]));
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawPrimitiveTypes();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawUnityValueTypes();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawRefTypes();

        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region LIFECYCLE

    private void OnEnable()
    {
        RefreshDerivedTypes();
    }

    #endregion

    #region PRIMITIVE_TYPES

    private static readonly Color IntColor    = new Color(0.40f, 0.95f, 0.55f);
    private static readonly Color FloatColor  = new Color(0.35f, 0.75f, 1.00f);
    private static readonly Color DoubleColor = new Color(0.25f, 0.60f, 0.95f);
    private static readonly Color BoolColor   = new Color(1.00f, 0.45f, 0.45f);
    private static readonly Color StringColor = new Color(1.00f, 0.80f, 0.25f);

    private void DrawPrimitiveTypes()
    {
        DrawSectionHeader("Primitive Types", new Color(1.00f, 0.80f, 0.25f));
        EditorGUILayout.Space(5);

        DrawEqualButtons(
            ("Int",    IntColor,    () => CreateAsset("IntVariable")),
            ("Float",  FloatColor,  () => CreateAsset("FloatVariable")),
            ("Double", DoubleColor, () => CreateAsset("DoubleVariable"))
        );
        DrawEqualButtons(
            ("Bool",   BoolColor,   () => CreateAsset("BoolVariable")),
            ("String", StringColor, () => CreateAsset("StringVariable")),
            ("",       Color.clear, null)
        );
    }


    #endregion

    #region UNITY_VALUE_TYPES

    private static readonly Color Vector2Color    = new Color(0.75f, 0.45f, 1.00f);
    private static readonly Color Vector3Color    = new Color(0.60f, 0.35f, 0.95f);
    private static readonly Color Vector2IntColor = new Color(0.90f, 0.40f, 0.90f);
    private static readonly Color Vector3IntColor = new Color(0.75f, 0.30f, 0.80f);
    private static readonly Color QuaternionColor = new Color(1.00f, 0.55f, 0.20f);
    private static readonly Color ColorVarColor   = new Color(1.00f, 0.90f, 0.30f);
    private static readonly Color RectColor       = new Color(0.35f, 0.80f, 0.90f);
    private static readonly Color BoundsColor     = new Color(0.25f, 0.70f, 0.80f);

    private void DrawUnityValueTypes()
    {
        DrawSectionHeader("Unity Value Types", new Color(0.75f, 0.45f, 1.00f));
        EditorGUILayout.Space(5);

        DrawEqualButtons(
            ("Vector2",    Vector2Color,    () => CreateAsset("Vector2Variable")),
            ("Vector3",    Vector3Color,    () => CreateAsset("Vector3Variable")),
            ("Vector2Int", Vector2IntColor, () => CreateAsset("Vector2IntVariable"))
        );
        DrawEqualButtons(
            ("Vector3Int", Vector3IntColor, () => CreateAsset("Vector3IntVariable")),
            ("Quaternion", QuaternionColor, () => CreateAsset("QuaternionVariable")),
            ("Color",      ColorVarColor,   () => CreateAsset("ColorVariable"))
        );
        DrawEqualButtons(
            ("Rect",   RectColor,   () => CreateAsset("RectVariable")),
            ("Bounds", BoundsColor, () => CreateAsset("BoundsVariable")),
            ("",       Color.clear, null)
        );
    }

    #endregion

    #region REF_TYPES

    private static readonly Color TransformColor       = new Color(1.00f, 0.35f, 0.35f);
    private static readonly Color GameObjectColor      = new Color(1.00f, 0.45f, 0.25f);
    private static readonly Color CameraColor          = new Color(0.30f, 0.70f, 1.00f);
    private static readonly Color RigidbodyColor       = new Color(0.85f, 0.60f, 0.25f);
    private static readonly Color Rigidbody2DColor     = new Color(0.90f, 0.65f, 0.30f);
    private static readonly Color ColliderColor        = new Color(0.50f, 0.85f, 0.40f);
    private static readonly Color Collider2DColor      = new Color(0.55f, 0.90f, 0.45f);
    private static readonly Color AudioClipColor       = new Color(0.70f, 0.45f, 1.00f);
    private static readonly Color SpriteColor          = new Color(1.00f, 0.65f, 0.75f);
    private static readonly Color Texture2DColor       = new Color(0.40f, 0.85f, 0.85f);
    private static readonly Color MaterialColor        = new Color(0.85f, 0.85f, 0.25f);
    private static readonly Color MeshColor            = new Color(0.55f, 0.55f, 0.95f);
    private static readonly Color AnimationClipColor   = new Color(0.90f, 0.45f, 0.65f);
    private static readonly Color RuntimeAnimatorColor = new Color(0.45f, 0.75f, 0.60f);

    private void DrawRefTypes()
    {
        DrawSectionHeader("Ref Types", new Color(1.00f, 0.35f, 0.35f));
        EditorGUILayout.HelpBox("Scene object references — no saving mechanics.", MessageType.None);
        EditorGUILayout.Space(5);

        DrawEqualButtons(
            ("Transform",   TransformColor,   () => CreateAsset("TransformVariable")),
            ("GameObject",  GameObjectColor,  () => CreateAsset("GameObjectVariable")),
            ("Camera",      CameraColor,      () => CreateAsset("CameraVariable"))
        );
        DrawEqualButtons(
            ("Rigidbody",   RigidbodyColor,   () => CreateAsset("RigidbodyVariable")),
            ("Rigidbody2D", Rigidbody2DColor, () => CreateAsset("Rigidbody2DVariable")),
            ("AudioClip",   AudioClipColor,   () => CreateAsset("AudioClipVariable"))
        );
        DrawEqualButtons(
            ("Collider",    ColliderColor,    () => CreateAsset("ColliderVariable")),
            ("Collider2D",  Collider2DColor,  () => CreateAsset("Collider2DVariable")),
            ("Sprite",      SpriteColor,      () => CreateAsset("SpriteVariable"))
        );
        DrawEqualButtons(
            ("Texture2D",   Texture2DColor,   () => CreateAsset("Texture2DVariable")),
            ("Material",    MaterialColor,    () => CreateAsset("MaterialVariable")),
            ("Mesh",        MeshColor,        () => CreateAsset("MeshVariable"))
        );
        DrawEqualButtons(
            ("AnimClip",    AnimationClipColor,   () => CreateAsset("AnimationClipVariable")),
            ("Animator",    RuntimeAnimatorColor, () => CreateAsset("RuntimeAnimatorControllerVariable")),
            ("",            Color.clear,          null)
        );
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
        GUIStyle coloredHeader = new GUIStyle(_headerStyle);
        coloredHeader.normal.textColor = color;
        GUILayout.Label(title, coloredHeader);
    }

    private void DrawColoredButton(string label, Color color, Action onClick, bool disabled = false)
    {
        GUI.enabled = !disabled;
        Color prev = GUI.backgroundColor;
        GUI.backgroundColor = disabled ? Color.clear : color;
        if (GUILayout.Button(label, _buttonStyle) && onClick != null)
            onClick.Invoke();
        GUI.backgroundColor = prev;
        GUI.enabled = true;
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

    private static readonly HashSet<string> BuiltInTypeNames = new HashSet<string>
    {
        "IntVariable", "FloatVariable", "DoubleVariable", "BoolVariable", "StringVariable",
        "Vector2Variable", "Vector3Variable", "Vector2IntVariable", "Vector3IntVariable",
        "QuaternionVariable", "ColorVariable", "RectVariable", "BoundsVariable",
        "TransformVariable", "GameObjectVariable", "CameraVariable", "RigidbodyVariable",
        "Rigidbody2DVariable", "ColliderVariable", "Collider2DVariable", "AudioClipVariable",
        "SpriteVariable", "Texture2DVariable", "MaterialVariable", "MeshVariable",
        "AnimationClipVariable", "RuntimeAnimatorControllerVariable"
    };

    private void RefreshDerivedTypes()
    {
        derivedTypeNames = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract
                     && t.BaseType != null
                     && t.BaseType.IsGenericType
                     && t.BaseType.GetGenericTypeDefinition() == typeof(DataVariable<>)
                     && !BuiltInTypeNames.Contains(t.Name))
            .Select(t => t.Name)
            .ToArray();
    }

    #endregion
}