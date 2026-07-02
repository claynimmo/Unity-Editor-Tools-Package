using UnityEditor;
using UnityEngine;

static class HierarchyIconsPreferences
{
    private const string EnableIconsKey = "Clay_EditorTools.HierarchyIcons_Enable";
    private const string ShowAudioKey = "Clay_EditorTools.HierarchyIcons_ShowAudio";
    private const string ShowLightKey = "Clay_EditorTools.HierarchyIcons_ShowLight";
    private const string ShowEmptyKey = "Clay_EditorTools.HierarchyIcons_ShowEmpty";
    private const string ShowInvisibleKey = "Clay_EditorTools.HierarchyIcons_ShowInvisible";
    private const string ShowColliderKey = "Clay_EditorTools.HierarchyIcons_ShowCollider";
    private const string ShowScriptKey = "Clay_EditorTools.HierarchyIcons_ShowScript";

    public static string RightAlignKey = "Clay_EditorTools.HierarchyIcons_RightAlign";

    public static bool EnableIcons => EditorPrefs.GetBool(EnableIconsKey, true);
    public static bool ShowAudio => EditorPrefs.GetBool(ShowAudioKey, true);
    public static bool ShowLight => EditorPrefs.GetBool(ShowLightKey, true);
    public static bool ShowEmpty => EditorPrefs.GetBool(ShowEmptyKey, true);
    public static bool ShowInvisible => EditorPrefs.GetBool(ShowInvisibleKey, true);
    public static bool ShowCollider => EditorPrefs.GetBool(ShowColliderKey, false);
    public static bool ShowScript => EditorPrefs.GetBool(ShowScriptKey, false);

    public static bool RightAlign => EditorPrefs.GetBool(RightAlignKey, false);

    [SettingsProvider]
    public static SettingsProvider CreateProvider(){
        return new SettingsProvider("Preferences/Clay_EditorTools/Hierarchy Icons", SettingsScope.User){
            guiHandler = (searchContext) =>{
                EditorGUI.BeginChangeCheck();

                bool enableIcons = EditorPrefs.GetBool(EnableIconsKey, true);
                bool showAudio = EditorPrefs.GetBool(ShowAudioKey, true);
                bool showLight = EditorPrefs.GetBool(ShowLightKey, true);
                bool showEmpty = EditorPrefs.GetBool(ShowEmptyKey, true);
                bool showInvisible = EditorPrefs.GetBool(ShowInvisibleKey, true);
                bool showCollider = EditorPrefs.GetBool(ShowColliderKey, false);
                bool showScript = EditorPrefs.GetBool(ShowScriptKey, false);

                bool rightAlign = EditorPrefs.GetBool(RightAlignKey, false);

                EditorGUILayout.LabelField("Displayed Icons", EditorStyles.boldLabel);


                enableIcons = EditorGUILayout.Toggle("Enable Icons", enableIcons);
                showAudio = EditorGUILayout.Toggle("Audio Icons", showAudio);
                showLight = EditorGUILayout.Toggle("Light Icons", showLight);
                showEmpty = EditorGUILayout.Toggle("Empty Object Icons", showEmpty);
                showInvisible = EditorGUILayout.Toggle("Invisible Object Icons", showInvisible);
                showCollider = EditorGUILayout.Toggle("Collider Icons", showCollider);
                showScript = EditorGUILayout.Toggle("Custom Script Icons", showScript);
                
                EditorGUILayout.LabelField("Alignment Settings", EditorStyles.boldLabel);

                rightAlign = EditorGUILayout.Toggle("Align to Right", rightAlign);

                if (EditorGUI.EndChangeCheck()){
                    EditorPrefs.SetBool(EnableIconsKey, enableIcons);
                    EditorPrefs.SetBool(ShowAudioKey, showAudio);
                    EditorPrefs.SetBool(ShowLightKey, showLight);
                    EditorPrefs.SetBool(ShowEmptyKey, showEmpty);
                    EditorPrefs.SetBool(ShowInvisibleKey, showInvisible);
                    EditorPrefs.SetBool(ShowColliderKey, showCollider);
                    EditorPrefs.SetBool(ShowScriptKey, showScript);

                    EditorPrefs.SetBool(RightAlignKey, rightAlign);
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        };
    }
}
