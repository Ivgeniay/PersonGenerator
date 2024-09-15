using System.Collections.Generic;

namespace Meshes.Figure
{
    public interface IFigure
    {
        public IEnumerable<int> GetBothSideIndexes();
        public IEnumerable<int> GetOneSideIndexes();
        public IEnumerable<Vertex> GetVertexes();
    }
}
