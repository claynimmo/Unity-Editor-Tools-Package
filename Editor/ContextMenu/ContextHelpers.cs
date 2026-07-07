using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A set of static helpers to get information from a property, or change its value
/// </summary>
public class ContextHelpers : MonoBehaviour
{
    /// <summary>
    /// Get the value of a field 
    /// </summary>
    /// <param name="obj">property.serializedObject.targetObject object</param>
    /// <param name="path">property.propertyPath path</param>
    /// <returns></returns>
    public static object GetValueFromPath(object obj, string path){
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

    /// <summary>
    /// Set the value of a property
    /// </summary>
    /// <param name="obj">property.serializedObject.targetObject object</param>
    /// <param name="path">property.propertyPath path</param>
    /// <param name="value">new object value</param>
    public static void SetValueFromPath(object obj, string path, object value){
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

    /// <summary>
    /// Change the value of a property with a new one, including the undo command
    /// </summary>
    /// <param name="property">selected property</param>
    /// <param name="newValue">new value to be set from the property</param>
    public static void SwapPropertyValue(SerializedProperty property, object newValue){
        var target = property.serializedObject.targetObject;

        Undo.RecordObject(target,"Swap Field");

        SetValueFromPath(target,property.propertyPath,newValue);

        property.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    /// <summary>
    /// Get the GameObject referenced in the field (for example, if it is a collider, get the collider.gameObject)
    /// </summary>
    /// <param name="property">property field to get the referenced object from</param>
    /// <returns>GameObject represented by Component.gameObject. Can be null</returns>
    public static GameObject GetReferencedGameObject(SerializedProperty property){
        object obj = property.objectReferenceValue;

        if(obj == null)
            return null;

        if(obj is GameObject g)
            return g;
        
        if(obj is Component c)
            return c.gameObject;

        return null;
    }

    /// <summary>
    /// Get the type of the field
    /// </summary>
    /// <param name="property">serialized property to test the type</param>
    /// <returns>Type of the serialized property</returns>
    public static Type GetPropertyFieldType(SerializedProperty property){
        Type objType = property.serializedObject.targetObject.GetType();
        string[] sections = property.propertyPath.Split('.');

        Type currentType = objType;
        foreach (string section in sections){
            if(section == "Array") continue;

            if(section.StartsWith("data[")){
                // Move into list element type
                if (currentType.IsArray)
                    currentType = currentType.GetElementType();
                else
                    currentType = currentType.GetGenericArguments()[0];

                continue;
            }

            FieldInfo field = currentType.GetField(section, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            currentType = field.FieldType;
        }
        return currentType;
    }

}
