using UnityEditor;
using UnityEngine;

namespace AtomEngine.SceneViews
{
    public delegate void HandleChanged(Vector3 newPosition);
    [InitializeOnLoad]
    public static class HandleDrawer
    { 
        public static event HandleChanged OnHandleChanged;

        private static Vector3 handlePosition = new Vector3(5, 5, 0);
        private static bool showHandle = false;

        static HandleDrawer()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static void EnableHandle()
        {
            if (!showHandle)
            {
                SceneView.duringSceneGui += OnSceneGUI;
                showHandle = true;
                SceneView.RepaintAll();
            }
        } 
        public static void DisableHandle()
        {
            if (showHandle)
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                showHandle = false;
                SceneView.RepaintAll();
            }
        }

        public static void SetPosition(Vector3 position)
        {
            handlePosition = position;
            OnHandleChanged?.Invoke(handlePosition);
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!showHandle) return; 

            Handles.color = Color.blue;
            Vector3 newHandlePosition = Handles.PositionHandle(handlePosition, Quaternion.identity);
            if (newHandlePosition != handlePosition)
            {
                SetPosition(newHandlePosition);
                Debug.Log("New handle position: " + handlePosition);
            } 
            Handles.Label(handlePosition + Vector3.up * 0.5f, "Move me!");

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }
    }
}
