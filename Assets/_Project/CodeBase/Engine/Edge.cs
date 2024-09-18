using System.Collections.Generic;
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

        public Edge(Atom atom, Atom atom2)
        {
            this.atom = atom;
            this.atom2 = atom2;

            this.AddComponent<AtomEngineTransform>(Vector3.zero);
            this.AddComponent<AtomEngineAtomIndex>(0);
        }

        public void Draw()
        {
            Debug.DrawLine(atom.GetComponent<AtomEngineTransform>().Position, atom2.GetComponent<AtomEngineTransform>().Position, Color.red);
            //Debug.Log("Draw");
        }
    }
}
