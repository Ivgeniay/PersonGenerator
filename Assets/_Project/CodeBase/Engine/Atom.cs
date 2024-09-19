using AtomEngine.Components; 
using AtomEngine.Meshes;
using System;

namespace AtomEngine
{
    [Serializable]
    public class Atom : AtomObject
    {
        public Atom() : base("Atom")
        {
            this.AddComponent<AtomEngineAtomIndex>(this, 0);
            this.AddComponent<AtomEnginePointDistanceCheckerComponent>(this);
        }
         
        public static implicit operator Vertex(Atom atom)
        {
            var position = atom.GetComponent<AtomEngineTransform>().Position;
            var index = atom.GetComponent<AtomEngineAtomIndex>().Index;
            var vert = new Vertex(position);
            vert.Index = index;
            return vert;
        }

        public override string ToString()
        {
            var position = this.GetComponent<AtomEngineTransform>().Position;
            var index = this.GetComponent<AtomEngineAtomIndex>().Index;
            return $"Atom id:{index} pos:{position}";
        }
    }
}
