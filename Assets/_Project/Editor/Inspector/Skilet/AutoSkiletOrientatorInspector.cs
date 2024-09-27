using MvLib.Testing.Inspector;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using AtomEngine.Skillets; 
using UnityEditor;

namespace AtomEngine.Skilet
{
    [CustomEditor(typeof(AutoSkilletOrientator))]
    internal class AutoSkiletOrientatorInspector : TestedEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            AutoSkilletOrientator _target = (AutoSkilletOrientator)target;
            Button button = new Button(() =>
            {
                _target.Orientate();
            })
            {
                text = "Auto Orientate"
            }; 

            root.Add(button);
            return root;
        }
    }
}
