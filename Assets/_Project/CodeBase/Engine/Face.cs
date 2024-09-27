using AtomEngine.Components;
using UnityEngine; 
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

            AtomEngineTransform[] parameters = this.atoms.Select(e => e.Transform).ToArray();
            transform = this.AddComponent<MultiAtomsTransform>(parameters);
            this.AddComponent<AtomEngineAtomIndex>(0);
            this.AddComponent<AtomEngineSquareDistanceCheckerComponent>();
        }
    }
}
