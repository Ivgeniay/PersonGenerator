using System.Collections.Generic;

namespace AtomEngine.Meshes.Figure
{
    public interface IFigure
    {
        public IEnumerable<int> GetBothSideIndexes();
        public IEnumerable<int> GetOneSideIndexes();
        public IEnumerable<Vertex> GetVertexes();
    }
}
