using System.Collections.Generic;
using System.Linq;

namespace AtomEngine.Meshes.Figure.Triangles
{
    public struct Triangle : IFigure
    {
        public readonly int[] VerticlesIndex;
        public readonly int[] OtherSideVerticlesIndexes; 
        public readonly Vertex[] Vertices;

        public Triangle(Vertex vert, Vertex vert2, Vertex vert3)
        {
            VerticlesIndex = new int[3] { vert.Index, vert2.Index, vert3.Index };
            OtherSideVerticlesIndexes = new int[3] { vert3.Index, vert2.Index, vert.Index };
            Vertices = new Vertex[3] { vert, vert2, vert3 };
        }

        public IEnumerable<int> GetBothSideIndexes() => VerticlesIndex.Concat(OtherSideVerticlesIndexes);
        public IEnumerable<int> GetOneSideIndexes() => VerticlesIndex;
        public IEnumerable<Vertex> GetVertexes() => Vertices;

        public static bool operator ==(Triangle t1, int[] arrInt)
        { 
            return arrInt != null &&
                arrInt.Length == t1.VerticlesIndex.Length &
                arrInt[0] == t1.VerticlesIndex[0] && 
                arrInt[1] == t1.VerticlesIndex[1] && 
                arrInt[2] == t1.VerticlesIndex[2];
        }
        public static bool operator !=(Triangle t1, int[] arrInt) => (t1 == arrInt);
        public override int GetHashCode()
        {
            int hash = 17; 
            checked
            {
                foreach (int vert in VerticlesIndex) hash = hash * 31 + vert.GetHashCode(); 
            } 
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;
            Triangle t = (Triangle)obj;
            return t == VerticlesIndex; 
        }
    }
}
            //return verticles.Length * 22 + verticles[0] * 2 + verticles[1] * 15 + verticles[2] * 223;
