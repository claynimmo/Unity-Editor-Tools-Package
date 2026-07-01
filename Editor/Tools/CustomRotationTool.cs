using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

///
/// <summary> Custom rotation tool that gives a button in the center to rotate randomly, for placing objects like rocks </summary>
/// 
[EditorTool("Rotate Tool")]
public class CustomRotationTool : EditorTool
{
    public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("RotateTool");


    public override void OnToolGUI(EditorWindow window){
        Transform t = Selection.activeTransform;
        if (!t) return;

        EditorGUI.BeginChangeCheck();

        // Draw rotation handle
        Quaternion newRot = Handles.RotationHandle(t.rotation, t.position);

        if(EditorGUI.EndChangeCheck()){
            Undo.RecordObject(t, "Rotate");
            t.rotation = newRot;
        }

        Color c;
        ColorUtility.TryParseHtmlString("#7d111190", out c);
        Handles.color = c;

        float size = HandleUtility.GetHandleSize(t.position) * 0.2f;

        if(Handles.Button(t.position, t.rotation, size, size, Handles.CubeHandleCap)){
            Undo.RecordObject(t, "Random Rotate");
            t.rotation = Quaternion.Euler(new Vector3(Random.Range(0,360),Random.Range(0,360), Random.Range(0,360)));
        }
    }
}
