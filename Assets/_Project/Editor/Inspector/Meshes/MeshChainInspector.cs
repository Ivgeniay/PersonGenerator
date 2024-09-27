using AtomEngine.Meshes.Chains;
using MvLib.Testing.Inspector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;

namespace AtomEngine.Meshes
{
    [CustomEditor(typeof(MeshChain))]
    internal class MeshChainInspector : TestedEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            MeshChain meshChain = (MeshChain)target;

            SerializedProperty vertexesCountProperty = serializedObject.FindProperty(nameof(meshChain.VertexCount));
            PropertyField vertexesCountField = new PropertyField(vertexesCountProperty, nameof(meshChain.VertexCount));
            root.Add(vertexesCountField); 

            return root;
        }
    }
}
