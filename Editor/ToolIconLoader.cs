using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class ToolIconLoader
{
    private static string GetRelativePath(string fullPath){
        fullPath = fullPath.Replace("\\", "/");

        string projectPath = Application.dataPath.Replace("/Assets", "");

        if(fullPath.StartsWith(projectPath)){
            return fullPath.Substring(projectPath.Length + 1); 
        }

        return null; // not inside project
    }

    public static Texture2D LoadIcon(string relativePath){
        var script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<BelzierPathEditorTool>());
        var scriptPath = AssetDatabase.GetAssetPath(script);
        var folder = Path.GetDirectoryName(scriptPath);

        // move up two folders, since the tools are in editor/tools
        var root = Path.GetFullPath(Path.Combine(folder, "../.."));

        var fullPath = Path.Combine(root, relativePath).Replace("\\", "/");

        var projectRelative = GetRelativePath(fullPath);

        if (string.IsNullOrEmpty(projectRelative))
        {
            Debug.LogError("Icon path is outside project: " + fullPath);
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<Texture2D>(projectRelative);
    }

}
