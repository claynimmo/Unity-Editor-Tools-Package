using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEditor.ShortcutManagement;

///
/// <summary> Custom move tool that gives movement gizmos to every individual child of the selected object at once, making it especially easier to move invisible walls </summary>
/// 
[EditorTool("Child Move Tool")]
public class ChildMoveTool : EditorTool
{
    
    public override GUIContent toolbarIcon{
        get{
            var content = EditorGUIUtility.IconContent("MoveTool");
            content.tooltip = "Child Move Tool (Alt+X)";
            return content;
        }
    }

    [Shortcut("Tools/Child Move Tool", KeyCode.X, ShortcutModifiers.Alt)]
    static void ActivateMoveTool(){
        ToolManager.SetActiveTool<ChildMoveTool>();
    }

    public override void OnToolGUI(EditorWindow window){
        Transform t = Selection.activeTransform;
        if (!t) return;

        foreach (Transform obj in t){
            // X axis (cyan)
            Handles.color = Color.cyan;

            float scale = HandleUtility.GetHandleSize(obj.position) * 0.5f;
            Vector3 newPosX = Handles.Slider(
                obj.position,
                Axis(obj, Vector3.right),
                scale,
                Handles.ArrowHandleCap,
                0f
            );

            // Y axis (magenta)
            Handles.color = Color.magenta;
            Vector3 newPosY = Handles.Slider(
                obj.position,
                Axis(obj, Vector3.up),
                scale,
                Handles.ArrowHandleCap,
                0f
            );

            // Z axis (yellow)
            Handles.color = Color.yellow;
            Vector3 newPosZ = Handles.Slider(
                obj.position,
                Axis(obj, Vector3.forward),
                scale,
                Handles.ArrowHandleCap,
                0f
            );

            // apply movement
            if(newPosX != obj.position){
                Undo.RecordObject(obj, "Move X");
                obj.position = newPosX;
            }
            else if(newPosY != obj.position){
                Undo.RecordObject(obj, "Move Y");
                obj.position = newPosY;
            }
            else if(newPosZ != obj.position){
                Undo.RecordObject(obj, "Move Z");
                obj.position = newPosZ;
            }
        }

    

        EditorGUI.BeginChangeCheck();

        Quaternion handleRotation =
            Tools.pivotRotation == PivotRotation.Local
                ? t.rotation
                : Quaternion.identity;

        Vector3 newPos = Handles.PositionHandle(t.position, handleRotation);

        if (EditorGUI.EndChangeCheck()){
            Undo.RecordObject(t, "Move Parent");
            t.position = newPos;
        }
    }

    Vector3 Axis(Transform obj, Vector3 localAxis){
        return Tools.pivotRotation == PivotRotation.Local
            ? obj.TransformDirection(localAxis)   // local mode
            : localAxis;                          // global mode
    }
}
