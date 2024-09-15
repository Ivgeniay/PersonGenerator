using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using Skillets;

namespace Skilet
{
    [CustomEditor(typeof(AutoSkilletOrientator))]
    internal class AutoSkiletOrientatorInspector : Editor
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
