
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[InitializeOnLoad]
public static class HierarchyIcons
{
    static Texture2D audioIcon;
    static Texture2D lightIcon;
    static Texture2D emptyObjectIcon;
    static Texture2D invisibleObjectIcon;
    static Texture2D colliderIcon;
    static Texture2D customScriptIcon;
    static Texture2D cameraIcon;

    private static readonly int _horizontalOffset = 16;

    // a set of supported components with unique properties, indexed by rarity. Higher rarity is shown over a component with lower rarity
    // only includes components that are useful to see. (you would never need to visually see the audio listener, for example)
    private static readonly Dictionary<Type,int> _componentRarities = new(){

        // 100 series: common, but useful to see components
        {typeof(Light),1},
        {typeof(AudioSource),2},
        {typeof(Camera),3},
        {typeof(Animator),4},

        // 200 series: uncommon objects
        {typeof(Terrain),200},

        // 400 series: effects
        {typeof(ParticleSystem),400},
        {typeof(TrailRenderer),401},
        {typeof(LineRenderer),402},
        {typeof(ParticleSystemForceField),403},
        {typeof(LensFlare),404},

        // 500 series: UI
        {typeof(Canvas),550},
        {typeof(CanvasGroup),551},
        {typeof(Toggle),501},
        {typeof(UnityEngine.UI.Button),502},
        {typeof(Slider),503},
        {typeof(ScrollRect),504},
        {typeof(Scrollbar),505},


        // 1000 series: one per scene
        {typeof(ReflectionProbe),1000},
    };

    static HierarchyIcons(){
        ColorUtility.TryParseHtmlString("#ffb700c2", out Color audioCol);
        ColorUtility.TryParseHtmlString("#ffb700c2", out Color lightCol);
        ColorUtility.TryParseHtmlString("#94fafaa2", out Color invisibleCol);
        ColorUtility.TryParseHtmlString("#b3005f", out Color emptyCol);
        ColorUtility.TryParseHtmlString("#00b339", out Color colliderCol);
        ColorUtility.TryParseHtmlString("#ffffff", out Color customScriptCol);

        Texture2D defaultIcon = MakeReadable(EditorGUIUtility.IconContent("blendKey").image as Texture2D);
        
        audioIcon = MakeReadable(EditorGUIUtility.IconContent("AudioSource Gizmo").image as Texture2D);
        audioIcon = TintTexture(audioIcon, audioCol);

        lightIcon = MakeReadable(EditorGUIUtility.IconContent("PointLight Gizmo").image as Texture2D);
        lightIcon = TintTexture(lightIcon, lightCol);

        cameraIcon = MakeReadable(EditorGUIUtility.IconContent("d_Camera Icon").image as Texture2D);

        invisibleObjectIcon = TintTexture(defaultIcon, invisibleCol);
        emptyObjectIcon = TintTexture(defaultIcon, emptyCol);

        colliderIcon = MakeReadable(EditorGUIUtility.IconContent("d_BoxCollider Icon").image as Texture2D);
        colliderIcon = TintTexture(colliderIcon, colliderCol);

        customScriptIcon = MakeReadable(EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
        customScriptIcon = TintTexture(customScriptIcon, customScriptCol);
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect rect){
        if(!HierarchyIconsPreferences.EnableIcons) return;
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (!obj) return;


        if(HierarchyIconsPreferences.ShowEmpty && CheckEmpty(obj,rect))  return;

        if(HierarchyIconsPreferences.ShowSpecial && CheckComponent(obj,rect)) return;

        if(HierarchyIconsPreferences.ShowScript && CheckScript(obj,rect))return;

        if(HierarchyIconsPreferences.ShowInvisible && CheckInvisible(obj,rect)) return;

        if(HierarchyIconsPreferences.ShowCollider && CheckCollider(obj, rect)) return;

    }

    // a function per check, so reorganising the priority can be made easily in the ongui function
    static bool CheckEmpty(GameObject obj, Rect rect){
        if(obj.GetComponents<Component>().Length == 1){
            PaintIcon(emptyObjectIcon,obj,rect, new Vector2(4,1));
            return true;
        }
        return false;
    }

    static bool CheckInvisible(GameObject obj, Rect rect){
        Renderer rend = obj.GetComponent<Renderer>();
        Collider col = obj.GetComponent<Collider>();
        if((rend==null || !rend.enabled) && (col != null)){
            PaintIcon(invisibleObjectIcon, obj, rect, new Vector2(4,1));
            return true;
        }
        return false;
    }

    static bool CheckCollider(GameObject obj, Rect rect){
        Collider col = obj.GetComponent<Collider>();
        if(col != null && col.enabled){
            PaintIcon(colliderIcon, obj, rect, new Vector2(4,1), new Vector2(12,12));
            return true;
        }
        return false;
    }

    static bool CheckComponent(GameObject obj, Rect rect, Vector2? offset = null){
        if(offset == null) offset = new Vector2(3, 0);

        var comps = obj.GetComponents<Component>(); // get all components;

        int bestRank = 0;
        Component rarest = null;

        foreach (var c in comps){

            var type = c.GetType();
            
            if(!_componentRarities.ContainsKey(type))
                continue;
            
            if(_componentRarities.TryGetValue(type, out int rank)){
                if(rank > bestRank){
                    bestRank = rank;
                    rarest = c;
                }
            }
            
        }
        if(rarest == null) return false;

        Texture2D icon = GetComponentIcon(rarest);
        if(icon == null) return false;

        PaintIcon(icon,obj,rect,offset);

        return true;
    }

    static bool CheckScript(GameObject obj, Rect rect){
        MonoBehaviour[] h = obj.GetComponents<MonoBehaviour>();
        foreach(MonoBehaviour c in h){
            if(IsCustomScript(c)){
                PaintIcon(customScriptIcon, obj, rect, new Vector2(4,1), new Vector2(12,12));
                return true;
            }
        }
        return false;
    }

    static bool IsCustomScript(Component c){
        string asm = c.GetType().Assembly.GetName().Name;

        if (asm.StartsWith("UnityEngine")) return false;
        if (asm.StartsWith("UnityEditor")) return false;

        return true;
    }

    static Texture2D GetComponentIcon(Component c){
        GUIContent content = EditorGUIUtility.ObjectContent(null, c.GetType());
        return content.image as Texture2D;
    }


    // function to print the icon to the hiearchy
    static void PaintIcon(Texture2D icon, GameObject obj, Rect rect, Vector2? offset, Vector2? scale = null){
        
        if (Event.current.type != EventType.Repaint)
            return;
        if(scale == null) scale   = new Vector2(16,16);
        if(offset == null) offset = new Vector2(4,1);

        GUIStyle style = GUI.skin.label;

        Vector2 textSize = style.CalcSize(new GUIContent(obj.name));

        float nameStartX = rect.x + _horizontalOffset;

        float x = nameStartX + textSize.x; // default to after text

        if (HierarchyIconsPreferences.RightAlign)
            x = rect.x + rect.width - _horizontalOffset;

        if (!obj.activeInHierarchy)
            GUI.color = new Color(1f, 1f, 1f, 0.35f);   // faded
        else if (!obj.activeSelf)
            GUI.color = new Color(0.6f, 0.6f, 0.6f, 1f); // darkened

        Rect iconRect = new Rect(x + offset.Value.x, rect.y+offset.Value.y, scale.Value.x, scale.Value.y);

        if(icon == null) return;
        GUI.DrawTexture(iconRect, icon);

        GUI.color = Color.white;
    }
    
    // helper to tint a texture with a new color. Caching the results is required for performance
    static Texture2D TintTexture(Texture2D source, Color tint){
        Texture2D tex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        var pixels = source.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] *= tint;

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    // unity textures can sometimes not be read, so this function duplicates the texture into a new readable one
    static Texture2D MakeReadable(Texture2D source){
        RenderTexture rt = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(source.width, source.height);
        readable.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readable.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }
}
