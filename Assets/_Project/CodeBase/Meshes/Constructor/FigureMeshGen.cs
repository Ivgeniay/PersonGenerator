using AtomEngine.Testing.Inspector; 
using UnityEditor;
using UnityEngine;

namespace AtomEngine.Meshes.Constructor
{
    public class FigureMeshGen : MonoBehaviour, IPublicMethodsInspector
    {
        public void Generate()
        {
#if UNITY_EDITOR
            GameObject go = new GameObject("GeneratedMesh");
            go.AddComponent<ConstructedElement>();

            Selection.SetActiveObjectWithContext(go, null);
            if (Selection.activeTransform != null)
            {
                SceneView sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                {
                    Selection.activeTransform.position = sceneView.camera.transform.position;
                    Transform cameraTransform = sceneView.camera.transform; 
                    Vector3 newPosition = cameraTransform.position + cameraTransform.forward * 5f;
                    Selection.activeTransform.position = newPosition;

                    SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Constructed element was created!"));
                    //SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Enable 'Custom Tools' overlay from the overlay menu!"));
                }
                else
                {
                    Debug.LogError("Сцена не активна. Откройте Scene View.");
                }
            }
            else
            {
                Debug.LogError("Не выбран объект для перемещения.");
            }
#endif
        }
    }
}
