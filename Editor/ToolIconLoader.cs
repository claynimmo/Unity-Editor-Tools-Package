using UnityEditor;
using UnityEngine;
using System.IO;

public static class ToolIconLoader
{
    public static Texture2D LoadIcon(string relativePath){
        var script = MonoScript.FromScriptableObject(
            ScriptableObject.CreateInstance<BelzierPathEditorTool>()
        );

        string scriptPath = AssetDatabase.GetAssetPath(script);
        // Assets/EditorToolsPackage/Editor/Tools/BelzierPathEditorTool.cs
        // Packages/com.clay.editor/Editor/Tools/BelzierPathEditorTool.cs

        string scriptDir = Path.GetDirectoryName(scriptPath).Replace("\\", "/");
        string packageRoot = Path.GetDirectoryName(
            Path.GetDirectoryName(scriptDir)
        ).Replace("\\", "/");

        string iconPath = packageRoot + "/" + relativePath;

        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

        if (icon == null)
            Debug.LogError("Failed to load icon at: " + iconPath);

        return icon;
    }
}
