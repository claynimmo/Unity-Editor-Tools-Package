using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

/// <summary>
/// base class inheriting from the editor window for windows that are coupled to tool bar tools
/// </summary>
public class EditorToolWindow : EditorWindow {
    
    private bool deactived;

    /// <summary>
    /// Close the menu, called from the tool script when exiting the tool. This is to remove the window when it is no longer needed
    /// </summary>
    public void CloseMenu(){
        deactived = true;
        Close();
    }

    void OnDisable(){
        if(deactived) return;
        // Check if EditorTool is currently active and disable it when window is closed
        if(ToolManager.activeToolType == typeof(MeasureTool)){
            // Try to activate previously used tool
            ToolManager.RestorePreviousPersistentTool();
        }
    }

    protected static void AdjustWindow(EditorWindow window){
        Vector2 size = new Vector2(300,200);
        window.minSize = size;
        window.Show();
    }
}
