using UnityEditor;
using UnityEngine;
using Skillets;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Skilet
{
    [CustomEditor(typeof(MeshBuilder))]
    internal class MeshBuilderInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            var _target = (MeshBuilder)target;

            Button generateButton = new Button(() => _target.GenerateMesh()) { text = "Total Generate" };
            Button generateBindPoseButton = new Button(() => _target.GenerateSkinRenerer(_target.SkilletBuilder.transform)) { text = "GeenerateSkinRenerer" };

            root.Add(generateButton);
            root.Add(generateBindPoseButton);

            return root;
        }
    }
}
