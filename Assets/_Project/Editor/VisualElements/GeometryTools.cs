using UnityEngine.UIElements;
using UnityEngine;
using static AtomEngine.SceneViews.SceneToolsOverlay;
using System;

namespace AtomEngine.VisualElements
{
    public delegate void SwitchModeType(ModeType modeType);
    public delegate void SwitchToolsType(ToolsType toolsType);
    public delegate void SwitchHandlesOrientationType(HandlesOrientationType toolsType);
    public class GeometryTools : VisualElement
    {
        public SwitchModeType OnModeTypeChanged;
        public SwitchToolsType OnToolsTypeChanged;
        public SwitchHandlesOrientationType OnHandlesOrientationTypeChanged;

        VisualElement translateToolsWrap;
        VisualElement notificationWrap;
        VisualElement modeWrap;

        public GeometryTools()
        {
            contentContainer.Add(CreatePanelContent());
        } 

        public VisualElement CreatePanelContent()
        {
            VisualElement root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignContent = Align.Center;

            notificationWrap = new VisualElement();
            modeWrap = new VisualElement();
            translateToolsWrap = new VisualElement();

            Notification_Configurating();
            Mode_Configurating();
            Tools_Configurating();

            root.Add(notificationWrap);
            root.Add(modeWrap);
            root.Add(translateToolsWrap);

            return root;
        }

        private void Notification_Configurating()
        {
            Label label = new Label("Select Constructed Element");
            notificationWrap.Add(label);

            notificationWrap.style.display = DisplayStyle.None;
        }
        private void Mode_Configurating()
        {
            Label label = new Label("Mode");
            label.style.unityTextAlign = TextAnchor.UpperCenter;

            VisualElement buttonWrap = new VisualElement();
            buttonWrap.style.flexDirection = FlexDirection.Row;

            Button objectModeBtn = new Button(() =>
            {
                Switch_OverlayStage(OverlayStage.ToolsTools);
                OnModeTypeChanged?.Invoke(ModeType.Object);
            })
            { text = "Object" };

            Button atomModeBtn = new Button(() =>
            {
                Switch_OverlayStage(OverlayStage.ToolsTools);
                OnModeTypeChanged?.Invoke(ModeType.Atom);
            })
            { text = "Atom" };

            Button edgeModeBtn = new Button(() =>
            {
                Switch_OverlayStage(OverlayStage.ToolsTools);
                OnModeTypeChanged?.Invoke(ModeType.Edge);
            })
            { text = "Edge" };

            Button faceModeBtn = new Button(() =>
            {
                Switch_OverlayStage(OverlayStage.ToolsTools);
                OnModeTypeChanged?.Invoke(ModeType.Face);
            })
            { text = "Face" };

            buttonWrap.Add(objectModeBtn);
            buttonWrap.Add(atomModeBtn);
            buttonWrap.Add(edgeModeBtn);
            buttonWrap.Add(faceModeBtn);

            modeWrap.Add(label);
            modeWrap.Add(buttonWrap);

            modeWrap.style.display = DisplayStyle.None;
        }
        private void Tools_Configurating()
        {
            Label label = new Label("Tools");
            label.style.unityTextAlign = TextAnchor.UpperCenter;

            VisualElement buttonToolsWrap = new VisualElement();
            buttonToolsWrap.style.flexDirection = FlexDirection.Row;

            Button translateButton = new Button(() =>
            {
                OnToolsTypeChanged?.Invoke(ToolsType.Translate);
            })
            { text = "Move" };
            Button rotateButton = new Button(() =>
            {
                OnToolsTypeChanged?.Invoke(ToolsType.Rotate);
            })
            { text = "Rotate" };
            Button scaleButton = new Button(() =>
            {
                OnToolsTypeChanged?.Invoke(ToolsType.Scale);
            })
            { text = "Scale" };

            Button backBtn = new Button(() =>
            {
                OnToolsTypeChanged?.Invoke(ToolsType.Translate);
                Switch_OverlayStage(OverlayStage.ModeType);
                OnModeTypeChanged?.Invoke(ModeType.None);
            })
            { text = "Back" };

            //buttonToolsWrap.Add(backBtn);
            buttonToolsWrap.Add(translateButton);
            buttonToolsWrap.Add(rotateButton);
            buttonToolsWrap.Add(scaleButton);
            buttonToolsWrap.Add(backBtn);

            VisualElement buttonHandlesWrap = new VisualElement();
            buttonHandlesWrap.style.flexDirection = FlexDirection.Row;
            Button worldButton = new Button(() =>
            {
                OnHandlesOrientationTypeChanged?.Invoke(HandlesOrientationType.World);
            })
            { text = "World" };

            Button localBtn = new Button(() =>
            {
                OnHandlesOrientationTypeChanged?.Invoke(HandlesOrientationType.Local);
            })
            { text = "Local" };
            buttonHandlesWrap.Add(worldButton);
            buttonHandlesWrap.Add(localBtn);

            translateToolsWrap.Add(label);
            translateToolsWrap.Add(buttonToolsWrap);
            translateToolsWrap.Add(buttonHandlesWrap);

            translateToolsWrap.style.flexDirection = FlexDirection.Column;
            translateToolsWrap.style.display = DisplayStyle.None;
        }

        internal void Switch_OverlayStage(OverlayStage overlayStage)
        {
            switch (overlayStage)
            {
                case OverlayStage.Notification:
                    notificationWrap.style.display = DisplayStyle.Flex;
                    modeWrap.style.display = DisplayStyle.None;
                    translateToolsWrap.style.display = DisplayStyle.None;
                    break;

                case OverlayStage.ModeType:
                    notificationWrap.style.display = DisplayStyle.None;
                    modeWrap.style.display = DisplayStyle.Flex;
                    translateToolsWrap.style.display = DisplayStyle.None;
                    break;

                case OverlayStage.ToolsTools:
                    notificationWrap.style.display = DisplayStyle.None;
                    modeWrap.style.display = DisplayStyle.None;
                    translateToolsWrap.style.display = DisplayStyle.Flex;
                    break;
            }
        }
    }
}
