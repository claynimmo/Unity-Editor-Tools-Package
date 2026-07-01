using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

///
/// <summary> popup window to adjust the settings of the prefab brush</summary>
/// 
public class PrefabBrushWindow : EditorToolWindow
{

    public GameObject Prefab;
    public float BrushRadius = 2f;
    public int Amount = 5;
    public Transform Folder;

    public float MinScale = 0.7f;
    public float MaxScale = 1.3f;

    public bool  RandomScale;
    public bool  RandomRotation;


    [MenuItem("Tools/Prefab Painter")]
    public static void Open(){
        PrefabBrushWindow window = GetWindow<PrefabBrushWindow>("Prefab Painter");
        AdjustWindow(window);
    }


    void OnGUI(){

        Folder = (Transform)EditorGUILayout.ObjectField("Parent Folder", Folder, typeof(Transform), true);
        Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Prefab, typeof(GameObject), false);

        BrushRadius = EditorGUILayout.FloatField("Brush Radius", BrushRadius);
        Amount = EditorGUILayout.IntField("Spawn Amount", Amount);

        RandomRotation = EditorGUILayout.Toggle("Fully Random Rotation", RandomRotation);
        RandomScale = EditorGUILayout.Toggle("Randomize Scale", RandomScale);

        if(RandomScale){
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Scale Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.MinMaxSlider("Scale", ref MinScale, ref MaxScale, 0f, 3f);
            MinScale = EditorGUILayout.FloatField(MinScale, GUILayout.Width(50));
            MaxScale = EditorGUILayout.FloatField(MaxScale, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
    }
}
