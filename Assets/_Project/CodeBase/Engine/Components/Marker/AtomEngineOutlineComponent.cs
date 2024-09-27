using UnityEngine;
using System;

namespace AtomEngine.Components
{
    public delegate void OnOutlineDelegate(AtomObject atomObject, bool isEnter);
    [Serializable]
    public class AtomEngineOutlineComponent : AtomEngineComponent
    {
        public OnOutlineDelegate OnHoverDelegate;
        public OnOutlineDelegate OnSelectedDelegate;

        [SerializeField] public float NonSelectedWidth = .05f;
        [SerializeField] public float SelectedWidth = .025f;
        [SerializeField] public Color SelectedColor = new Color(61, 226, 178, 150);
        [SerializeField] public Color NonSelecteColor = new Color32(144, 178, 255, 150);
        [SerializeField] public Color HoveredColor = Color.red;
        [SerializeField] public Color DisabledColor = new Color32(144, 178, 255, 150);

        [SerializeField] private bool isHovered = false;
        [SerializeField] private bool isSelected = false;
        [SerializeField] private bool isDisabled = false;

        public bool IsHovered { get => isHovered; }
        public bool IsSelected { get => isSelected; }
        public bool IsDisabled { get => isDisabled; }

        public AtomEngineOutlineComponent(MarkerConfig markerModel) : base()
        {
            NonSelectedWidth = markerModel.NonSelectedWidth;
            SelectedWidth = markerModel.SelectedWidth;
            SelectedColor = markerModel.SelectedColor;
            NonSelecteColor = markerModel.NonSelecteColor;
            HoveredColor = markerModel.HoveredColor;
            DisabledColor = markerModel.DisabledColor;
        }

        public void OnHover()
        {
            isHovered = true;
            OnHoverDelegate?.Invoke(parentObject, true);
        }

        public void OnUnhover()
        {
            isHovered = false;
            OnHoverDelegate?.Invoke(parentObject, false);
        }

        public void OnSelected()
        {
            isSelected = true;
            OnSelectedDelegate?.Invoke(parentObject, true);
        }

        public void OnDeselected()
        {
            isSelected = false;
            OnSelectedDelegate?.Invoke(parentObject, false);
        }
    }

    [Serializable]
    public class MarkerConfig
    {
        public float NonSelectedWidth = .05f;
        public float SelectedWidth = .025f;
        public Color SelectedColor = new Color(61, 226, 178, 150);
        public Color NonSelecteColor = new Color32(144, 178, 255, 10);
        public Color HoveredColor = Color.red;
        public Color DisabledColor = new Color32(144, 178, 255, 150); 
    }
}
