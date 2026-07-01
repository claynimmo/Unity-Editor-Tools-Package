using UnityEditor;
using UnityEngine;

public static class ParentTools
{

    private static void MoveObjects(Vector3 parentPos, Transform target){
        int childCount = target.childCount;

        // register undo
        Undo.RecordObject(target, "Moved Parent Pivot");

        for (int i = 0; i < childCount; i++)
            Undo.RecordObject(target.GetChild(i), "Moved Parent Pivot");

        // register the childrens global position
        Vector3[] worldPos = new Vector3[childCount];
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            worldPos[i] = child.position;
        }

        target.position = parentPos;

        // move the children back to the global position
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            child.position = worldPos[i];
        }

    }

    /// <summary>
    /// centres the parent object to the global point computed by the centre pivot option, without moving the children
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Centre to Children", false, 0)]
    public static void CentreParentToChildren(){

        // defensive checks not required due to the validate function
        Transform target = Selection.activeTransform;
        Vector3 centre = target.position;
        
        // calculate the centre by combining the bounds
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds b = renderers[0].bounds;

        for(int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        
        centre = b.center;

        MoveObjects(centre, target);
    }

    [MenuItem("GameObject/Tools/Parent/Centre to Children", true)]
    private static bool CentreParentToChildren_Validate(){
        Transform selected = Selection.activeTransform;
        if(selected == null)
            return false;
        
        return selected.childCount > 0;
    }

    /// <summary>
    /// moves the parent to the selected child, without changing the position of the other children
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Move Parent to This", false, 0)]
    public static void MoveParentToChild(){

        // defensive checks not required due to the validate function
        Transform target = Selection.activeTransform;
        Transform parent = target.parent;

        Vector3 targetPoint = target.position;

        MoveObjects(targetPoint, parent);
    }

    [MenuItem("GameObject/Tools/Parent/Move Parent to This", true)]
    private static bool MoveParentToChild_Validate(){
        Transform selected = Selection.activeTransform;
        if(selected == null)
            return false;
        
        Transform parent = selected.parent;
        if(parent == null)
            return false;
        return parent.childCount > 0;
    }
}
