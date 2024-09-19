using UnityEngine;
using System;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineComponent
    {
        [SerializeReference] protected AtomObject parenObject;
        public AtomEngineTransform Transform => parenObject.Transform;
        public AtomObject Parent { get => parenObject; }

        public AtomEngineComponent(AtomObject parenObject)
        {
            this.parenObject = parenObject;
        }
    }
}
