using AtomEngine.Components;
using UnityEngine;
using System;

namespace AtomEngine
{
    [Serializable]
    public class Edge : AtomObject
    {
        [SerializeField] private Atom atom;
        [SerializeField] private Atom atom2;

        public Atom Atom { get => atom; }
        public Atom Atom2 { get => atom2; }

        public Edge(Atom atom, Atom atom2) : base("Edge")
        {
            this.atom = atom;
            this.atom2 = atom2;

            var parameters = new AtomEngineTransform[] { atom.Transform, atom2.Transform }; 
            transform = this.AddComponent<MultiAtomsTransform>(parameters);

            this.AddComponent<AtomEngineAtomIndex>(0);
            this.AddComponent<AtomEngineLineDistanceCheckerComponent>();

            AddComponent<AtomEngineTransform>(Atom.Transform);
            AddComponent<AtomEngineTransform>(Atom2.Transform);
        }

        public override string ToString()
        {
            return $"Edge {Atom} : {Atom2}";
        }
    }
}
