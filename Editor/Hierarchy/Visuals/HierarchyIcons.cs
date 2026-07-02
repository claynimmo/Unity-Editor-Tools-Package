
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyIcons
{
    static Texture2D audioIcon;
    static Texture2D lightIcon;
    static Texture2D emptyObjectIcon;
    static Texture2D invisibleObjectIcon;
    static Texture2D colliderIcon;
    static Texture2D customScriptIcon;

    private static int _horizontalOffset = 16;

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

        if(HierarchyIconsPreferences.ShowAudio && CheckAudio(obj, rect)) return;

        if(HierarchyIconsPreferences.ShowLight && CheckLight(obj, rect)) return;

        if(HierarchyIconsPreferences.ShowScript && CheckScript(obj,rect))return;

        if(HierarchyIconsPreferences.ShowCollider && CheckCollider(obj, rect)) return;

        if(HierarchyIconsPreferences.ShowInvisible && CheckInvisible(obj,rect)) return;
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
        if(rend==null || !rend.enabled){
            PaintIcon(invisibleObjectIcon, obj, rect, new Vector2(4,1));
            return true;
        }
        return false;
    }

    static bool CheckAudio(GameObject obj, Rect rect){
        AudioSource source = obj.GetComponent<AudioSource>();
        if(source != null){
            PaintIcon(audioIcon, obj, rect, new Vector2(4,0));
            return true;
        }
        return false;
    }

    static bool CheckLight(GameObject obj, Rect rect){
        Light l = obj.GetComponent<Light>();
        if(l != null){
            PaintIcon(lightIcon, obj, rect, new Vector2(3f,0));
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

    static bool CheckScript(GameObject obj, Rect rect){
        MonoBehaviour h = obj.GetComponent<MonoBehaviour>();
        if(h != null){
            PaintIcon(customScriptIcon, obj, rect, new Vector2(4,1), new Vector2(12,12));
            return true;
        }
        return false;
    }

    // function to print the icon to the hiearchy
    static void PaintIcon(Texture2D icon, GameObject obj, Rect rect, Vector2 offset, Vector2? scale = null){

        if(scale == null) scale = new Vector2(16,16);

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

        Rect iconRect = new Rect(x + offset.x, rect.y+offset.y, scale.Value.x, scale.Value.y);
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
