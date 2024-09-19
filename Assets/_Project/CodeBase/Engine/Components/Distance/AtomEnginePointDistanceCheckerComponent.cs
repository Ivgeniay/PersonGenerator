using UnityEditor;
using UnityEngine;
using System;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEnginePointDistanceCheckerComponent : AtomEngineDistanceCheckerComponent
    {
        public AtomEnginePointDistanceCheckerComponent(AtomObject parenObject) : base(parenObject) { }
        public override bool CheckDistance(Vector3 position)
        {
            Vector2 screenPos1 = HandleUtility.WorldToGUIPoint(parenObject.Transform.Position);
            float distanceToPoint = Vector2.Distance(position, screenPos1);

            return distanceToPoint < SentityDistance;
        }
    }
}
