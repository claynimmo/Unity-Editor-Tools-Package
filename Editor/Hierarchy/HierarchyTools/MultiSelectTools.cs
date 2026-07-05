using UnityEditor;
using UnityEngine;

public static class MultiSelectTools 
{

    private static bool _executed = false; // flag to prevent the editor action from being called once per object, rather than only once

    private static bool HasSelected(){
        return Selection.gameObjects.Length > 0;
    }

    private static bool HasMultipleSelected(){
        return Selection.gameObjects.Length >= 1;
    }

    /// <summary>
    /// saves the current selected list of objects to a static cache
    /// </summary>
    [MenuItem("GameObject/Tools/Selection/Save Selected Objects %&s", false, 0)]
    private static void SaveSelectedObjects(){
        if(_executed) return;

        _executed = true;

        SelectionCache.SaveCurrentSelection();
        EditorUtility.DisplayDialog("Saved", "Stored selected GameObjects.", "OK");

        EditorApplication.delayCall += () => _executed = false;
    }

    [MenuItem("GameObject/Tools/Selection/Save Selected Objects", true)]
    private static bool SaveSelectedObjects_Validate(){
        return HasSelected();
    }
}
