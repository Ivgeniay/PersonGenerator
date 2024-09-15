using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Meshes.MeshGenerators;
using UnityEditor;

namespace Meshes
{
    [CustomEditor(typeof(MonoProceduralMesh))]
    public class MonoProceduralMeshInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            //SimpleProceduralMesh _target = (SimpleProceduralMesh)target;
            //SerializedObject serializedObject = base.serializedObject;

            //SerializedProperty configuratorProperty = serializedObject.FindProperty(nameof(_target.MeshName));

            //TextField meshNameTF = new TextField("Mesh Name");
            //meshNameTF.RegisterValueChangedCallback(e =>
            //{
            //    if (string.IsNullOrEmpty(e.newValue) || string.IsNullOrWhiteSpace(e.newValue)) return;
            //    _target.MeshName = e.newValue;
            //});
            //meshNameTF.BindProperty(configuratorProperty);

            //Button generateButton = new Button(() =>
            //{
            //    _target.Generate(_target.MeshName, );
            //})
            //{
            //    text = "Generate"
            //};

            //InspectorElement.FillDefaultInspector(root, serializedObject, this);

            //root.Add(meshNameTF);
            //root.Add(generateButton);
            return root;
        }
    }
}
