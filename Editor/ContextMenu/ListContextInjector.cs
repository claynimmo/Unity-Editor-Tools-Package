using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
// adds context menu pe
[InitializeOnLoad]
public static class ListContextInjector {

    static ListContextInjector(){
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property){
        if (property.isArray && property.propertyType == SerializedPropertyType.Generic){
            
            List<GameObject> selectedObjects = SelectionCache.SavedSelection;


            if(selectedObjects.Count > 0){
                Type t = GetListElementType(property);

                if(t == typeof(Vector3)){
                    AddVector3Options(menu, selectedObjects, property);
                    return;
                }
                menu.AddItem(new GUIContent("Fill From Saved Selection"), false, () =>{
                    ListSelectionFiller.Fill(property, selectedObjects);
                });
            }
        }
    }

    public static Type GetListElementType(SerializedProperty property){
        var target = property.serializedObject.targetObject;
        var type = target.GetType();

        string fieldName = property.propertyPath.Split('.')[0];

        var field = type.GetField(fieldName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (field == null)
            return null;

        var fieldType = field.FieldType;

        if (fieldType.IsArray)
            return fieldType.GetElementType();

        if (fieldType.IsGenericType)
            return fieldType.GetGenericArguments()[0];

        return null;
    }

    private static void AddVector3Options(GenericMenu menu, List<GameObject> objs, SerializedProperty property) {
        menu.AddItem(new GUIContent("Fill From Saved Selection (Global Position)"), false, () => {
            List<Vector3> values = objs
                .Select(go => go.transform.position)
                .ToList();
            ListSelectionFiller.FillFromList(property, values);
        });

        menu.AddItem(new GUIContent("Fill From Saved Selection (Local Position)"), false, () => {
            List<Vector3> values = objs
                .Select(go => go.transform.localPosition)
                .ToList();
            ListSelectionFiller.FillFromList(property, values);
        });

        menu.AddItem(new GUIContent("Fill From Saved Selection (Euler Angles)"), false, () => {
            List<Vector3> values = objs
                .Select(go => go.transform.eulerAngles)
                .ToList();

            ListSelectionFiller.FillFromList(property, values);
        });

        menu.AddItem(new GUIContent("Fill From Saved Selection (Local Scale)"), false, () => {
            List<Vector3> values = objs
                .Select(go => go.transform.localScale)
                .ToList();

            ListSelectionFiller.FillFromList(property, values);
        });
    }

}