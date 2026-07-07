using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class GuideLine{
    public enum Orientation { Horizontal, Vertical }

    public Orientation orientation;
    public float position;
}

[EditorTool("UI Positioning Tool")]
public class UIPositioningTool : EditorTool
{
    private static List<GuideLine> lines = new List<GuideLine>();

    private static List<RectTransform> _transformCache = new List<RectTransform>();

    private static Color _hoverCol = Color.white;
    private static Color _dragCol = Color.green;
    private static Color _defaultCol = Color.yellow;
    private static Color _boundsCol = Color.grey;

    private static bool _alreadyIn2D;

    public override GUIContent toolbarIcon{
        get{
            var content = EditorGUIUtility.IconContent("Grid.BoxTool");
            content.tooltip = "UI Positioning Tool";
            return content;
        }
    }

    [InitializeOnLoadMethod]
    private static void Init(){
        EditorApplication.hierarchyChanged += UpdateTransformCache;
    }

    public override void OnActivated(){
        SceneView sceneView = SceneView.lastActiveSceneView;

        if(sceneView == null) return;

        _alreadyIn2D = sceneView.in2DMode;

        _transformCache = FindObjectsOfType<RectTransform>().ToList();

        sceneView.in2DMode = true;
        sceneView.Repaint();
    }

    public override void OnWillBeDeactivated(){
        SceneView sceneView = SceneView.lastActiveSceneView;

        if(sceneView == null) return;

        sceneView.in2DMode = _alreadyIn2D;

        sceneView.Repaint();
    }



    public override void OnToolGUI(EditorWindow window){
        var sceneView = window as SceneView;
        if (sceneView == null) return;

        var canvas = GetCanvasTransform();
        if (!canvas) return;

        const float startPos = 100;
        if(lines.Count == 0){
            AddLine(GuideLine.Orientation.Horizontal, startPos);
            AddLine(GuideLine.Orientation.Horizontal, -startPos);
            AddLine(GuideLine.Orientation.Vertical, startPos);
            AddLine(GuideLine.Orientation.Vertical, -startPos);
        }

        DrawGuideLines(sceneView, canvas);
        DrawGizmoForSelected();
        SnapSelectedRect(canvas);
        DrawBounds(canvas);
    }

    private static void UpdateTransformCache(){
        _transformCache = FindObjectsOfType<RectTransform>().ToList();
    }

    public static void AddLine(GuideLine.Orientation orientation, float pos = 0){
        Transform canvasTf = GetCanvasTransform();
        if (canvasTf == null) return;

        lines.Add(new GuideLine{
            orientation = orientation,
            position = pos
        });

        SceneView.RepaintAll();
    }

    private static void SnapSelectedRect(Transform canvas){

        GameObject selectedObj = Selection.activeGameObject;
        if(!selectedObj) return;

        RectTransform rt = selectedObj.GetComponent<RectTransform>();
        if(!rt) return;

        Vector3 worldPos = rt.position;

        EditorGUI.BeginChangeCheck();
        Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

        bool changed = EditorGUI.EndChangeCheck();

        if (changed){
            Undo.RecordObject(rt, "Move UI Element");
            rt.position = newWorldPos;
            SceneView.RepaintAll();
        }

        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        for (int i = 0; i < 4; i++)
            corners[i] = canvas.InverseTransformPoint(corners[i]);

        float left   = corners[0].x;
        float right  = corners[2].x;
        float top    = corners[1].y;
        float bottom = corners[0].y;

        Vector3 localPivot = canvas.InverseTransformPoint(rt.position);

        float snappedX = localPivot.x;
        float snappedY = localPivot.y;

        float snapThreshold = 6f;

        foreach (var line in lines){
            if(line.orientation == GuideLine.Orientation.Vertical){
                float distLeft  = Mathf.Abs(left  - line.position);
                float distRight = Mathf.Abs(right - line.position);

                if(distLeft < snapThreshold || distRight < snapThreshold){
                    float target = distLeft < distRight ? left : right;
                    float delta  = line.position - target;
                    snappedX += delta;
                }
            }
            else{
                float distTop    = Mathf.Abs(top    - line.position);
                float distBottom = Mathf.Abs(bottom - line.position);

                if(distTop < snapThreshold || distBottom < snapThreshold){
                    float target = distTop < distBottom ? top : bottom;
                    float delta  = line.position - target;
                    snappedY += delta;
                }
            }
        }

        Vector3 snappedWorld = canvas.TransformPoint(
            new Vector3(snappedX, snappedY, localPivot.z)
        );

        Undo.RecordObject(rt, "Snap UI Element To Guide Lines");
        rt.position = snappedWorld;

        SceneView.RepaintAll();
    }

    private static void DrawGuideLines(SceneView view, Transform canvas){
        Event e = Event.current;

        foreach (var line in lines){
            DrawGuideLine(line, canvas, e);
        }
    }

    private static void DrawGuideLine(GuideLine line, Transform canvas, Event e){
        Vector3 localStart, localEnd;

        if(line.orientation == GuideLine.Orientation.Vertical){
            localStart = new Vector3(line.position, -10000f, 0f);
            localEnd   = new Vector3(line.position,  10000f, 0f);
        }
        else{
            localStart = new Vector3(-10000f, line.position, 0f);
            localEnd   = new Vector3( 10000f, line.position, 0f);
        }

        Vector3 worldStart = canvas.TransformPoint(localStart);
        Vector3 worldEnd   = canvas.TransformPoint(localEnd);


        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        Vector2 guiStart = HandleUtility.WorldToGUIPoint(worldStart);
        Vector2 guiEnd   = HandleUtility.WorldToGUIPoint(worldEnd);

        const float guiThickness = 5f;

        float guiDist = HandleUtility.DistancePointToLineSegment(e.mousePosition, guiStart, guiEnd);

        bool isHovering = guiDist < guiThickness && GUIUtility.hotControl == 0;
        bool isDragging = GUIUtility.hotControl == controlId;

        Color handleCol = _defaultCol;

        if (isHovering){
            handleCol = _hoverCol;
        }
        if (isDragging){
            handleCol = _dragCol;
        }

        Handles.color = handleCol;
        Handles.DrawLine(worldStart, worldEnd);

        switch (e.type){
            case EventType.Layout:
                if (guiDist < guiThickness)
                    HandleUtility.AddControl(controlId, guiDist);
                break;

            case EventType.MouseDown:
                if(e.button == 0 && isHovering){
                    GUIUtility.hotControl = controlId;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if(GUIUtility.hotControl == controlId){
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Plane canvasPlane = new Plane(canvas.forward, canvas.position);

                    Handles.color = Color.blue;
                    Handles.DrawLine(worldStart, worldEnd);

                    if(canvasPlane.Raycast(ray, out float dist)){
                        Vector3 worldPoint = ray.GetPoint(dist);
                        Vector3 local = canvas.InverseTransformPoint(worldPoint);

                        if (line.orientation == GuideLine.Orientation.Vertical)
                            line.position = local.x;
                        else
                            line.position = local.y;

                        TrySnapLineToUI(line,canvas);

                        SceneView.RepaintAll();
                        e.Use();
                    }
                }
                break;

            case EventType.MouseUp:
                if(GUIUtility.hotControl == controlId){
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            default:
                break;
        }
    }


    private static void DrawGizmoForSelected(){
        GameObject selectedObj = Selection.activeGameObject;
        if(!selectedObj) return;

        var rt = selectedObj.GetComponent<RectTransform>();
        if(!rt) return;

        Vector3 worldPos = rt.position;

        EditorGUI.BeginChangeCheck();
        Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

        if(EditorGUI.EndChangeCheck()){
            Undo.RecordObject(rt, "Move UI Element");
            rt.position = newWorldPos;
            SceneView.RepaintAll();
        }
    }

    private static void DrawBounds(Transform canvas){
        foreach(RectTransform rt in  _transformCache){
            if(!rt.IsChildOf(canvas)) continue;

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            Handles.color = _boundsCol;
            Handles.DrawLine(corners[0],corners[1]);
            Handles.DrawLine(corners[2],corners[1]);
            Handles.DrawLine(corners[0],corners[3]);
            Handles.DrawLine(corners[3],corners[2]);
        }
    }

    private static void TrySnapLineToUI(GuideLine line, Transform canvas){
        float snapThreshold = 4f;

        // search the bounds of every rect transform
        foreach(RectTransform rt in _transformCache){
            if(!rt.IsChildOf(canvas)) continue;

            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

            for(int i = 0; i < 4; i++)
                corners[i] = canvas.InverseTransformPoint(corners[i]);

            float left   = corners[0].x;
            float right  = corners[2].x;
            float top    = corners[1].y;
            float bottom = corners[0].y;


            if(line.orientation == GuideLine.Orientation.Vertical){
                if (Mathf.Abs(line.position - left) < snapThreshold)
                    line.position = left;
                else if (Mathf.Abs(line.position - right) < snapThreshold)
                    line.position = right;
            }
            else{
                if (Mathf.Abs(line.position - top) < snapThreshold)
                    line.position = top;
                else if (Mathf.Abs(line.position - bottom) < snapThreshold)
                    line.position = bottom;
            }
        }
    }

    private static Transform GetCanvasTransform(){
        GameObject selectedObj = Selection.activeGameObject;

        // get the canvas of the selected object
        if (selectedObj != null){
            Canvas canvas = selectedObj.GetComponentInParent<Canvas>();
            if (canvas != null)
                return canvas.transform;
        } 

        // get the first canvas as a fallback
        Canvas c = FindObjectOfType<Canvas>();
        if (c != null)
            return c.transform;

        return null;
    }
}