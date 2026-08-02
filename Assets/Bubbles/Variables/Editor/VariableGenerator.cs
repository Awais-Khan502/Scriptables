using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class VariableGenerator : EditorWindow
{


    #region  MENU_ITEMS
    private string typeName = "";
    private string suffix = "Variable";



    [MenuItem("Bubbles/Variables/Create Variable")]
    public static void ShowWindow()
    {
        GetWindow<VariableGenerator>("Create Variable");
    }
    private void OnGUI()
    {
        // --- Generate Section ---
        GUILayout.Label("Generate Variable Class", EditorStyles.boldLabel);
        typeName = EditorGUILayout.TextField("Type:", typeName);

        GUI.enabled = !string.IsNullOrEmpty(typeName);
        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
}
    #endregion

    #region CORE
    private void Generate()
    {
        if (string.IsNullOrEmpty(typeName))
        {
            Debug.LogError("Type name is empty.");
            return;
        }

        Type foundType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);

        if (foundType == null)
        {
            Debug.LogError($"No class named {typeName} found in project.");
            return;
        }
        if (!foundType.IsClass || foundType.IsAbstract)
        {
            Debug.LogError("Type must be a non-abstract class.");
            return;
        }

        string className = typeName + suffix;
        string path = $"Assets/Bubbles/Variables/Classes/{className}.cs";

        if (File.Exists(path))
        {
            Debug.LogError($"{className} already exists.");
            return;
        }

        Type baseGenericType = typeof(DataVariable<>);
        Type closedBaseType = typeof(DataVariable<>).MakeGenericType(foundType);
        HashSet<string> requiredNamespaces;

        string overrides = GenerateOverrides(closedBaseType , out requiredNamespaces);
        // Build using statements
        string usings = string.Join("\n", requiredNamespaces.Select(ns => $"using {ns};"));
string script =
$@"{usings}
using UnityEngine;

[CreateAssetMenu(menuName = ""Variables/{className}"")]
public class {className} : DataVariable<{typeName}>
{{
    {overrides}
}}";

        File.WriteAllText(path, script);
        AssetDatabase.Refresh();
        

        Debug.Log($"{className} created successfully.");
    }
    #endregion

    #region HELPER
    private string GenerateOverrides(Type closedBaseType , out HashSet<string> requiredNamespaces)
    {
        requiredNamespaces = new HashSet<string>();

        var methods = closedBaseType
            .GetMethods(System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic)
            .Where(m => m.IsVirtual && !m.IsFinal && m.DeclaringType == closedBaseType && !m.IsSpecialName);

        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        foreach (var method in methods)
        {
            string returnType = GetCSharpTypeName(method.ReturnType);

            var parameters = method.GetParameters();

            // Collect namespaces from parameters
            foreach (var p in parameters)
            {
                CollectNamespaces(p.ParameterType, requiredNamespaces);
            }
            string paramList = string.Join(", ",
                parameters.Select(p => $"{GetCSharpTypeName(p.ParameterType)} {p.Name}"));

            string paramNames = string.Join(", ",
                parameters.Select(p => p.Name));

            builder.AppendLine($"    public override {returnType} {method.Name}({paramList})");
            builder.AppendLine("    {");

            if (returnType != "void")
                builder.AppendLine($"        return base.{method.Name}({paramNames});");
            else
                builder.AppendLine($"        base.{method.Name}({paramNames});");

            builder.AppendLine("    }");
            builder.AppendLine();
        }

        return builder.ToString();
    }
    private string GetCSharpTypeName(Type type)
    {
        if (type == typeof(void)) return "void";
        if (type == typeof(int)) return "int";
        if (type == typeof(float)) return "float";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(string)) return "string";
        if (type == typeof(object)) return "object";

        // return type.FullName; // fallback for classes
        if (!type.IsGenericType)
            return type.Name;

        // Handle Action<T>, Func<T> etc
        string genericTypeName = type.GetGenericTypeDefinition().Name;
        genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`')); // remove `1

        string genericArgs = string.Join(", ",
            type.GetGenericArguments().Select(GetCSharpTypeName)); // recursive for nested generics

        return $"{genericTypeName}<{genericArgs}>";
    }
    private void CollectNamespaces(Type type, HashSet<string> namespaces)
    {
        if (type.Namespace != null)
            namespaces.Add(type.Namespace);

        // Recurse into generic arguments e.g Action<KnightProfile>
        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
                CollectNamespaces(arg, namespaces);
        }
    }
    #endregion
}
