using UnityEditor;
using UnityEngine;

//[InitializeOnLoad]
public class CustomShapeDrawer
{ 
    static CustomShapeDrawer()
    {
        //SceneView.duringSceneGui += OnSceneGUI;
    }
     
    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.red; 
         
        Vector3 position = Vector3.zero; 
        float radius = 5f;  
        Handles.DrawWireDisc(position, Vector3.up, radius);

 
        Handles.color = Color.green;
        Handles.DrawLine(Vector3.zero, new Vector3(10, 10, 0));

 
        Handles.color = Color.blue;
        Vector3 handlePosition = Handles.PositionHandle(new Vector3(5, 5, 0), Quaternion.identity);
        Handles.Label(handlePosition, "Move me!");

 
        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }
    }
}