using AtomEngine.Meshes.Constructor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
namespace AtomEngine.VisualElements
{
    public class ConstructorOnInspector : ConstructorElementView
    {
        public ConstructorOnInspector(ConstructedElement constructedElement, SerializedObject serializedObject) : base(constructedElement, serializedObject)
        {
            SetUp_FigureCreation_Section(contentContainer);
            SetUp_AtomsList_As_Property(contentContainer);
            SetUp_EdgeList_As_Property(contentContainer);
            SetUp_FacesList_As_Property(contentContainer);
            SetUp_MarkerConfiguration_As_Property(contentContainer);
        }

        private void SetUp_FigureCreation_Section(VisualElement root)
        {
            VisualElement createFigure = new VisualElement();
            createFigure.style.flexDirection = FlexDirection.Column;

            Button createCubeBtn = new Button(() => constructedElement.GenerateCube()) { text = "Cube" };
            Button createSphereBtn = new Button(() => constructedElement.GenerateSphere()) { text = "Sphere" };
            Button createCapsuleBtn = new Button(() => constructedElement.GenerateCapsule()) { text = "Capsule" };
            Button createPlaneBtn = new Button(() => constructedElement.GeneratePlane()) { text = "Plane" };

            createFigure.Add(createCubeBtn);
            createFigure.Add(createSphereBtn);
            createFigure.Add(createCapsuleBtn);
            createFigure.Add(createPlaneBtn);

            root.Add(createFigure);
        }

        private void SetUp_FaceModify(VisualElement root)
        {
            VisualElement faceModify = new VisualElement();

            Button makeNewAtom = new Button(() => constructedElement.MakeNewAtom()) { text = "Make Atom" };
            Button makeEdgeAtom = new Button(() => constructedElement.MakeEdgeAtom()) { text = "Make Edge" };

            root.Add(faceModify);
        }

        private void SetUp_AtomsList_As_Property(VisualElement root)
        {
            SerializedProperty atomsSerializedProperty = serializedObject.FindProperty("atoms");
            PropertyField atomsField = new PropertyField(atomsSerializedProperty, "Atoms");
            root.Add(atomsField);
        }
        private void SetUp_EdgeList_As_Property(VisualElement root)
        {
            SerializedProperty edgesSerializedProperty = serializedObject.FindProperty("edges");
            PropertyField edgesField = new PropertyField(edgesSerializedProperty, "Edges");
            root.Add(edgesField);
        }
        private void SetUp_FacesList_As_Property(VisualElement root)
        {
            SerializedProperty facesSerializedProperty = serializedObject.FindProperty("faces");
            PropertyField facesField = new PropertyField(facesSerializedProperty, "Faces");
            root.Add(facesField);
        }
        private void SetUp_MarkerConfiguration_As_Property(VisualElement root)
        {
            SerializedProperty facesSerializedProperty = serializedObject.FindProperty("markerConfig");
            PropertyField facesField = new PropertyField(facesSerializedProperty, "Config");
            root.Add(facesField);
        }
        
        internal void AtomObject_Selected_Handler(List<AtomObject> atomObjects)
        { 
            List<AtomObjectView> prevTransforms = contentContainer.FindElementsOfType<AtomObjectView>().ToList();
            if (prevTransforms != null && prevTransforms.Count() > 0)
            {
                for (int i = 0; i < prevTransforms.Count(); i++)
                {
                    prevTransforms[i].Dispose();
                    contentContainer.Remove(prevTransforms[i]);
                }
            }

            foreach (AtomObject atomObject in atomObjects)
            {
                AtomObjectView atomObjectView = new AtomObjectView(atomObject);
                contentContainer.Insert(0, atomObjectView);
                atomObjectView.Show();

                AtomTransformView atomTransformView = (AtomTransformView)atomObjectView.Children().FirstOrDefault(e => e is AtomTransformView);
                if (atomObject is Atom atom)
                {
                    atomTransformView.BindStandartCallbacks((e) =>
                    {
                        constructedElement.AtomMoved(atom);
                    });
                }
            } 
        }
    }
}
