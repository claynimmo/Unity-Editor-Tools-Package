using UnityEditor;
using UnityEngine;

static class Preferences
{

    [SettingsProvider]
    public static SettingsProvider CreateHierarchyProvider(){
        return new SettingsProvider("Preferences/Clay_EditorTools/Hierarchy Icons", SettingsScope.User){
            guiHandler = (searchContext) =>{
                EditorGUI.BeginChangeCheck();

               
                HierarchyIconsPreferences.SetPrefs();

                if (EditorGUI.EndChangeCheck()){
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        };
    }

    [SettingsProvider]
    public static SettingsProvider CreateToolsProvider(){
        return new SettingsProvider("Preferences/Clay_EditorTools/Tools", SettingsScope.User){
            guiHandler = (searchContext) =>{
                EditorGUI.BeginChangeCheck();

               
                ToolPrefs.SetPrefs();
            }
        };
    }
}

public class ColorPref{
    public string label;
    public string defaultHex;
    public string key;

    private Color? cachedColor; // cache the color to minimise editor prefs calls, increasing performance

    public ColorPref(string label, string defaultHex, string key){
        this.label = label;
        this.defaultHex = defaultHex;
        this.key = key;
        cachedColor = null;
    }

    public Color Load(){
        if (cachedColor.HasValue)
            return cachedColor.Value;

        string hex = EditorPrefs.GetString(key, defaultHex);
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        cachedColor = c;
        return c;
    }

    public void Save(Color c){
        cachedColor = c;
        EditorPrefs.SetString(key, ColorUtility.ToHtmlStringRGBA(c));
    }

    public void Reset(){
        cachedColor = null;
        EditorPrefs.SetString(key, defaultHex);
    }

    public Color Draw(){
        return EditorGUILayout.ColorField(label, Load());
    }
}
