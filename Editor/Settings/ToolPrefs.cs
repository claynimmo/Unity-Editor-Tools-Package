using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

static class ToolPrefs
{

    static ColorPref MeasureDefaultColPref = new("Default Colour", "0000FFFF", "Clay_EditorTools.MeasureTool_DefaultCol");
    static ColorPref MeasureMarkerColPref  = new("Marker Colour",   "FF0000FF", "Clay_EditorTools.MeasureTool_MarkerCol");
    static ColorPref MeasurePlacingColPref    = new("Placing Colour",    "FF00FFFF", "Clay_EditorTools.MeasureTool_PlacingCol");
    static ColorPref MeasureThresholdColPref  = new("Threshold Colour",  "FF0000FF", "Clay_EditorTools.MeasureTool_ThresholdCol");

    static ColorPref UIDefaultColPref = new("Default Colour", "FFEC04FF", "Clay_EditorTools.UITool_DefaultCol");
    static ColorPref UIHoverColPref  = new("Hover Colour",   "FFFFFFFF", "Clay_EditorTools.UITool_HoverCol");
    static ColorPref UIDragColPref    = new("Drag Colour",    "00FF00FF", "Clay_EditorTools.UITool_DragCol");
    static ColorPref UIBoundsColPref  = new("Bounds Colour",  "808080FF", "Clay_EditorTools.UITool_BoundsCol");

    public static Color MeasureDefaultCol   => MeasureDefaultColPref.Load();
    public static Color MeasureMarkerCol    => MeasureMarkerColPref.Load();
    public static Color MeasurePlacingCol   => MeasurePlacingColPref.Load();
    public static Color MeasureThresholdCol => MeasureThresholdColPref.Load();

    public static Color UIDefaultCol => UIDefaultColPref.Load();
    public static Color UIHoverCol   => UIHoverColPref.Load();
    public static Color UIDragCol    => UIDragColPref.Load();
    public static Color UIBoundsCol  => UIBoundsColPref.Load();

    public static void SetPrefs(){

        List<ColorPref> measColPrefs = new List<ColorPref>();
        measColPrefs.Add(MeasureDefaultColPref);
        measColPrefs.Add(MeasurePlacingColPref);
        measColPrefs.Add(MeasureThresholdColPref);
        measColPrefs.Add(MeasureMarkerColPref);

        List<ColorPref> uiColPrefs = new List<ColorPref>();
        uiColPrefs.Add(UIDefaultColPref);
        uiColPrefs.Add(UIHoverColPref);
        uiColPrefs.Add(UIDragColPref);
        uiColPrefs.Add(UIBoundsColPref);
        
        EditorGUILayout.LabelField("Measure Tool", EditorStyles.boldLabel);

        DisplayColPrefSection(measColPrefs);

        EditorGUILayout.LabelField("UI Tool", EditorStyles.boldLabel);
        
        DisplayColPrefSection(uiColPrefs);
    }

    private static void DisplayColPrefSection(List<ColorPref> prefs){
        EditorGUI.BeginChangeCheck();
         if(GUILayout.Button("Reset to Default")){
            ResetColorPrefs(prefs);
            GUI.changed = true;
        }

        
        Color[] mCols = new Color[prefs.Count];
        for(int i = 0; i < mCols.Length; i++){
            mCols[i] = prefs[i].Draw();
        }

        if (EditorGUI.EndChangeCheck()){
            for(int i = 0; i < prefs.Count; i++)
                prefs[i].Save(mCols[i]);
        }
    }
    public static void ResetColorPrefs(List<ColorPref> prefs){
        foreach(ColorPref pref in prefs)
            pref.Reset();
    }
}

