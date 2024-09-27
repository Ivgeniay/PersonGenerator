using AtomEngine.VisualElements;
using MvLib.Testing.Inspector;
using UnityEngine.UIElements;
using AtomEngine.Components;
using UnityEditor;
using UnityEngine;
using System.Linq;
using AtomEngine;
using System;
using MvLib;
using MvLib.Reactive; 

using static AtomEngine.SceneViews.SceneToolsOverlay; 
using Edge = AtomEngine.Edge;

namespace Inspector
{

    [CustomEditor(typeof(AtomConstructed))]
    internal class AtomConstructorInspector : TestedEditor
    {
        public static AtomConstructorInspector Instance;

        private ReactiveProperty<ModeType> modeType = ModeType.None;
        private ReactiveProperty<ToolsType> _toolsType = ToolsType.Translate;
        private ReactiveProperty<HandlesOrientationType> handlesOrientatiom = HandlesOrientationType.World;

        private IDisposable modeTypeDisposable;

        private AtomConstructorOnInspector constructorOnInspector;
        private AtomConstructorOnScene constructorOnScene;
        private AtomConstructed constructedElement;
        private VisualElement root;

        #region Inspector 
        public override VisualElement CreateInspectorGUI()
        {
            Instance = this;
            AtomConstructed constructedElement = (AtomConstructed)target; 
            constructedElement.OnEnabled();

            root = new VisualElement();

            constructorOnInspector = new AtomConstructorOnInspector(constructedElement, serializedObject);
            constructorOnScene.SelectedObject.OnChange += constructorOnInspector.AtomObject_Selected_Handler;
            root.Add(constructorOnInspector);

            modeTypeDisposable = modeType.Subscribe((e) =>
            {
                ModeChangeHandler(e);
            }); 
             
            root.schedule.Execute(Update).Every(100);
            return root;
        }
        private void Update()
        {
            foreach (AtomView e in root.FindElementsOfType<AtomView>()) e.Update(); 
        }
        public void OnInspectorClose()
        {
            UnselectAll();
            constructedElement.OnDiasabled();
        }
        #endregion

        #region Scene
        private void OnEnable()
        {
            constructedElement = (AtomConstructed)target;
            constructorOnScene = new AtomConstructorOnScene(constructedElement); 

            AtomObject[] atomObjects = constructedElement.AtomObject; 
        }
        private void OnDisable()
        {
            modeTypeDisposable?.Dispose();
        }
        private void OnSceneGUI()
        {
            constructorOnScene.DrawMarker(constructedElement.AtomObject); 
            constructorOnScene.DrawPositionHandle(_toolsType, handlesOrientatiom, AtomEngineSelection.SelectedAtomObjects.ToArray()); 
        }
        internal void UnselectAll() => constructorOnScene.UnselectAll();
        private void ModeChangeHandler(ModeType modeType)
        {
            constructorOnScene?.SelectedObject?.Clear();

            VisualElement surfMod = constructorOnInspector.Q<VisualElement>(AtomConstructorOnInspector.SURFACE_MODIFYER);
            if (modeType == ModeType.Face)
            {
                surfMod.style.display = DisplayStyle.Flex;
            }
            else
            {
                surfMod.style.display = DisplayStyle.None;
            }
        }
        internal void SwitchTools(ToolsType toolsType) => this._toolsType = toolsType;
        internal void SwitchMode(ModeType modeType) => this.modeType = modeType; 
        internal void SwitchHandlesOrientationType(HandlesOrientationType handlesOrientationType) => this.handlesOrientatiom = handlesOrientationType;
        #endregion 

    }

} 