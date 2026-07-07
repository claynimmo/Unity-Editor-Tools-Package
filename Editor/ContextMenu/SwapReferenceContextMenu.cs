using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[InitializeOnLoad]
public class SwapReferenceContextMenu : MonoBehaviour
{
    static SwapReferenceContextMenu(){
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property){
        if(property.isArray) return;

        Type fieldType = ContextHelpers.GetPropertyFieldType(property);
        GameObject referencedObj = ContextHelpers.GetReferencedGameObject(property);

        if(referencedObj == null) return;
        if(fieldType == typeof(GameObject)) return;

        Component[] comps = referencedObj.GetComponents(fieldType);

        if(comps.Length <= 1) return; // if the length is one, then that is just the already referenced component, so showing the menu is redundant

        for(int i = 0; i < comps.Length; i++){
            Component c = comps[i];
            Type t = c.GetType();
            string name = t.Name;

            menu.AddItem(new GUIContent($"Swap Reference/{name} ({i})"),false, () =>{
                ContextHelpers.SwapPropertyValue(property, c);
            });
        }
    }
    
    
}
