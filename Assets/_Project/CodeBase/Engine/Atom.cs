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
            transform = this.AddComponent<AtomEngineTransform>();
            this.AddComponent<AtomEngineAtomIndex>(0);
            this.AddComponent<AtomEnginePointDistanceCheckerComponent>();
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
            var position = transform.Position;
            var index = this.GetComponent<AtomEngineAtomIndex>().Index;
            return $"Atom id:{index} pos:{position}";
        }
    }
}
