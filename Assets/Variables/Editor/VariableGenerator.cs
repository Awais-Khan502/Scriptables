using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

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
        string path = $"Assets/Variables/Classes/{className}.cs";

        if (File.Exists(path))
        {
            Debug.LogError($"{className} already exists.");
            return;
        }

        Type baseGenericType = typeof(Base<>);
        Type closedBaseType = typeof(Base<>).MakeGenericType(foundType);

        string overrides = GenerateOverrides(closedBaseType);
        string script =
$@"using UnityEngine;

[CreateAssetMenu(menuName = ""Variables/{className}"")]
public class {className} : Base<{typeName}>
{{
    {overrides}
}}";

        File.WriteAllText(path, script);
        AssetDatabase.Refresh();
        

        Debug.Log($"{className} created successfully.");
    }
    #endregion

    #region HELPER
    private string GenerateOverrides(Type closedBaseType)
    {
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
            string paramList = string.Join(", ",
                parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

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

        // Handle generics
        if (type.IsGenericType)
        {
            string typeName = type.Name;
            int index = typeName.IndexOf('`');
            if (index > 0) typeName = typeName.Substring(0, index); // remove `1 etc

            string genericArgs = string.Join(", ", type.GetGenericArguments().Select(t => GetCSharpTypeName(t)));
            return $"{typeName}<{genericArgs}>";
        }

        return type.FullName; // fallback for classes
    }
    #endregion
}
