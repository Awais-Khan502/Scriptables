using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class VariableInstanceGenerator : EditorWindow
{


    #region  MENU_ITEMS

    private string[] derivedTypeNames;
    private int selectedTypeIndex;

    [MenuItem("Awais/Variables/Create Instance")]
    public static void ShowWindow()
    {
        GetWindow<VariableInstanceGenerator>("Create Instance");
    }
    private void OnGUI()
    {
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
    }

    #endregion
    #region LIFECYCLE
    private void OnEnable()
    {
        RefreshDerivedTypes();
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
                    && t.BaseType.GetGenericTypeDefinition() == typeof(Base<>))
            .Select(t => t.Name)
            .ToArray();
    }
    #endregion
}
