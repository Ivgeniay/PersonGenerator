using AtomEngine.Meshes.Chains;
using MvLib.Testing.Inspector;
using UnityEngine.UIElements; 
using UnityEditor.UIElements; 
using UnityEditor;

namespace AtomEngine.Meshes
{
    [CustomEditor(typeof(MeshChainConnector))]
    internal class MeshChainConnectorInspector : TestedEditor
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
