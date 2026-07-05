using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using System.Collections.Generic;

/// <summary> Tool to measure the distance between two points placed from the cursor</summary>
[EditorTool("Measure Tool")]
public class MeasureTool : EditorTool
{
    private MeasureToolWindow window;

    private List<Vector3> markers = new();
    private Vector3 _lastHitPoint;

    private bool _lockY;
    private bool _lockX;
    private bool _lockZ;

    
    public override GUIContent toolbarIcon{
        get{
            Texture2D icon = ToolIconLoader.LoadIcon("Gizmos/Icons/ruler.png");
            return new GUIContent(){
                image = icon,
                text = "Neighbor Tool",
                tooltip = "left click to place, right click to clear, x,y,z to lock/unlock axis. Adjust the value in the popup window to change the threshold (alt+M)"
            };
        }
    }

    public override void OnActivated(){
        window = EditorWindow.GetWindow<MeasureToolWindow>();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public override void OnWillBeDeactivated(){
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    [Shortcut("Tools/Measure Tool", KeyCode.M, ShortcutModifiers.Alt)]
    static void ActivateNeighborTool(){
        ToolManager.SetActiveTool<MeasureTool>();
    }

    void OnSceneGUI(SceneView view){
        Handles.BeginGUI();

        if(markers == null) return;

        for(int i = 1; i < markers.Count; i+=2){
            Vector3 cur = markers[i];
            Vector3 from = markers[i-1];
            float dist = Vector3.Distance(cur, from);
            Vector3 dir = (cur - from).normalized;
            Vector3 textPoint = from + dir * dist/2;

            Vector2 screenPoint = HandleUtility.WorldToGUIPoint(textPoint);
            GUI.Label(
                new Rect(screenPoint.x, screenPoint.y, 300, 20),
                $"{Mathf.Round(dist*1000)/1000}"
            );
        }

        Handles.EndGUI();
    }

    public override void OnToolGUI(EditorWindow sceneWindow){

        if (sceneWindow is not SceneView sceneView){
            return;
        }

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        
        bool isLocked = _lockX || _lockY || _lockZ;

        if (Physics.Raycast(ray, out RaycastHit hit)){

            if(!isLocked){
                _lastHitPoint = hit.point;
            }

            if(e.type == EventType.MouseDown && !e.alt){
                if(e.button == 1)
                    RemoveMarker();
                else
                    PlaceMarker(_lastHitPoint);
                e.Use(); // consume event to prevent selecting objects while painting
            }
        }

        // additional event check for locking, since it may fail the raycast
        if(e.type == EventType.MouseDown && e.button == 0 && isLocked && !e.alt){
            PlaceMarker(_lastHitPoint);
            e.Use();
        }

        if(e.type == EventType.KeyDown){
            if(e.keyCode == KeyCode.Y){
                _lockY = !_lockY;
                _lockX = false;
                _lockZ = false;
                e.Use();
            }
            if(e.keyCode == KeyCode.X){
                _lockX = !_lockX;
                _lockY = false;
                _lockZ = false;
                e.Use();
            }
            if(e.keyCode == KeyCode.Z){
                _lockZ = !_lockZ;
                _lockX = false;
                _lockY = false;
                e.Use();
            }
        }

        bool pendingPlace = markers.Count >=1 && markers.Count % 2 == 1;

        

        if(isLocked && markers.Count >= 1){
            Vector3 axis = GetAxis();
            Vector3 origin = markers[markers.Count - 1];
            Vector3 camForward = SceneView.currentDrawingSceneView.camera.transform.forward;
            Plane plane = new Plane(camForward, origin);

            if (plane.Raycast(ray, out float enter) && pendingPlace){
                Vector3 mouseWorld = ray.GetPoint(enter);


                Vector3 delta = mouseWorld - origin;
                float dist = Vector3.Dot(delta, axis);
                Vector3 constrainedPoint = origin + axis * dist;

                _lastHitPoint = constrainedPoint;
            }
        }
        Handles.color = Color.yellow;

        float sphereSize = 0.1f;
        float threshold = window.threshold;
        for(int i = 1; i < markers.Count; i+=2){
            Handles.color = Color.blue;
            float dist = Vector3.Distance(markers[i], markers[i-1]);
            if(dist > threshold){
                Handles.color = Color.red;
            }
            Handles.DrawAAPolyLine(5f, markers[i], markers[i-1]);
            Vector3 size = new Vector3(0.3f,0.3f,0.3f);
            Handles.DrawWireCube(markers[i], size);
            Handles.DrawWireCube(markers[i-1], size);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawDepthSphere(markers[i], sphereSize);
            DrawDepthSphere(markers[i-1], sphereSize);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        }

        foreach(Vector3 m in markers){
            Handles.color = Color.red;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawDepthSphere(m, sphereSize);
        }
        
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        if(pendingPlace){
            Handles.color = Color.magenta;
            Handles.DrawLine(markers[markers.Count - 1], _lastHitPoint);
            DrawDepthSphere(_lastHitPoint,sphereSize);
        }
    }

    private void PlaceMarker(Vector3 marker){
        if(marker == null) return;
        markers.Add(marker);
        _lockX = false;
        _lockY = false;
        _lockZ = false;
    }

    private void RemoveMarker(){
        markers.Clear();
    }

    private Vector3 GetAxis(){
        if(_lockX)
            return Vector3.right;
        if(_lockY)
            return Vector3.up;
        if(_lockZ)
            return Vector3.forward;
        return Vector3.up;
    }

    private void DrawDepthSphere(Vector3 pos, float radius){
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.DrawWireDisc(pos, SceneView.currentDrawingSceneView.camera.transform.forward, radius);
        Handles.DrawWireDisc(pos, Vector3.up, radius);
        Handles.DrawWireDisc(pos, Vector3.right, radius);
        Handles.DrawWireDisc(pos, Vector3.forward, radius);
    }

}
