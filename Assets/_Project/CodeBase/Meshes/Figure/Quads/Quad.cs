using AtomEngine.Meshes.Figure.Triangles;
using System.Collections.Generic; 
using System.Linq;

namespace AtomEngine.Meshes.Figure.Quads
{
    /// <summary>
    /// Квадрат рисуется исходя из направления - по часовой стрелке. 
    /// Первая точка (0, 0, 0), вторая (0, 1, 0), третья (1, 0, 0), четвертая (1, 1, 0) 
    /// </summary>
    public class Quad : IFigure
    {
        public readonly Triangle[] Triangles;

        public Quad(Triangle triangleOne, Triangle triangleTwo)
        {
            Triangles = new Triangle[2] {
                triangleOne,
                triangleTwo };
        }

        public IEnumerable<int> GetBothSideIndexes() => Triangles[0].GetBothSideIndexes().Concat(Triangles[1].GetBothSideIndexes()); 
        public IEnumerable<int> GetOneSideIndexes() => Triangles[0].GetOneSideIndexes().Concat(Triangles[1].GetOneSideIndexes());
        public IEnumerable<Vertex> GetVertexes() => Triangles[0].GetVertexes().Concat(Triangles[1].GetVertexes());
    } 
}
