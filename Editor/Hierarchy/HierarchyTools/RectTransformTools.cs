using UnityEditor;
using UnityEngine;

using System;


public static class RectTransformTools
{
    private static void Translate(RectTransform t, Vector3 pos){
        Undo.RecordObject(t, "Translated Transform");
        t.anchoredPosition = new Vector2(pos.x, pos.y);
    }

    private static bool Validate(){
        Transform t = Selection.activeTransform;
        if(t == null) return false;
        if(t.GetType() != typeof(RectTransform)) return false;

        return true;
    }

    /// <summary>
    /// Move the selected object by inverting the x position
    /// </summary>
    [MenuItem("GameObject/Tools/Rect Transform/Translate Flip X", false, 0)]
    public static void TranslateFlipX(){
        RectTransform t = (RectTransform)Selection.activeTransform;
        Translate(t, new Vector3(-t.anchoredPosition.x, t.anchoredPosition.y, 0));
    }

    /// <summary>
    /// Move the selected object by inverting the y position
    /// </summary>
    [MenuItem("GameObject/Tools/Rect Transform/Translate Flip Y", false, 0)]
    public static void TranslateFlipY(){
        RectTransform t = (RectTransform)Selection.activeTransform;
        Translate(t, new Vector3(t.anchoredPosition.x, -t.anchoredPosition.y, 0));
    }

    [MenuItem("GameObject/Tools/Rect Transform/Translate Flip X", true)]
    [MenuItem("GameObject/Tools/Rect Transform/Translate Flip Y", true)]
    private static bool TranslateFlip_Validate(){
        return Validate();
    }
}
