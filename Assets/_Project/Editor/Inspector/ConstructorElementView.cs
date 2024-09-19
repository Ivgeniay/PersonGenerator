using AtomEngine.Meshes.Constructor;
using UnityEngine.UIElements;
using UnityEditor;
namespace AtomEngine.VisualElements
{
    public class ConstructorElementView : VisualElement
    {
        protected ConstructedElement constructedElement;
        protected SerializedObject serializedObject;
        public ConstructorElementView(ConstructedElement constructedElement, SerializedObject serializedObject)
        {
            this.constructedElement = constructedElement;
            this.serializedObject = serializedObject;
        }
    }
}
