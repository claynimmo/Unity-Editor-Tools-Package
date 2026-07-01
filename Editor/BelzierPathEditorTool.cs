using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

[EditorTool("Bezier Path Tool", typeof(BezierCurve))]
public class BezierPathEditorTool : EditorTool
{
    GUIContent _icon;

    void OnEnable(){
        _icon = new GUIContent(){
            image = EditorGUIUtility.IconContent("d_EditCollider").image,
            text = "Bezier Path Tool",
            tooltip = "Edit Bezier Path"
        };
    }

    public override GUIContent toolbarIcon => _icon;

    public override void OnToolGUI(EditorWindow window){
        BezierCurve path = (BezierCurve)target;

        if(path == null) return;
        if(path.controlPoints.Count == 0) return;
        

        for(int i = 0; i < path.controlPoints.Count; i++){
            Vector3 worldPos = path.transform.TransformPoint(path.controlPoints[i]);

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            if(EditorGUI.EndChangeCheck()){
                Undo.RecordObject(path, "Move Control Point");
                path.controlPoints[i] = path.transform.InverseTransformPoint(newWorldPos);
            }
        }

        Handles.color = Color.cyan;
        Vector3 prev = path.GetPoint(0f);
        for(int i = 1; i <= 50; i++){
            float t = i / 50f;
            Vector3 curr = path.GetPoint(t);
            Handles.DrawLine(prev, curr);
            prev = curr;
        }

        Handles.color = Color.green;
        Vector3 lastPoint = path.controlPoints[path.controlPoints.Count - 1];
        Vector3 lastWorldPos = path.transform.TransformPoint(lastPoint);
        float size = HandleUtility.GetHandleSize(lastWorldPos) * 0.1f;

        if(Handles.Button(lastWorldPos, path.transform.rotation, size, size, Handles.CubeHandleCap)){
            Undo.RecordObject(path, "Add Point");
            Vector3 dir = (path.controlPoints[Mathf.Max(path.controlPoints.Count - 2, 0)] - lastPoint).normalized;
            int dist = 2;
            if(dir == Vector3.zero)
                path.AddPoint(new Vector3(lastPoint.x + 2, lastPoint.y, lastPoint.z));
            else
                path.AddPoint(lastPoint - dir*dist);
        }
    }
}
