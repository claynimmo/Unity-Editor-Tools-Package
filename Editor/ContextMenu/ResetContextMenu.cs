using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

/// <summary>
/// Context menu to reset the value of the selected property to that of the original component, bypassing the need to reset the entire component.
/// </summary>
[InitializeOnLoad]
public static class ResetContextMenu {

    static ResetContextMenu(){
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property){
        menu.AddItem(new GUIContent("Reset Property"),false, () => {
            ResetToDefault(property);
        });
    }

    private static void ResetToDefault(SerializedProperty property){
        var target = property.serializedObject.targetObject;
        Type type = target.GetType();

        var newComponent = Activator.CreateInstance(type);

        Undo.RecordObject(target, "Reset Field");
        object defaultValue = GetValueFromPath(newComponent, property.propertyPath);

        SetValueFromPath(target, property.propertyPath, defaultValue);

        EditorUtility.SetDirty(target);
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }
    
    // function to get the value of a desired field as an object reference via reflection
    private static object GetValueFromPath(object obj, string path){
        foreach(string section in path.Split('.')){
            if(section == "Array") continue;
            if (section.StartsWith("data[")){ //set the object to the correct reference in the path (obj = list -> list[a])
                // if for example "data[a]", index = a
                int index = int.Parse(section.Substring(5,section.Length - 6));

                // move into the element at index a
                object list = obj;
                Type listType = list.GetType();

                PropertyInfo indexer = listType.GetProperty("Item");
                obj = indexer.GetValue(list, new object[] {index});
            }
            else{
                // find the field specified by the section, and store its value
                FieldInfo field = obj.GetType().GetField(section, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                obj = field.GetValue(obj);
            }
        }
        return obj;
    }

    private static void SetValueFromPath(object obj, string path, object value){
        string[] sections = path.Split('.');
        for(int i = 0; i < sections.Length; i++){
            string section = sections[i];
            if(section == "Array") continue;
            if(section.StartsWith("data[")){
                int index = int.Parse(section.Substring(5,section.Length - 6));

                object list = obj;
                Type listType = list.GetType();

                PropertyInfo indexer = listType.GetProperty("Item");

                object[] indexArr = new object[] {index};

                if(i == section.Length - 1)
                    indexer.SetValue(list, value, indexArr);
                else
                    obj = indexer.GetValue(list, indexArr);
            }
            else{
                // set the value of the field
                FieldInfo field = obj.GetType().GetField(section, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (i == sections.Length - 1){
                    field.SetValue(obj, value);
                    return;
                }
                else
                    obj = field.GetValue(obj);
            }
        }
    }

}