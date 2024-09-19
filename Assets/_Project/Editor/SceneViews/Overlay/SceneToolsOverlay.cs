using AtomEngine.Meshes.Constructor;
using AtomEngine.VisualElements;
using UnityEngine.UIElements;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEngine; 
using Inspector;
using static AtomEngine.SceneViews.SceneToolsOverlay;

namespace AtomEngine.SceneViews
{
    [Overlay(typeof(SceneView), "Construct Element Editor")]
    public class SceneToolsOverlay : Overlay
    {
        VisualElement translateToolsWrap;
        VisualElement notificationWrap;
        VisualElement modeWrap;

        GeometryTools geoTools;

        public override VisualElement CreatePanelContent()
        {
            geoTools = new GeometryTools(); 

            geoTools.OnModeTypeChanged += Switch_ModeType;
            geoTools.OnToolsTypeChanged += Switch_ToolsType;
            geoTools.OnHandlesOrientationTypeChanged += Switch_HandlesOrientationType;

            Selection.selectionChanged += OnSelection;

            if (IsSelectedObject_ConstructionElement()) geoTools.Switch_OverlayStage(OverlayStage.ToolsTools);
            else geoTools.Switch_OverlayStage(OverlayStage.Notification);

            return geoTools;
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= OnSelection;
            base.OnWillBeDestroyed();
        } 
        private void OnSelection()
        {
            if (IsSelectedObject_ConstructionElement())
            {
                geoTools.Switch_OverlayStage(OverlayStage.ModeType);
                Switch_ModeType(ModeType.None);
            }
            else geoTools.Switch_OverlayStage(OverlayStage.Notification);
        }
        private bool IsSelectedObject_ConstructionElement()
        { 
            if (Selection.activeObject == null) return false;
            if (Selection.activeObject is GameObject go)
                if (go.GetComponent<ConstructedElement>()) return true;
            return false;
        }
        private void Switch_ToolsType(ToolsType toolsType)
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                if (ConstructorElementInspector.Instance != null)
                    ConstructorElementInspector.Instance.SwitchTools(toolsType);
            }
        }
        private void Switch_ModeType(ModeType modeType)
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                if (ConstructorElementInspector.Instance != null)
                    ConstructorElementInspector.Instance.SwitchMode(modeType);
            }
        }

        private void Switch_HandlesOrientationType(HandlesOrientationType handlesOrientationType)
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                if (ConstructorElementInspector.Instance != null)
                    ConstructorElementInspector.Instance.SwitchHandlesOrientationType(handlesOrientationType);
            }
        }

        public enum OverlayStage { Notification, ModeType, ToolsTools }
        public enum ModeType { None, Object, Atom, Edge, Face }
        public enum ToolsType { Translate, Rotate, Scale }
        public enum HandlesOrientationType { World, Local }
    }
}
