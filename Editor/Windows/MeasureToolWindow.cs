using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// popup window to adjust the settings of the measure editor tool
/// </summary>
public class MeasureToolWindow : EditorToolWindow
{
    public float threshold = 3;

    [MenuItem("Tools/Measure Tool Window")]
    public static void OpenWindow(){
        MeasureToolWindow window = GetWindow<MeasureToolWindow>();
        AdjustWindow(window);
    }

    void OnGUI(){
        threshold = EditorGUILayout.FloatField("Threshold", threshold);
    }
}