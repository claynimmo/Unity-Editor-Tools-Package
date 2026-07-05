using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEditor;

/// <summary>
/// helper to fill a list property of a type from a list of gameobjects
/// </summary>
public static class ListSelectionFiller{

    public static void FillFromList<T>(SerializedProperty listProp, List<T> values){
        var serializedObject = listProp.serializedObject;
       
        listProp.ClearArray();

        for(int i = 0; i < values.Count; i++){
            listProp.InsertArrayElementAtIndex(i);
            var element = listProp.GetArrayElementAtIndex(i);
            AssignValue(element, values[i]);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static void AssignValue<T>(SerializedProperty element, T value){
        switch (element.propertyType){
            case SerializedPropertyType.Integer:
                element.intValue = Convert.ToInt32(value);
                break;

            case SerializedPropertyType.Float:
                element.floatValue = Convert.ToSingle(value);
                break;

            case SerializedPropertyType.Boolean:
                element.boolValue = Convert.ToBoolean(value);
                break;

            case SerializedPropertyType.String:
                element.stringValue = value?.ToString();
                break;

            case SerializedPropertyType.Vector2:
                element.vector2Value = (Vector2)(object)value;
                break;

            case SerializedPropertyType.Vector3:
                element.vector3Value = (Vector3)(object)value;
                break;

            case SerializedPropertyType.Color:
                element.colorValue = (Color)(object)value;
                break;

            case SerializedPropertyType.Quaternion:
                element.quaternionValue = (Quaternion)(object)value;
                break;

            case SerializedPropertyType.ObjectReference:
                element.objectReferenceValue = value as UnityEngine.Object;
                break;
        }
    }
    
    public static void Fill(SerializedProperty listProp,List<GameObject> selectedObjects) {
       var serializedObject = listProp.serializedObject;

        Type elementType = GetElementType(listProp);
        if (elementType == null)
        {
            Debug.LogError("Could not determine list element type.");
            return;
        }

        listProp.ClearArray();
        int index = 0;

        foreach (var go in selectedObjects)
        {
            // Special case: List<GameObject>
            if (elementType == typeof(GameObject))
            {
                listProp.InsertArrayElementAtIndex(index);
                listProp.GetArrayElementAtIndex(index).objectReferenceValue = go;
                index++;
                continue;
            }

            // Generic case: List<T> where T : Component
            Component c = go.GetComponent(elementType);
            if (c != null)
            {
                listProp.InsertArrayElementAtIndex(index);
                listProp.GetArrayElementAtIndex(index).objectReferenceValue = c;
                index++;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static Type GetElementType(SerializedProperty listProp){
        var target = listProp.serializedObject.targetObject;
        var field = target.GetType().GetField(listProp.propertyPath,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic);

        if (field == null)
            return null;

        var fieldType = field.FieldType;

        if (fieldType.IsArray)
            return fieldType.GetElementType();

        if (fieldType.IsGenericType)
            return fieldType.GetGenericArguments()[0];

        return null;
    }
}

