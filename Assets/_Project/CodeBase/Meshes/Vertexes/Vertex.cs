using MIConvexHull;
using UnityEngine;
using System;

namespace AtomEngine.Meshes
{
    [Serializable]
    public class Vertex : IVertex
    {
        [SerializeField] public Vector3 Position;
        [SerializeField] public int Index;
        double[] IVertex.Position => new double[] { Position.x, Position.y, Position.z };

        public Vertex(int x, int y, int z) => Position = new Vector3(x, y, z); 
        public Vertex(Vector3 position) => this.Position = position;
        public override string ToString() => $"Vector: Position:{Position} Index:{Index}";
        

        public static implicit operator Vector3(Vertex vertex) => vertex.Position;
    }
}
