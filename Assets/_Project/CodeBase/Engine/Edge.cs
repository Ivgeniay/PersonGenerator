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

            this.AddComponent<AtomEngineAtomIndex>(this, 0);
            this.AddComponent<AtomEngineLineDistanceCheckerComponent>(
                this,
                Atom.Transform.Position, 
                Atom2.Transform.Position);
        }

        public override string ToString()
        {
            return $"Edge {Atom} : {Atom2}";
        }
    }
}
