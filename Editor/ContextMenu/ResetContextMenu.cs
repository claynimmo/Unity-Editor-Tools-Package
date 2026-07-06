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
        object defaultValue = ContextHelpers.GetValueFromPath(newComponent, property.propertyPath);

        ContextHelpers.SetValueFromPath(target, property.propertyPath, defaultValue);

        EditorUtility.SetDirty(target);
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }


}