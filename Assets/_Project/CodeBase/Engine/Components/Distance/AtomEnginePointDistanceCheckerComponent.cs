using UnityEditor;
using UnityEngine;
using System;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEnginePointDistanceCheckerComponent : AtomEngineDistanceCheckerComponent
    {
        public AtomEnginePointDistanceCheckerComponent() : base() { }
        public override bool CheckDistance(Vector3 position)
        {
            Vector2 screenPos1 = HandleUtility.WorldToGUIPoint(parentObject.Transform.Position);
            float distanceToPoint = Vector2.Distance(position, screenPos1);

            return distanceToPoint < SentityDistance;
        }
    }
}
