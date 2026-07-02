using UnityEditor;
using UnityEngine;

using System;

public static class TransformTools
{
    private static readonly float delta = 0.0001f;

    private static void Scale(Transform t, Vector3 shift){
        Undo.RecordObject(t, "Scaled transform");
        t.localScale = t.localScale + shift;
    }

    private static void Shift(Transform t, Vector3 shift){
        Undo.RecordObject(t, "Shifted transform");
        t.localPosition = t.localPosition+shift;
    }

    private static bool Validate(){
        return Selection.activeTransform != null;
    }

    /// <summary>
    /// uniformly scale up the transform by such a small margin that the only noticable impact is avoiding overlapping UVs
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Scale (Enlarge)", false, 0)]
    public static void UVScaleUp(){
        Scale(Selection.activeTransform, new Vector3(delta,delta,delta));
    }

    [MenuItem("GameObject/Tools/Transform/UV Scale (Enlarge)", true)]
    private static bool UVScaleUp_Validate(){
        return Validate();
    }
    /// <summary>
    /// uniformly scale down the transform by such a small margin that the only noticable impact is avoiding overlapping UVs
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Scale (Shrink)", false, 0)]
    public static void UVScaleDown(){
        Scale(Selection.activeTransform, new Vector3(-delta,-delta,-delta));
    }

    [MenuItem("GameObject/Tools/Transform/UV Scale (Shrink)", true)]
    private static bool UVScaleDown_Validate(){
        return Validate();
    }

    /// <summary>
    /// shift the transform locally down
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Down)", false, 0)]
    public static void UVShiftDown(){
        Transform t = Selection.activeTransform;
        Shift(Selection.activeTransform, -t.up * delta);
    }

    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Down)", true)]
    private static bool UVShiftDown_Validate(){
        return Validate();
    }

    /// <summary>
    /// shift the transform locally up
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Up)", false, 0)]
    public static void UVShiftUp(){
        Transform t = Selection.activeTransform;
        Shift(Selection.activeTransform, t.up * delta);
    }

    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Up)", true)]
    private static bool UVShiftUp_Validate(){
        return Validate();
    }

    /// <summary>
    /// shift the transform locally forward
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Forward)", false, 0)]
    public static void UVShiftForward(){
        Transform t = Selection.activeTransform;
        Shift(Selection.activeTransform, t.forward * delta);
    }

    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Forward)", true)]
    private static bool UVShiftForward_Validate(){
        return Validate();
    }

    /// <summary>
    /// shift the transform locally back
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Back)", false, 0)]
    public static void UVShiftBack(){
        Transform t = Selection.activeTransform;
        Shift(Selection.activeTransform, -t.forward * delta);
    }

    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Back)", true)]
    private static bool UVShiftBack_Validate(){
        return Validate();
    }

    /// <summary>
    /// shift the transform locally left
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Left)", false, 0)]
    public static void UVShiftLeft(){
        Transform t = Selection.activeTransform;
        Shift(Selection.activeTransform, -t.right * delta);
    }

    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Left)", true)]
    private static bool UVShiftLeft_Validate(){
        return Validate();
    }

    /// <summary>
    /// shift the transform locally right
    /// </summary>
    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Right)", false, 0)]
    public static void UVShiftRight(){
        Transform t = Selection.activeTransform;
        Shift(Selection.activeTransform, t.right * delta);
    }

    [MenuItem("GameObject/Tools/Transform/UV Local Shift (Right)", true)]
    private static bool UVShiftRight_Validate(){
        return Validate();
    }

}
