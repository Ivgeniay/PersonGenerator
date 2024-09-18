using UnityEditor.UIElements;
using UnityEngine.UIElements; 
using UnityEditor;
using AtomEngine.Meshes.MeshGenerators;
using AtomEngine.Testing;

namespace AtomEngine.Meshes
{
    [CustomEditor(typeof(MonoProceduralMesh))]
    internal class MonoProceduralMeshInspector : TestedEditor
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
