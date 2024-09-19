using AtomEngine.Components;
using UnityEngine;
using AtomEngine;
using System;
using System.Linq;

namespace AtomEngine
{
    [Serializable]
    public class Face : AtomObject
    {
        [SerializeField] private Atom[] atoms;
        public Atom[] Atoms { get => atoms; } 

        public Face(Atom[] atoms) : base("Face")
        { 
            this.atoms = atoms;
            this.AddComponent<AtomEngineAtomIndex>(this, 0);
            this.AddComponent<AtomEngineSquareDistanceCheckerComponent>(
                this,
                atoms.Select(e=> e.Transform.Position).ToArray());
        }
    }
}
