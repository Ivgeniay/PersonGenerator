using System.Collections.Generic;
using MIConvexHull; 
using System.Linq; 
using UnityEngine;

namespace Meshes.MeshGenerators
{
    internal static class MeshGenerator
    {
        public const string MESH_NAME_DEFAULT = "GeneratedMesh";
        public static void Generate(string meshName, IEnumerable<Vertex> vertices, MeshFilter meshFilter, bool isMeshFiltedGOLocalCoordinates = false)
        {
            if (vertices is null)
            {
                Debug.LogError("Vertices is null");
                return;
            }
            if (vertices.Count() < 4)
            {
                Debug.LogError("Vertexes is less than 4");
                return;
            }
            if (meshFilter is null)
            {
                Debug.LogError("MeshFilter is null");
                return;
            }
            List<Vertex> vertexes = vertices.ToList();
            var convexHull = ConvexHull.Create<Vertex, DefaultConvexFace<Vertex>>(vertexes);

            if (convexHull == null || convexHull.Result == null)
            {
                Debug.LogError("Не удалось создать выпуклую оболочку.");
                return;
            }

            var meshVertices = new List<Vector3>();
            var meshTriangles = new List<int>();
            int vertexIndex = 0;

            foreach (DefaultConvexFace<Vertex> face in convexHull.Result.Faces)
            {
                Vector3 v1 = face.Vertices[0];
                Vector3 v2 = face.Vertices[1];
                Vector3 v3 = face.Vertices[2];

                if (isMeshFiltedGOLocalCoordinates)
                {
                    v1 = meshFilter.gameObject.transform.InverseTransformPoint(v1);
                    v2 = meshFilter.gameObject.transform.InverseTransformPoint(v2);
                    v3 = meshFilter.gameObject.transform.InverseTransformPoint(v3);
                }

                // Добавляем вершины в список
                meshVertices.Add(v1);
                meshVertices.Add(v2);
                meshVertices.Add(v3);

                // Добавляем индексы треугольника
                meshTriangles.Add(vertexIndex);
                meshTriangles.Add(vertexIndex + 1);
                meshTriangles.Add(vertexIndex + 2);

                vertexIndex += 3;
            }

            // Создаем новый Mesh
            Mesh mesh = new Mesh();
            mesh.name = string.IsNullOrEmpty(meshName) ? MESH_NAME_DEFAULT : meshName;
            mesh.vertices = meshVertices.ToArray();
            mesh.triangles = meshTriangles.ToArray();
            mesh.RecalculateNormals(); 
            meshFilter.mesh = mesh; 
        }
    }
}
