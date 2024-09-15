using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Meshes.Chains;
using UnityEditor;

namespace Meshes
{
    [CustomEditor(typeof(MeshChain))]
    internal class MeshChainInspector : Editor
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
