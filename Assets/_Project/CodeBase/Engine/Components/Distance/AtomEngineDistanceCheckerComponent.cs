using System;
using UnityEngine;

namespace AtomEngine.Components
{
    [Serializable]
    public abstract class AtomEngineDistanceCheckerComponent : AtomEngineComponent
    {
        [SerializeField] public float SentityDistance = 10;

        public AtomEngineDistanceCheckerComponent() : base() { }

        public abstract bool CheckDistance(Vector3 position);
    }
}
