using MvLib.Testing.Inspector;
using UnityEditor.UIElements;
using UnityEngine.UIElements; 
using UnityEditor;
using UnityEngine;

namespace AtomEngine.Meshes.Vertexes
{
    [CustomEditor(typeof(VertexMono))]
    internal class VertexMonoInspector : TestedEditor
    {
        void UpdateLabel(Label label, SerializedProperty indexProperty)
        {
            label.text = $"Index: {indexProperty.intValue}";
        }

        public override VisualElement CreateInspectorGUI()
        {

            VisualElement root = new VisualElement();
            VertexMono vertexMono = (VertexMono)target;

            SerializedProperty unityEventProperty = serializedObject.FindProperty(nameof(vertexMono.OnVertexChangedEvent));
            PropertyField eventField = new PropertyField(unityEventProperty, nameof(vertexMono.OnVertexChangedEvent));
             
            SerializedProperty vertexProperty = serializedObject.FindProperty("vertex");
            SerializedProperty indexProperty = vertexProperty.FindPropertyRelative(nameof(vertexMono.Value.Index));
            SerializedProperty positionProperty = vertexProperty.FindPropertyRelative(nameof(vertexMono.Value.Position));

            VisualElement wrapDiv = new VisualElement();
            wrapDiv.style.backgroundColor = Color.grey;
            wrapDiv.style.paddingTop = 3;
            wrapDiv.style.paddingBottom = 3;
            wrapDiv.style.paddingLeft = 3;
            wrapDiv.style.paddingRight = 3;
            wrapDiv.style.borderTopLeftRadius = 5;
            wrapDiv.style.borderTopRightRadius = 5;
            wrapDiv.style.borderBottomLeftRadius = 5;
            wrapDiv.style.borderBottomRightRadius = 5;

            Label indexLabel = new Label() { text = $"Index: {indexProperty.intValue}" };
            Label positionLabel = new Label() { text = $"Position: {positionProperty.vector3Value}" };

            wrapDiv.Add(indexLabel);
            wrapDiv.Add(positionLabel);
            root.Add(wrapDiv);
            root.Add(eventField);

            return root;
        }
    }
}
