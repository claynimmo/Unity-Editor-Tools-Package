using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class MeasureToolWindow : EditorWindow
{
    public float threshold = 3;
    

    [MenuItem("Tools/Measure Tool Window")]
    public static void OpenWindow(){
        MeasureToolWindow window = GetWindow<MeasureToolWindow>();

        window.minSize = new Vector2(300, 200);
        window.Show();
    }

    void OnDisable(){
        // Check if EditorTool is currently active and disable it when window is closed
        if(ToolManager.activeToolType == typeof(MeasureTool)){
            // Try to activate previously used tool
            ToolManager.RestorePreviousPersistentTool();
        }
    }

    void OnGUI(){


        threshold = EditorGUILayout.FloatField("Threshold", threshold);
    }
}