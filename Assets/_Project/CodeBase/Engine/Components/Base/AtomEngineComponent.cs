using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq; 

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineComponent
    {
        [SerializeReference] protected AtomObject parentObject;
        public AtomEngineTransform Transform => parentObject.Transform;
        public AtomObject Parent
        {
            get => parentObject;
            set
            { 
                if (!IsCalledFromAddComponent())
                {
                    throw new InvalidOperationException("Parent property can only be set from the AddComponent method.");
                } 
                parentObject = value;
            }
        }

        public AtomEngineComponent() { }
        private bool IsCalledFromAddComponent()
        {
            StackTrace stackTrace = new StackTrace();
            return stackTrace.GetFrames()
                             .Any(frame => frame.GetMethod().Name == "AddComponent");
        }

        public virtual void OnEnable() { } 
        public virtual void OnDisable() { } 
    }
}
