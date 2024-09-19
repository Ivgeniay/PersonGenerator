using System;
using UnityEditor;
using UnityEngine;

namespace AtomEngine.Components
{
    [Serializable]
    public class AtomEngineLineDistanceCheckerComponent : AtomEngineDistanceCheckerComponent
    {
        public AtomEngineLineDistanceCheckerComponent(AtomObject parenObject, Vector3 pointOne, Vector3 pointTwo) : base(parenObject, new Vector3[] { pointOne, pointTwo })  { }

        public override bool CheckDistance(Vector3 position)
        {
            if (parenObject is Edge edge)
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

