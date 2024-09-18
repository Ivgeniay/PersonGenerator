using UnityEngine.UIElements;
using UnityEditor.UIElements;
using AtomEngine.Skillets;
using AtomEngine.Testing;
using UnityEditor;

namespace AtomEngine.Skilet
{
    [CustomEditor(typeof(MeshBuilder))]
    internal class MeshBuilderInspector : TestedEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            //InspectorElement.FillDefaultInspector(root, serializedObject, this);

            var _target = (MeshBuilder)target;
            SerializedProperty materialProp = serializedObject.FindProperty("material");
            PropertyField materialField = new PropertyField(materialProp, "Material");
            root.Add(materialField);

            VisualElement testSection = base.CreateInspectorGUI().Q<VisualElement>(TEST_INSPECTOR_GUI);
            root.Add(testSection);

            return root;
        }

    }
}
