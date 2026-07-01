using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEditor.ShortcutManagement;
/// <summary> 
/// Gizmo like the move tool to perfectly snap duplicate objects together, helping build walls without overlapping UVs. It does not work for planes, due to the normals being 0.
/// The tool draws a bounding box around the mesh, and connects objects by snapping the faces of this bounding box together.
/// Multiselection is not supported, but similar functionality can be achieved by using a parent object. It creates a bounding box for every child
/// </summay>
[EditorTool("Neighbor Tool", typeof(Transform))]
public class NeighborTool : EditorTool
{

    // override the toolbar content to set the custom icon, text, and description
    public override GUIContent toolbarIcon{
        get{
           
            Texture2D icon = ToolIconLoader.LoadIcon("Gizmos/Icons/neighbortool.png");
            return new GUIContent(){
                image = icon,
                text = "Neighbor Tool",
                tooltip = "Perfectly place neighbors from the bounding box"
            };
        }
    }

    // shortcut to neighbours when pressing n
    [Shortcut("Tools/Neighbor Tool", KeyCode.N)]
    static void ActivateNeighborTool(){
        var t = Selection.activeTransform;
        if (t != null)
            ToolManager.SetActiveTool<NeighborTool>();
    }

    public override void OnToolGUI(EditorWindow window){
        if (!Selection.activeTransform) return; // do nothing if no object is selected

        // set the color to something visible, but different to the default selection and colliders
        Color c;
        ColorUtility.TryParseHtmlString("#ff00e6", out c);
        Handles.color = c;

        DrawPerChildBounds(Selection.activeTransform);
    }

    void DrawPerChildBounds(Transform root){
        MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>();

        foreach (var mf in filters){
            if (!mf.sharedMesh) continue;

            Mesh mesh = mf.sharedMesh;
            Bounds b = mesh.bounds;

            // 8 local corners
            /* indexes correspond to these vertices (front face is closest, top is the top)
                  7 ---------- 6
                 /|           /|
                3 ---------- 2 |
                | |          | |
                | 4 ---------|-5
                |/           |/
                0 ---------- 1
            */
            Vector3[] c = new Vector3[8];
            c[0] = new Vector3(b.min.x, b.min.y, b.min.z);
            c[1] = new Vector3(b.max.x, b.min.y, b.min.z);
            c[2] = new Vector3(b.max.x, b.max.y, b.min.z);
            c[3] = new Vector3(b.min.x, b.max.y, b.min.z);
            c[4] = new Vector3(b.min.x, b.min.y, b.max.z);
            c[5] = new Vector3(b.max.x, b.min.y, b.max.z);
            c[6] = new Vector3(b.max.x, b.max.y, b.max.z);
            c[7] = new Vector3(b.min.x, b.max.y, b.max.z);

            // to world
             for (int i = 0; i < 8; i++)
                c[i] = mf.transform.TransformPoint(c[i]);

            // edges
            Handles.DrawLine(c[0], c[1]);
            Handles.DrawLine(c[1], c[2]);
            Handles.DrawLine(c[2], c[3]);
            Handles.DrawLine(c[3], c[0]);

            Handles.DrawLine(c[4], c[5]);
            Handles.DrawLine(c[5], c[6]);
            Handles.DrawLine(c[6], c[7]);
            Handles.DrawLine(c[7], c[4]);

            Handles.DrawLine(c[0], c[4]);
            Handles.DrawLine(c[1], c[5]);
            Handles.DrawLine(c[2], c[6]);
            Handles.DrawLine(c[3], c[7]);

            // faces
            Vector3[][] faces = new Vector3[][]
            {
                new [] { c[0], c[1], c[2], c[3] }, // front
                new [] { c[4], c[5], c[6], c[7] }, // back
                new [] { c[0], c[1], c[5], c[4] }, // bottom
                new [] { c[3], c[2], c[6], c[7] }, // top
                new [] { c[0], c[3], c[7], c[4] }, // left
                new [] { c[1], c[2], c[6], c[5] }  // right
            };
            
            // add arrows to each face, allowing the neighbouring
            foreach(var face in faces){
                DrawFaceArrow(face, c,root,mf.gameObject.transform);
            }
        }
    }

    void DrawFaceArrow(Vector3[] face, Vector3[] allCorners, Transform root, Transform currentObject){
        // face center (sum all corners, and divide by 4)
        Vector3 center = (face[0] + face[1] + face[2] + face[3]) / 4;

        // face normal
        Vector3 normal = Vector3.Cross(face[1] - face[0], face[3] - face[0]).normalized;
        Vector3 toRoot = (center - currentObject.position).normalized;

        if (normal == Vector3.zero)
            return;

        // ensure outward direction relative to root
        if(Vector3.Dot(normal, center - root.position) < 0)
            normal = -normal;

        float size = HandleUtility.GetHandleSize(center) * 0.4f;

        if(Handles.Button(center,Quaternion.LookRotation(normal),size,size,Handles.ArrowHandleCap)){
            float thickness = ComputeThicknessAlongNormal(allCorners, normal);
            DuplicateAndSnap(root, normal, thickness);
        }
    }

    // thickness determines how to position the duplicate
    float ComputeThicknessAlongNormal(Vector3[] corners, Vector3 normal){
        float minProj = float.MaxValue;
        float maxProj = float.MinValue;

        for(int i = 0; i < corners.Length; i++){
            float p = Vector3.Dot(corners[i], normal); // project the corner onto the normal
            if (p < minProj) minProj = p;
            if (p > maxProj) maxProj = p;
        }

        // depth along face normal is the max projection of the 8 corners onto that face normal, minus the minimum projection
        return maxProj - minProj;
    }

    void DuplicateAndSnap(Transform original, Vector3 normal, float thickness){
        Transform clone = Object.Instantiate(original.gameObject).transform;
        Undo.RegisterCreatedObjectUndo(clone.gameObject, "Duplicate Neighbor");
        clone.name = original.name; // overide the name to avoid having (clone)(clone)(clone)... for consecutive duplications

        clone.position = original.position + normal * thickness;
        clone.SetParent(original.parent);
        Selection.activeTransform = clone;
    }
}
