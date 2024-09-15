using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Meshes.Chains;
using UnityEditor;

namespace Meshes
{
    [CustomEditor(typeof(MeshChainConnector))]
    internal class MeshChainConnectorInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            MeshChainConnector meshChainConnector = (MeshChainConnector)target;
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            Button testBtn = new Button(() => meshChainConnector.Connect())
            {
                text = "Test"
            };

            root.Add(testBtn);

            return root;
        }
    }
}
