using UnityEditor;
using UnityEngine;

public static class ParentTools
{
    /// <summary>
    /// centres the parent object to the global point computed by the centre pivot option, without moving the children
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Centre to Children", false, 0)]
    public static void CentreParent(){

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

        int childCount = target.childCount;

        // register the childrens global position
        Vector3[] worldPos = new Vector3[childCount];
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            worldPos[i] = child.position;
        }

        target.position = centre;

        // move the children back to the global position
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            child.position = worldPos[i];
        }
    }

    [MenuItem("GameObject/Tools/Parent/Centre to Children", true)]
    private static bool CentreParent_Validate(){
        Transform selected = Selection.activeTransform;
        if(selected == null)
            return false;
        
        return selected.childCount > 0;
    }
}
