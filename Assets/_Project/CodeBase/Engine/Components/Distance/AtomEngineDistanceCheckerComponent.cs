using System;
using UnityEngine;

namespace AtomEngine.Components
{
    [Serializable]
    public abstract class AtomEngineDistanceCheckerComponent : AtomEngineComponent
    {
        [SerializeField] public float SentityDistance = 10;
        [SerializeField] protected Vector3[] positions;

        public AtomEngineDistanceCheckerComponent(AtomObject parenObject, params Vector3[] positions) : base(parenObject)
        {
            this.positions = positions;
        }

        public abstract bool CheckDistance(Vector3 position);
        public void ChangePointValue(params Vector3[] newPositions)
        {
            if (positions == null) return;

            for (var i = 0; i < positions.Length; i++)
            {
                if (newPositions.Length == i) break; 
                positions[i] = newPositions[i];
            }
        }
    }
}
