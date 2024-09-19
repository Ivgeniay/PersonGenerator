using UnityEngine;
using System;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineAtomIndex : AtomEngineComponent
    {
        [field: SerializeField] public int Index { get; set; }
         
        public AtomEngineAtomIndex(AtomObject parenObject, int index) : base(parenObject)
        {
            Index = index;
        }

    }
}
