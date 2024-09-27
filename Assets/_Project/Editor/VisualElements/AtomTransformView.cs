using UnityEngine.UIElements;
using UnityEngine;
using AtomEngine.Components;
using MvLib;

namespace AtomEngine.VisualElements
{
    public class AtomTransformView : AtomView
    {
        public override string Name { get; protected set; } = "ATOM_TRANSFORM";
        private AtomEngineTransform atomTransform;

        private Vector3Field position;
        private Vector3Field rotation;
        private Vector3Field scale; 

        private Binder<Vector3> positionBinder;
        private Binder<Quaternion, Vector3> rotationBinder;
        private Binder<Vector3> scaleBinder;

        public AtomTransformView(AtomEngineTransform atomTransform) : base()
        {
            this.atomTransform = atomTransform;
            contentContainer.Add(GetView(atomTransform));
            BindValue();
            BindStandartCallbacks();
            AddStype();
        }

        private void AddStype()
        {
            contentContainer.style.marginBottom = 8;
            contentContainer.style.borderBottomLeftRadius = 6;
            contentContainer.style.borderBottomRightRadius = 6;
            contentContainer.style.borderTopLeftRadius = 6;
            contentContainer.style.borderTopLeftRadius = 6;
            contentContainer.style.borderBottomColor = new StyleColor(Color.blue);
        }

        private VisualElement GetView(AtomEngineTransform atomTransform)
        {
            VisualElement atomTransformView = new VisualElement();

            Label label = new Label();
            label.text = $"Transform ({atomTransform.Parent.Name})";
            if (atomTransform.Parent is Atom atom)
            {
                label.text += $" index {atom.GetComponent<AtomEngineAtomIndex>().Index}";
            }

            position = new Vector3Field("Position");
            rotation = new Vector3Field("Rotation");
            scale = new Vector3Field("Scale");

            atomTransformView.Add(label);
            atomTransformView.Add(position);
            atomTransformView.Add(rotation);
            atomTransformView.Add(scale);

            return atomTransformView;
        }

        private void BindValue()
        { 
            position.value = atomTransform.Position;
            rotation.value = atomTransform.Rotation.eulerAngles;
            scale.value = atomTransform.Scale;
        } 

        internal void BindStandartCallbacks()
        {
            positionBinder = new Binder<Vector3>();
            rotationBinder = new Binder<Quaternion, Vector3>((e) => e.eulerAngles);
            scaleBinder = new Binder<Vector3>();

            positionBinder.Bind(atomTransform.ReactivePosition, position);
            rotationBinder.Bind(atomTransform.ReactiveRotation, rotation);
            scaleBinder.Bind(atomTransform.ReactiveScale, scale); 
        }

        private void UnbindCallbacks()
        {
            positionBinder.Dispose();
            rotationBinder.Dispose();
            scaleBinder.Dispose(); 
        }

        public override void Update() { } 
        public void Hide()
        {
            contentContainer.StartFadeOutAnimation(1.5f, () =>
            {
                contentContainer.style.display = DisplayStyle.None;
            });
        }

        public void Show()
        {
            contentContainer.style.display = DisplayStyle.Flex;
            contentContainer.StartFadeInAnimation(1.5f);
        }

        public override void Dispose()
        {
            UnbindCallbacks();
        }
    }
}
