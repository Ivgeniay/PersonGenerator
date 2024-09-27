using System;
using UnityEditor;
using UnityEngine;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineLineDistanceCheckerComponent : AtomEngineDistanceCheckerComponent
    {
        public AtomEngineLineDistanceCheckerComponent() : base()  { }

        public override bool CheckDistance(Vector3 position)
        {
            if (parentObject is Edge edge)
            {
                Vector2 screenPos1 = HandleUtility.WorldToGUIPoint(edge.Atom.Transform.Position);
                Vector2 screenPos2 = HandleUtility.WorldToGUIPoint(edge.Atom2.Transform.Position); 
                float distanceToEdge = HandleUtility.DistancePointToLineSegment(position, screenPos1, screenPos2);

                return distanceToEdge < SentityDistance; 
            }

            return false;
        }
    }

}

