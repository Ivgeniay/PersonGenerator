using AtomEngine.Meshes.Constructor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using MvLib;

namespace AtomEngine.VisualElements
{
    public class AtomConstructorOnInspector : VisualElement
    {
        public const string SURFACE_MODIFYER = "FACE_MODIFY";

        protected AtomConstructed constructedElement;
        protected SerializedObject serializedObject;

        public AtomConstructorOnInspector(AtomConstructed constructedElement, SerializedObject serializedObject)
        {
            this.constructedElement = constructedElement;
            this.serializedObject = serializedObject;
             
            Foldout debugFoldout = new Foldout() { text = "Config", };

            SetUp_FigureCreation_Section(contentContainer); 
            SetUp_FaceModify(contentContainer);

            contentContainer.Add(debugFoldout);

            SetUp_AtomsList_As_Property(debugFoldout);
            SetUp_EdgeList_As_Property(debugFoldout);
            SetUp_FacesList_As_Property(debugFoldout);
            SetUp_MarkerConfiguration_As_Property(debugFoldout);
        }

        private void SetUp_FigureCreation_Section(VisualElement root)
        {
            VisualElement createFigure = new VisualElement();
            createFigure.style.flexDirection = FlexDirection.Row;

            Button createCubeBtn = new Button(() => constructedElement.GenerateCube()) { text = "Cube" };
            Button createSphereBtn = new Button(() => constructedElement.GenerateSphere()) { text = "Sphere" };
            Button createCapsuleBtn = new Button(() => constructedElement.GenerateCapsule()) { text = "Capsule" };
            Button createPlaneBtn = new Button(() => constructedElement.GeneratePlane()) { text = "Plane" };
            Button createCylinderBtn = new Button(() => constructedElement.GenerateCylinder()) { text = "Cylinder" };
            Button createTorusBtn = new Button(() => constructedElement.GenerateTorus()) { text = "Torus" };

            createFigure.Add(createCubeBtn);
            createFigure.Add(createSphereBtn);
            createFigure.Add(createCapsuleBtn);
            createFigure.Add(createPlaneBtn);
            createFigure.Add(createCylinderBtn);
            createFigure.Add(createTorusBtn);

            root.Add(createFigure);
        }

        private void SetUp_FaceModify(VisualElement root)
        {
            VisualElement faceModify = new VisualElement();
            faceModify.name = SURFACE_MODIFYER;

            faceModify.style.flexDirection = FlexDirection.Row;

            Button makeNewAtom = new Button(() => constructedElement.MakeNewAtom()) { text = "Make Atom" };
            Button makeEdgeAtom = new Button(() => constructedElement.MakeEdgeAtom()) { text = "Make Edge" };

            faceModify.Add(makeEdgeAtom);
            faceModify.Add(makeNewAtom);

            root.Add(faceModify);
        }

        private void SetUp_AtomsList_As_Property(VisualElement root)
        {
            SerializedProperty atomsSerializedProperty = serializedObject.FindProperty("atoms");
            PropertyField atomsField = new PropertyField(atomsSerializedProperty, "Atoms");
            atomsField.SetEnabled(false);
            root.Add(atomsField);
        }
        private void SetUp_EdgeList_As_Property(VisualElement root)
        {
            SerializedProperty edgesSerializedProperty = serializedObject.FindProperty("edges");
            PropertyField edgesField = new PropertyField(edgesSerializedProperty, "Edges");
            edgesField.SetEnabled(false);
            root.Add(edgesField);
        }
        private void SetUp_FacesList_As_Property(VisualElement root)
        {
            SerializedProperty facesSerializedProperty = serializedObject.FindProperty("faces");
            PropertyField facesField = new PropertyField(facesSerializedProperty, "Faces");
            facesField.SetEnabled(false);
            root.Add(facesField);
        }
        private void SetUp_MarkerConfiguration_As_Property(VisualElement root)
        {
            SerializedProperty facesSerializedProperty = serializedObject.FindProperty("markerConfig");
            PropertyField facesField = new PropertyField(facesSerializedProperty, "Marker Configurations");
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

                //AtomTransformView atomTransformView = (AtomTransformView)atomObjectView.Children().FirstOrDefault(e => e is AtomTransformView);
                //if (atomObject is Atom atom)
                //{
                //    atomTransformView.BindStandartCallbacks((e) =>
                //    {
                //        constructedElement.AtomMoved(atom);
                //    });
                //}
            } 
        }
    }
}
