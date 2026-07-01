using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary> A tool to paint prefabs where the mouse is touching, replicating the terrain brush tool </summary>
[EditorTool("Prefab Brush Tool")]
public class PrefabBrushTool : EditorTool
{
    private PrefabBrushWindow window;

    private GameObject _prefab;
    private float _brushRadius;
    private int _amount;
    private Transform _folder;
    private bool _randomScale;
    private float _minScale;
    private float _maxScale;
    private bool _randomRotation;
    Vector3 _lastHitPoint;

    public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("TerrainInspector.TerrainToolTrees");

    public override void OnActivated(){
        window = EditorWindow.GetWindow<PrefabBrushWindow>();
    }

    public override void OnWillBeDeactivated()
    {
        window.CloseMenu();
    }


    public override void OnToolGUI(EditorWindow sceneWindow){
    
        if (sceneWindow is not SceneView sceneView)
            return;

        _prefab = window.Prefab;
        _brushRadius = window.BrushRadius;
        _amount = window.Amount;
        if(window.Folder != null)
            _folder = window.Folder;
        _randomScale = window.RandomScale;
        _randomRotation = window.RandomRotation;
        _minScale = window.MinScale;
        _maxScale = window.MaxScale;

        if(_prefab == null)
            return;

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit)){
            _lastHitPoint = hit.point;

            if(e.type == EventType.MouseDown && e.button == 0 && !e.alt){

                PaintPrefabs(_lastHitPoint);
                e.Use(); // consume event to prevent selecting objects while painting
            }

            // draw the brush
            Color c;
            ColorUtility.TryParseHtmlString("#5ecbf230", out c);
            Handles.color = c;
            Handles.DrawSolidDisc(_lastHitPoint, Vector3.up, _brushRadius);
        }
    }

    void PaintPrefabs(Vector3 center){

        if (_folder == null){
            GameObject container = new GameObject("Painted Prefabs");
            _folder = container.transform;
            Undo.RegisterCreatedObjectUndo(container, "Create Container");
        }

        for(int i = 0; i < _amount; i++){
            Vector2 rand = Random.insideUnitCircle * _brushRadius;
            Vector3 pos = center + new Vector3(rand.x, 0, rand.y);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);
            instance.name = _prefab.name + $"_painted_{i}";
            Undo.RegisterCreatedObjectUndo(instance, "Paint Prefab"); // let the spawned prefabs be undone

            instance.transform.position = pos;
            if(_randomRotation){
                instance.transform.rotation = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0,360f), Random.Range(0,360f));
            }
            else
                instance.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            instance.transform.SetParent(_folder);

            if(_randomScale){
                float random = Random.Range(_minScale, _maxScale);
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
