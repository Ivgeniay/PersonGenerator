using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
namespace AtomEngine.VisualElements
{
    public class AtomView : VisualElement, IDisposable
    {
        public virtual string Name { get; protected set; }

        public AtomView()
        {
            contentContainer.name = Name;
        }

        public List<T> GetInsite<T>()
        {
            List<T> values = new();
            return values;
        }

        public virtual void Update() { }
        public virtual void Dispose() { }
    }
}
