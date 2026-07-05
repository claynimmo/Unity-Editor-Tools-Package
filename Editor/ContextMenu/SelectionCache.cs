using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Static class that encapsulates the stored selected objects
/// </summary>
public static class SelectionCache
{
    public static List<GameObject> SavedSelection = new();

    /// <summary>
    /// Saves all selected gameobjects into the static list
    /// </summary>
    public static void SaveCurrentSelection(){
        SavedSelection = Selection.gameObjects.ToList();
    }
}
