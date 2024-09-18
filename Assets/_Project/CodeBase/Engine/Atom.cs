using AtomEngine.Components; 
using MIConvexHull;
using UnityEngine;
using System; 

namespace AtomEngine
{
    [Serializable]
    public class Atom : AtomObject, IVertex
    {
        public Atom()
        {
            this.AddComponent<AtomEngineTransform>(Vector3.zero);
            this.AddComponent<AtomEngineAtomIndex>(0);
        }

        public double[] Position { get {
                IVertex iVert = GetComponent<AtomEngineTransform>(); 
                return iVert.Position; 
            } 
        } 
    }
}
