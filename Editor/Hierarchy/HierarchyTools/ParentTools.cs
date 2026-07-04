using UnityEditor;
using UnityEngine;

using System;

public static class ParentTools
{

    private static void MoveObjects(Vector3 parentPos, Transform target, Func<Transform, bool>? func = null){
        int childCount = target.childCount;

        // register undo
        Undo.RecordObject(target, "Moved Parent Pivot");

        for (int i = 0; i < childCount; i++)
            Undo.RecordObject(target.GetChild(i), "Moved Parent Pivot");

        // register the childrens global position
        Vector3[] worldPos = new Vector3[childCount];
        Quaternion[] worldRot = new Quaternion[childCount];
        Vector3[] worldScale = new Vector3[childCount];
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            worldPos[i] = child.position;
            worldRot[i] = child.rotation;
            worldScale[i] = child.lossyScale;
        }

        target.position = parentPos;

        func?.Invoke(target);

        // move the children back to the global position
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            child.position = worldPos[i];
            child.rotation = worldRot[i];
            SetWorldScale(child, worldScale[i]);
        }

    }

    private static void SetWorldScale(Transform t, Vector3 worldScale){
        Transform parent = t.parent;

        if(parent != null){
            Vector3 parentScale = parent.lossyScale;
            t.localScale = new Vector3(
                worldScale.x / parentScale.x,
                worldScale.y / parentScale.y,
                worldScale.z / parentScale.z
            );
        }
        else{
            t.localScale = worldScale;
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

    /// <summary>
    /// swaps the child with the parent, conserving the relationship with the other children
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Swap With Parent", false, 0)]
    public static void SwapWithParent(){

        Transform target = Selection.activeTransform;
        Transform parent = target.parent;
        Transform grandParent = parent.parent; // can be null

        // capture full tree for undo
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Swap With Parent");

        // cache original child indexing, to keep hierarchal structure
        int parentIndex = parent.GetSiblingIndex();
        int targetIndex = target.GetSiblingIndex();

        // cache the children
        int childCount = parent.childCount;
        Transform[] children = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
            children[i] = parent.GetChild(i);

        // cache the children of the swapped component, to make sure grandchildren are not promoted to standard children when the swap is made
        int grandChildCount = target.childCount;
        Transform[] grandChildren = new Transform[grandChildCount];
        for (int i = 0; i < grandChildCount; i++)
            grandChildren[i] = target.GetChild(i);

        Undo.SetTransformParent(target, grandParent, "Swap With Parent");

        // perform the swap
        target.SetSiblingIndex(parentIndex);
        Undo.SetTransformParent(parent, target, "Swap With Parent");

        // move the original children under the new parent
        foreach (Transform c in children){
            if (c == target) continue;
            Undo.SetTransformParent(c, target, "Swap With Parent");
        }

        // move grand children to its new parent, preserving the relationship it has with its previous parent
        foreach (Transform c in grandChildren){
            Undo.SetTransformParent(c, parent, "Swap With Parent");
        }

        // make the parents position in the children the same point as the original object it swapped with
        parent.SetSiblingIndex(targetIndex);
        
    }

    [MenuItem("GameObject/Tools/Parent/Swap With Parent", true)]
    private static bool SwapWithParent_Validate(){
        return MoveParentToChild_Validate();
    }

    /// <summary>
    /// resets the scale of the parent, without impacting the children
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Reset Scale (not effecting children)", false, 0)]
    public static void ResetScale(){

        Transform target = Selection.activeTransform;
        bool F(Transform t){ // func has to be bool, since void return type is not allowed
            t.localScale = Vector3.one;
            return true;
        }
        MoveObjects(target.position,target,F);
    }

    [MenuItem("GameObject/Tools/Parent/Reset Scale (not effecting children)", true)]
    private static bool ResetScale_Validate(){
        return Selection.activeTransform != null;
    }

    /// <summary>
    /// resets the rotation of the parent, without impacting the children
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Reset Rotation (not effecting children)", false, 0)]
    public static void ResetRotation(){

        Transform target = Selection.activeTransform;
        bool F(Transform t){ // func has to be bool, since void return type is not allowed
            t.localRotation = Quaternion.Euler(Vector3.zero);
            return true;
        }
        MoveObjects(target.position,target,F);
    }

    [MenuItem("GameObject/Tools/Parent/Reset Rotation (not effecting children)", true)]
    private static bool ResetRotation_Validate(){
        return ResetScale_Validate();
    }


    // helper function to perform the set active function
    private static void SetChildrenActive(bool isActive, Transform target){
        int childCount = target.childCount;
        for(int i = 0; i < childCount; i++){
            Transform child = target.GetChild(i);
            Undo.RegisterCompleteObjectUndo(child.gameObject, "Changed Active");
            child.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// disables all children of the selected object
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Set Active/Disable Children", false, 0)]
    public static void DisableChildren(){
        SetChildrenActive(false, Selection.activeTransform);
    }


    [MenuItem("GameObject/Tools/Parent/Set Active/Disable Children", true)]
    private static bool DisableChildren_Validate(){
        return CentreParentToChildren_Validate();
    }

    /// <summary>
    /// enables all children of the selected object
    /// </summary>
    [MenuItem("GameObject/Tools/Parent/Set Active/Enable Children", false, 0)]
    public static void EnableChildren(){
        SetChildrenActive(true, Selection.activeTransform);
    }


    [MenuItem("GameObject/Tools/Parent/Set Active/Enable Children", true)]
    private static bool EnableChildren_Validate(){
        return CentreParentToChildren_Validate();
    }
}
