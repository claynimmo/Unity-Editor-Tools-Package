using UnityEditor;
using UnityEngine;

static class ToolPrefs
{

    static ColorPref UIDefaultColPref = new("Default Colour", "FFEC04FF", "Clay_EditorTools.UITool_DefaultCol");
    static ColorPref UIHoverColPref  = new("Hover Colour",   "FFFFFFFF", "Clay_EditorTools.UITool_HoverCol");
    static ColorPref UIDragColPref    = new("Drag Colour",    "00FF00FF", "Clay_EditorTools.UITool_DragCol");
    static ColorPref UIBoundsColPref  = new("Bounds Colour",  "808080FF", "Clay_EditorTools.UITool_BoundsCol");

    public static Color UIDefaultCol => UIDefaultColPref.Load();
    public static Color UIHoverCol   => UIHoverColPref.Load();
    public static Color UIDragCol    => UIDragColPref.Load();
    public static Color UIBoundsCol  => UIBoundsColPref.Load();

    public static void SetPrefs(){
        
        
        EditorGUILayout.LabelField("UI Tool", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        
        if(GUILayout.Button("Reset to Default")){
            ResetToDefaultUI();
            GUI.changed = true;
        }
        

        Color newDefault = UIDefaultColPref.Draw();
        Color newHover   = UIHoverColPref.Draw();
        Color newDrag    = UIDragColPref.Draw();
        Color newBounds  = UIBoundsColPref.Draw();
     

        if(EditorGUI.EndChangeCheck()){
            UIDefaultColPref.Save(newDefault);
            UIHoverColPref.Save(newHover);
            UIDragColPref.Save(newDrag);
            UIBoundsColPref.Save(newBounds);
        }
    }

    public static void ResetToDefaultUI(){
        UIDefaultColPref.Reset();
        UIHoverColPref.Reset();
        UIDragColPref.Reset();
        UIBoundsColPref.Reset();
    }
}

