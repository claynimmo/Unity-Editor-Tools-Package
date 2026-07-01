using UnityEditor;
using UnityEngine;
using System.IO;

public static class ToolIconLoader
{
    public static Texture2D LoadIcon(string relativePath){
        // get the path to this script
        var script = MonoScript.FromScriptableObject(
            ScriptableObject.CreateInstance<BelzierPathEditorTool>()
        );

        string scriptPath = AssetDatabase.GetAssetPath(script); 

        // Assets/EditorToolsPackage/Editor/Tools/BelzierPathEditorTool.cs
        // Packages/EditorToolsPackage/Editor/Tools/BelzierPathEditorTool.cs

        string scriptDir = Path.GetDirectoryName(scriptPath).Replace("\\", "/");

        // go up folders
        string packageRoot = Path.GetFullPath(Path.Combine(scriptDir, "../.."))
            .Replace("\\", "/");

        string projectRoot = Application.dataPath.Replace("/Assets", "");
        string projectRelativeRoot = packageRoot.Replace(projectRoot + "/", "");
        string iconPath = projectRelativeRoot + "/" + relativePath;
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

        if (icon == null)
            Debug.LogError("Failed to load icon at: " + iconPath);

        return icon;
    }
}
