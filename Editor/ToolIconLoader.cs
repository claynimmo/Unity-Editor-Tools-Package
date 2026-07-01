using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// Helper class to load the icons for the toolbar regardless of where in the project the package is. It only works for tools inside editor/tools directly
/// </summary>
public static class ToolIconLoader
{
    /// <summary>
    /// Helper function to load the icon from the asset database through the package relative file path
    /// </summary>
    /// <param name="relativePath"> path starting from the root directory of the package to the icon, such as Gizmos/Icons/filename.png</param>
    /// <returns>Texture 2D object loaded from the path. Can return null if the path is invalid</returns>
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
