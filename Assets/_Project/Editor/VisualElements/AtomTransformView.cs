using UnityEngine.UIElements;
using UnityEngine;
namespace AtomEngine.VisualElements
{
    public class AtomTransformView : AtomView
    {
        public override string Name { get; protected set; } = "ATOM_TRANSFORM";
        private AtomEngineTransform atomTransform;

        private Vector3Field position;
        private Vector3Field rotation;
        private Vector3Field scale;

        private EventCallback<ChangeEvent<Vector3>> positionCallback = null;
        private EventCallback<ChangeEvent<Vector3>> rotationCallback = null;
        private EventCallback<ChangeEvent<Vector3>> scaleCallback = null;

        public AtomTransformView(AtomEngineTransform atomTransform) : base()
        {
            this.atomTransform = atomTransform;
            contentContainer.Add(GetView());
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

        private VisualElement GetView()
        {
            VisualElement atomTransformView = new VisualElement();

            position = new Vector3Field("Position");
            rotation = new Vector3Field("Rotation");
            scale = new Vector3Field("Scale");

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

        internal void BindStandartCallbacks(EventCallback<ChangeEvent<Vector3>> _positionCallback = null, EventCallback<ChangeEvent<Vector3>> _rotationCallback = null, EventCallback<ChangeEvent<Vector3>> _scaleCallback = null)
        {
            positionCallback = e => { atomTransform.Position = e.newValue; };
            if (_positionCallback != null) positionCallback += _positionCallback;

            rotationCallback = e => { atomTransform.Rotation = Quaternion.Euler(e.newValue); };
            if (_rotationCallback != null) rotationCallback += _rotationCallback;

            scaleCallback = e => { atomTransform.Scale = e.newValue; };
            if (_scaleCallback != null) scaleCallback += _scaleCallback;

            position.RegisterValueChangedCallback(positionCallback);
            rotation.RegisterValueChangedCallback(rotationCallback);
            scale.RegisterValueChangedCallback(scaleCallback);
        }

        private void UnbindCallbacks()
        {
            if (positionCallback != null)
            {
                position.UnregisterValueChangedCallback(positionCallback);
                positionCallback = null;
            }
            if (rotationCallback != null)
            {
                rotation.UnregisterValueChangedCallback(rotationCallback);
                rotationCallback = null;
            }
            if (scaleCallback != null)
            {
                scale.UnregisterValueChangedCallback(scaleCallback);
                scaleCallback = null;
            }
        }

        public override void Update()
        {
            BindValue();
        }

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
