using AtomEngine.SystemFunc.Extensions;
using AtomEngine.Meshes.Constructor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using AtomEngine.Components;
using AtomEngine.SceneViews;
using AtomEngine.Testing; 
using UnityEditor;
using UnityEngine;
using AtomEngine;
using System;

namespace Inspector
{
    [CustomEditor(typeof(ConstructedElement))]
    internal class ConstructorElementInspector : TestedEditor
    {
        public event Action<AtomObject> OnAtomSelected;

        public static ConstructorElementInspector Instance;
        private OverlayToolsType _toolsType = OverlayToolsType.Translate;


        #region Inspector
        public override VisualElement CreateInspectorGUI()
        {
            Instance = this;
            ConstructedElement constructedElement = (ConstructedElement)target;

            Selection.selectionChanged += OnInspectorClose;
            void OnInspectorClose()
            {
                if (Selection.activeObject == null ||
                Selection.activeObject != constructedElement)
                {
                    constructedElement.PreviousSelectedAtomObject = null;
                    constructedElement.SelectedAtomObject = null;
                    constructedElement.SelectedAtomObjects.Clear();
                    Selection.selectionChanged -= OnInspectorClose;
                }
            }

            VisualElement root = new VisualElement();

            SetUp_FigureCreation_Section(constructedElement, root); 
            SetUp_AtomTransform(constructedElement, root);
            SetUp_AtomsList_As_Property(root);
            SetUp_EdgeList_As_Property(root);
            SetUp_EditorSettings(constructedElement, root);

            root.Add(base.CreateInspectorGUI());

            return root;
        }

        private void SetUp_FigureCreation_Section(ConstructedElement constructedElement, VisualElement root)
        {
            VisualElement createFigure = new VisualElement();
            Button createCubeBtn = new Button(() => constructedElement.GenerateCube()) { text = "Generate Cube" };
            Button createSphereBtn = new Button(() => constructedElement.GenerateSphere()) { text = "Generate Sphere" };
            Button createCapsuleBtn = new Button(() => constructedElement.GenerateCapsule()) { text = "Generate Capsule" };
            Button createPlaneBtn = new Button(() => constructedElement.GeneratePlane()) { text = "Generate Plane Cube" };

            createFigure.Add(createCubeBtn);
            createFigure.Add(createSphereBtn);
            createFigure.Add(createCapsuleBtn);
            createFigure.Add(createPlaneBtn);
            root.Add(createFigure);
        }

        private void SetUp_AtomTransform(ConstructedElement constructedElement, VisualElement root)
        {
            VisualElement atomTransformView = new VisualElement();
            atomTransformView.style.opacity = 0;
            atomTransformView.style.display = constructedElement.SelectedAtomObject is null ? DisplayStyle.None : DisplayStyle.Flex;
            atomTransformView.StartFadeOutAnimation(0.5f, () =>
            {
                atomTransformView.StartFoldAnimation(50, 0.5f);
            });


            Vector3Field position = new Vector3Field("Position");
            Vector3Field rotation = new Vector3Field("Rotation");
            Vector3Field scale = new Vector3Field("Scale");

            atomTransformView.Add(position);
            atomTransformView.Add(rotation);
            atomTransformView.Add(scale);


            EventCallback<ChangeEvent<Vector3>> positionCallback = null;
            EventCallback<ChangeEvent<Vector3>> rotationCallback = null;
            EventCallback<ChangeEvent<Vector3>> scaleCallback = null;

            OnAtomSelected += (atomObject) =>
            {
                if (atomObject is not Atom atom) return;

                if (constructedElement.PreviousSelectedAtomObject != null)
                {
                    AtomEngineTransform prevAtomTrams = constructedElement.PreviousSelectedAtomObject.GetComponent<AtomEngineTransform>();
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

                if (atomObject != null)
                {
                    atomTransformView.style.display = DisplayStyle.Flex;
                    constructedElement.PreviousSelectedAtomObject = atomObject;

                    AtomEngineTransform atomTransform = atomObject.GetComponent<AtomEngineTransform>();
                    AtomEngineAtomIndex atomIndex = atomObject.GetComponent<AtomEngineAtomIndex>();

                    position.value = atomTransform.Position;
                    rotation.value = atomTransform.Rotation.eulerAngles;
                    scale.value = atomTransform.Scale;

                    positionCallback = e => atomTransform.Position = e.newValue;
                    rotationCallback = e => atomTransform.Rotation = Quaternion.Euler(e.newValue);
                    scaleCallback = e => atomTransform.Scale = e.newValue;

                    positionCallback = e =>
                    {
                        atomTransform.Position = e.newValue;
                        AtomMoved(constructedElement, atom);
                        SceneView.RepaintAll();
                    };
                    rotationCallback = e =>
                    {
                        atomTransform.Rotation = Quaternion.Euler(e.newValue);
                        SceneView.RepaintAll();
                    };
                    scaleCallback = e =>
                    {
                        atomTransform.Scale = e.newValue;
                        SceneView.RepaintAll();
                    };
                    position.RegisterValueChangedCallback(positionCallback);
                    rotation.RegisterValueChangedCallback(rotationCallback);
                    scale.RegisterValueChangedCallback(scaleCallback);

                    root.schedule.Execute(() =>
                    {
                        position.value = atomTransform.Position;
                        rotation.value = atomTransform.Rotation.eulerAngles;
                        scale.value = atomTransform.Scale;
                    })
                    .Every(100)
                    .Until(() => constructedElement.SelectedAtomObject == atomObject);

                    atomTransformView.StartFadeInAnimation(1.5f);
                }
                else
                {
                    atomTransformView.StartFadeOutAnimation(1.5f, () =>
                    {
                        atomTransformView.style.display = DisplayStyle.None;
                    });
                }
            };

            root.Add(atomTransformView);
        }

        

        private void SetUp_EditorSettings(ConstructedElement constructedElement, VisualElement root)
        {
            var cSharpFoldout = new Foldout { text = "GUI Settings" }; 

            var nonSelectedSliderSize = new Slider("GUI non selected point size:", 0, 1, SliderDirection.Horizontal, 0.05f);
            nonSelectedSliderSize.value = constructedElement.SelectedSphereRadius;
            nonSelectedSliderSize.RegisterValueChangedCallback(e =>
            {
                var thisSlider = (Slider)e.target;
                thisSlider.value = e.newValue;
                constructedElement.SelectedSphereRadius = e.newValue;
            });
            cSharpFoldout.Add(nonSelectedSliderSize);

            var selectedSliderSize = new Slider("GUI selected point size:", 0, 1, SliderDirection.Horizontal, 0.05f);
            selectedSliderSize.value = constructedElement.NonSelectedSphereRadius;
            selectedSliderSize.RegisterValueChangedCallback(e =>
            {
                var thisSlider = (Slider)e.target;
                thisSlider.value = e.newValue;
                constructedElement.SelectedSphereRadius = e.newValue;
            });
            cSharpFoldout.Add(selectedSliderSize);

            var selectedColorField = new ColorField("Selected Atoms Color");
            selectedColorField.value = constructedElement.SelectedAtomColor;
            selectedColorField.RegisterValueChangedCallback(e =>
            {
                var thisField = (ColorField)e.target;
                thisField.value = e.newValue;
                constructedElement.SelectedAtomColor = e.newValue;
            });

            var nonSelectedColorField = new ColorField("Non Selected Atoms Color");
            nonSelectedColorField.value = constructedElement.NonSelectedAtomColor;
            nonSelectedColorField.RegisterValueChangedCallback(e =>
            {
                var thisField = (ColorField)e.target;
                thisField.value = e.newValue;
                constructedElement.NonSelectedAtomColor = e.newValue;
            });


            cSharpFoldout.Add(selectedColorField);
            cSharpFoldout.Add(nonSelectedColorField);
            root.Add(cSharpFoldout);
        }

        private void SetUp_AtomsList_As_Property(VisualElement root)
        {
            SerializedProperty atomsSerializedProperty = serializedObject.FindProperty("atoms");
            PropertyField atomsField = new PropertyField(atomsSerializedProperty, "Atoms");
            root.Add(atomsField);
        }

        private void SetUp_EdgeList_As_Property(VisualElement root)
        {
            SerializedProperty atomsSerializedProperty = serializedObject.FindProperty("edges");
            PropertyField atomsField = new PropertyField(atomsSerializedProperty, "Edges");
            root.Add(atomsField);
        }
        internal void SwitchTools(OverlayToolsType toolsType)
        {
            this._toolsType = toolsType;
            SceneView.RepaintAll();
        }

        #endregion

        #region Scene

        private void OnSceneGUI()
        {
            ConstructedElement constructedElement = (ConstructedElement)target;
            var atoms = constructedElement.Atoms;

            var selectedAtoms = constructedElement.SelectedAtomObjects;
            bool isMultiSelection = constructedElement.SelectedAtomObjects.Count > 1;

            Vector3 centerPosition = Vector3.zero;
            Quaternion centerRotation = Quaternion.identity;

            if (isMultiSelection)
            {
                foreach (Atom atom in selectedAtoms)
                {
                    AtomEngineTransform transform = atom.GetComponent<AtomEngineTransform>();
                    centerPosition += transform.Position;
                }
                centerPosition /= selectedAtoms.Count;

                switch (_toolsType)
                {
                    case OverlayToolsType.Translate:
                        // Создаем PositionHandle в центре всех выделенных атомов
                        Vector3 newCenterPosition = Handles.PositionHandle(centerPosition, Quaternion.identity);

                        if (newCenterPosition != centerPosition)  // Проверяем, переместился ли центр
                        {
                            Vector3 offset = newCenterPosition - centerPosition;  // Сдвиг центра
                            foreach (Atom selectedAtom in selectedAtoms)
                            {
                                AtomEngineTransform selectedTransform = selectedAtom.GetComponent<AtomEngineTransform>();
                                selectedTransform.SetPosition(selectedTransform.Position + offset);  // Сдвигаем каждый атом на смещение центра
                                constructedElement.AtomMoved(selectedAtom);
                                Undo.RecordObject(constructedElement, $"Moved Atom id: {selectedAtom.GetComponent<AtomEngineAtomIndex>().Index}");
                            }
                        }
                        break;

                    case OverlayToolsType.Rotate:
                        Quaternion newCenterRotation = Handles.RotationHandle(centerRotation, centerPosition);

                        if (newCenterRotation != centerRotation)
                        {
                            Quaternion rotationDelta = newCenterRotation * Quaternion.Inverse(centerRotation);
                            foreach (Atom selectedAtom in selectedAtoms)
                            {
                                AtomEngineTransform selectedTransform = selectedAtom.GetComponent<AtomEngineTransform>();
                                Vector3 direction = selectedTransform.Position - centerPosition;
                                Vector3 newDirection = rotationDelta * direction;
                                selectedTransform.SetPosition(centerPosition + newDirection);
                                constructedElement.AtomMoved(selectedAtom);
                                Undo.RecordObject(constructedElement, $"Rotated Atom id: {selectedAtom.GetComponent<AtomEngineAtomIndex>().Index}");
                            }
                        }
                        break;
                }
            }

            for (int i = 0; i < atoms.Length; i++)
            {
                Atom atom = atoms[i];
                AtomEngineTransform transform = atom.GetComponent<AtomEngineTransform>();
                AtomEngineAtomIndex atomIndex = atom.GetComponent<AtomEngineAtomIndex>();
                Undo.RecordObject(constructedElement, $"Move Atom id {atomIndex.Index}");

                bool isSelected = selectedAtoms.Contains(atom);

                Handles.color = isSelected ? constructedElement.SelectedAtomColor : constructedElement.NonSelectedAtomColor;
                float handlerSphereRadius = isSelected ? constructedElement.SelectedSphereRadius : constructedElement.NonSelectedSphereRadius;
                HandleAtomSelection(constructedElement, atom);

                if (isSelected && !isMultiSelection)
                { 
                    switch (_toolsType)
                    {
                        case OverlayToolsType.Translate:
                            Vector3 newPosition = Handles.PositionHandle(transform.Position, transform.Rotation); 
                            if (newPosition != transform.Position)
                            {
                                transform.SetPosition(newPosition);
                                AtomMoved(constructedElement, atom);
                                Undo.RecordObject(constructedElement, $"Moved Atom id: {atomIndex.Index}");
                            }
                            break;

                        case OverlayToolsType.Rotate:
                            Quaternion newRotation = Handles.RotationHandle(transform.Rotation, transform.Position);
                            if (newRotation != transform.Rotation)
                            {
                                transform.Rotation = newRotation;
                                AtomRotate(constructedElement, atom);
                                Undo.RecordObject(constructedElement, $"Rotated Atom id: {atomIndex.Index}");
                            }
                            break;
                    }
                }

                Handles.SphereHandleCap(0, transform.Position, Quaternion.identity, handlerSphereRadius, EventType.Repaint);
                Handles.Label(transform.Position + Vector3.up * 0.25f, $"Atom {atom.GetComponent<AtomEngineAtomIndex>().Index}");
            }
             
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }
        private void AtomMoved(ConstructedElement constructedElement, Atom atom) => constructedElement.AtomMoved(atom);
        private void AtomRotate(ConstructedElement constructedElement, Atom atom) => constructedElement.AtomRotate(atom);
        private void HandleAtomSelection(ConstructedElement constructedElement, Atom atom)
        {
            Event e = Event.current;
            if (e == null) return; 

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Vector2 atomScreenPosition = HandleUtility.WorldToGUIPoint(atom.GetComponent<AtomEngineTransform>().Position);
                float distance = Vector2.Distance(e.mousePosition, atomScreenPosition);
                float selectionRadius = 10f;

                if (distance < selectionRadius)
                { 
                    if (e.shift || e.control)
                    {
                        if (constructedElement.SelectedAtomObjects.Contains(atom)) constructedElement.SelectedAtomObjects.Remove(atom); 
                        else constructedElement.SelectedAtomObjects.Add(atom);
                         
                        UpdateSelectedAtomObject(atom, constructedElement);
                    }
                    else
                    { 
                        if (constructedElement.SelectedAtomObject == atom) return;
                        else
                        {
                            UpdateSelectedAtomObject(atom, constructedElement);
                            constructedElement.SelectedAtomObjects.Clear();
                            constructedElement.SelectedAtomObjects.Add(constructedElement.SelectedAtomObject);
                        }
                    } 
                    SceneView.RepaintAll();
                    e.Use();

                    OnAtomSelected?.Invoke(constructedElement.SelectedAtomObject);
                }
            }
        }

        private void UpdateSelectedAtomObject(AtomObject atomObject, ConstructedElement constructedElement)
        {
            if (constructedElement.SelectedAtomObject == atomObject) return;
            if (constructedElement.SelectedAtomObject != null) constructedElement.PreviousSelectedAtomObject = constructedElement.SelectedAtomObject;
            constructedElement.SelectedAtomObject = atomObject;
        }

        #endregion

    }
}
