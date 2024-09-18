using AtomEngine.Meshes.Constructor;
using UnityEngine.UIElements;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEngine;
using System;
using Inspector;

namespace AtomEngine.SceneViews
{
    [Overlay(typeof(SceneView), "Construct Element Editor")]
    internal class SceneToolsOverlay : Overlay
    {
        VisualElement translateToolsWrap;
        VisualElement notificationWrap;
        ConstructedElement target;

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignContent = Align.Center;

            notificationWrap = new VisualElement();
            translateToolsWrap = new VisualElement();

            WrapConfigurating(translateToolsWrap);
            NotificationConfiguration();

            root.Add(notificationWrap);
            root.Add(translateToolsWrap);

            Selection.selectionChanged += OnSelection;

            if (SetTarget_ConstructionElement_FromSelect()) SwitchOverlayStage(OverlayStage.TranslateTools); 
            else SwitchOverlayStage(OverlayStage.Notification);

            return root;
        }

        private void NotificationConfiguration()
        {
            Label label = new Label("Select Constructed Element");
            notificationWrap.Add(label);
            notificationWrap.style.display = DisplayStyle.None;
        }

        private void WrapConfigurating(VisualElement wrap)
        {
            Label label = new Label("Construct");
            label.style.unityTextAlign = TextAnchor.UpperCenter;

            VisualElement buttonWrap = new VisualElement();
            buttonWrap.style.flexDirection = FlexDirection.Row;

            Button button1 = new Button(() =>
            { 
                SwitchHandleMode(OverlayToolsType.Translate);
            })
            { text = "Move" };
            Button button2 = new Button(() =>
            { 
                SwitchHandleMode(OverlayToolsType.Rotate); 
            })
            { text = "Rotate" };


            buttonWrap.Add(button1);
            buttonWrap.Add(button2);

            wrap.Add(label);
            wrap.Add(buttonWrap);
            wrap.style.display = DisplayStyle.None;
        }

        private static void SwitchHandleMode(OverlayToolsType toolsType)
        {
            GameObject selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                if (ConstructorElementInspector.Instance != null)
                    ConstructorElementInspector.Instance.SwitchTools(toolsType); 
            }
        }

        private void OnSelection()
        {
            if (SetTarget_ConstructionElement_FromSelect())
            {
                SwitchOverlayStage(OverlayStage.TranslateTools);
            }
            else
            {
                SwitchOverlayStage(OverlayStage.Notification); 
            }
        }

        public override void OnWillBeDestroyed()
        {
            Selection.selectionChanged -= OnSelection;
            base.OnWillBeDestroyed();
        } 
        private bool SetTarget_ConstructionElement_FromSelect()
        { 
            if (Selection.activeObject == null)
            { 
                return false;
            }

            Type type = Selection.activeObject.GetType();
            if (Selection.activeObject is GameObject go)
            {
                ConstructedElement constructElement = go.GetComponent<ConstructedElement>();
                if (constructElement) return true; 
            }
             
            return false;
        } 
        private void SwitchOverlayStage(OverlayStage overlayStage)
        {
            switch (overlayStage)
            {
                case OverlayStage.Notification:
                    notificationWrap.style.display = DisplayStyle.Flex;
                    translateToolsWrap.style.display = DisplayStyle.None;
                    break;

                case OverlayStage.TranslateTools:
                    notificationWrap.style.display = DisplayStyle.None;
                    translateToolsWrap.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }

    public enum OverlayStage
    {
        Notification,
        TranslateTools
    }

    public enum OverlayToolsType
    {
        Translate,
        Rotate
    }
}
