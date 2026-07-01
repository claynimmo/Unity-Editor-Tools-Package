using UnityEditor;
using UnityEngine;

///
/// <summary> A tool to paint prefabs where the mouse is touching, replicating the terrain brush tool </summary>
/// 
public class PrefabBrush : EditorWindow
{

    public GameObject Prefab;
    public float BrushRadius = 2f;
    public int Amount = 5;
    public Transform Folder;

    public float MinScale = 0.7f;
    public float MaxScale = 1.3f;

    bool _painting;
    bool _randomScale;
    bool _randomRotation;
    bool _hasHitPoint;
    Vector3 _lastHitPoint;
    [MenuItem("Tools/Prefab Painter")]
    public static void Open(){
        GetWindow<PrefabBrush>("Prefab Painter");
    }

    void OnEnable(){
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable(){
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI(){

        Folder = (Transform)EditorGUILayout.ObjectField("Parent Folder", Folder, typeof(Transform), true);
        Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Prefab, typeof(GameObject), false);

        BrushRadius = EditorGUILayout.FloatField("Brush Radius", BrushRadius);
        Amount = EditorGUILayout.IntField("Spawn Amount", Amount);

        _randomRotation = EditorGUILayout.Toggle("Fully Random Rotation", _randomRotation);
        _randomScale = EditorGUILayout.Toggle("Randomize Scale", _randomScale);

        if(_randomScale){
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
        EditorGUILayout.Space();

        _painting = EditorGUILayout.Toggle("Enable Painting", _painting);
    }

    void OnSceneGUI(SceneView sceneView){
        if(!_painting || Prefab == null)
            return;
        
        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit)){
            _lastHitPoint = hit.point;
            _hasHitPoint = true;

            if(e.type == EventType.MouseDown && e.button == 0 && !e.alt){

                PaintPrefabs(_lastHitPoint);
                e.Use(); // consume event to prevent selecting objects while painting
            }

            // draw the brush
            Color c;
            ColorUtility.TryParseHtmlString("#5ecbf230", out c);
            Handles.color = c;
            Handles.DrawSolidDisc(_lastHitPoint, Vector3.up, BrushRadius);
        }
        else{
            _hasHitPoint = false;
        }
    }

    void PaintPrefabs(Vector3 center){

        if (Folder == null){
            GameObject container = new GameObject("Painted Prefabs");
            Folder = container.transform;
            Undo.RegisterCreatedObjectUndo(container, "Create Container");
        }

        for(int i = 0; i < Amount; i++){
            Vector2 rand = Random.insideUnitCircle * BrushRadius;
            Vector3 pos = center + new Vector3(rand.x, 0, rand.y);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
            instance.name = Prefab.name + $"_painted_{i}";
            Undo.RegisterCreatedObjectUndo(instance, "Paint Prefab"); // let the spawned prefabs be undone

            instance.transform.position = pos;
            if(_randomRotation){
                instance.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0,360f), Random.Range(0,360f));
            }
            else
                instance.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            instance.transform.SetParent(Folder);

            if(_randomScale){
                float random = Random.Range(MinScale, MaxScale);
                instance.transform.localScale *= random;
            }

            // set painted objects to batching static, to lower draw calls
            GameObjectUtility.SetStaticEditorFlags(
                instance,
                StaticEditorFlags.BatchingStatic
            );
        }
    }
}
