using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using MvLib;

namespace AtomEngine.VisualElements
{
    public class AtomObjectView : AtomView
    {
        public override string Name { get; protected set; } = "ATOM_OBJECT";

        private AtomObject aObject;
        private List<AtomTransformView> transformViews = new List<AtomTransformView>();

        public AtomObjectView(AtomObject aObject) : base()
        {
            this.aObject = aObject;

            VisualElement title = GetTitle(this.aObject);
            contentContainer.Add(title); 
            
            List<AtomEngineTransform> transforms = aObject.GetComponents<AtomEngineTransform>();
            foreach (AtomEngineTransform el in transforms)
            {
                AtomTransformView transformView = new AtomTransformView(el);
                transformViews.Add(transformView);
                contentContainer.Add(transformView);
            }

            AddStype();
        }

        private VisualElement GetTitle(AtomObject aObject)
        {
            Label label = new Label($"{aObject.Name}:");
            label.style.marginBottom = 5;
            return label;
        }
        private void AddStype()
        {
            contentContainer.style.marginBottom = 10;
            contentContainer.style.borderBottomLeftRadius = 6;
            contentContainer.style.borderBottomRightRadius = 6;
            contentContainer.style.borderTopLeftRadius = 6;
            contentContainer.style.borderTopLeftRadius = 6;
            contentContainer.style.borderBottomColor = new StyleColor(Color.black);
            contentContainer.style.borderBottomWidth = 3;
        }
        public void Hide()
        {
            contentContainer.StartFadeOutAnimation(1.5f, () =>
            {
                contentContainer.StartFoldAnimation(50, 0.5f, () =>
                {
                    contentContainer.style.display = DisplayStyle.None;
                });
            });
        }
        public void Show()
        {
            contentContainer.style.display = DisplayStyle.Flex;
            contentContainer.StartFadeInAnimation(1.5f);
            foreach (var transformView in transformViews)
            {
                transformView.Show();
            }
        }
        public override void Update()
        {
            foreach (var transformView in transformViews)
            {
                transformView.Update();
            }
        }
        public override void Dispose()
        {
            foreach (var e in transformViews)
            {
                e.Dispose();
            }
        }
    } 
}
