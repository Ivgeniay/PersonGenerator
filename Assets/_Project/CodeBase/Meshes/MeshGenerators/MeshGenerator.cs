using System.Collections.Generic;
using MIConvexHull; 
using System.Linq; 
using UnityEngine;

namespace AtomEngine.Meshes.MeshGenerators
{
    internal static class MeshGenerator
    {
        public const string MESH_NAME_DEFAULT = "GeneratedMesh";
        public static IEnumerable<DefaultConvexFace<Vertex>> Generate(string meshName, IEnumerable<Vertex> vertices, MeshFilter meshFilter, bool isMeshFiltedGOLocalCoordinates = false, Mesh mesh = null)
        {
            if (vertices is null)
            {
                Debug.LogError("Vertices is null");
                return null;
            }
            if (vertices.Count() < 4)
            {
                Debug.LogError("Vertexes is less than 4");
                return null;
            }
            if (meshFilter is null)
            {
                Debug.LogError("MeshFilter is null");
                return null;
            }
            List<Vertex> vertexes = vertices.ToList();
            var convexHull = ConvexHull.Create<Vertex, DefaultConvexFace<Vertex>>(vertexes);

            if (convexHull == null || convexHull.Result == null)
            {
                Debug.LogError("Не удалось создать выпуклую оболочку.");
                return null;
            } 

            var meshVertices = new List<Vector3>();
            var meshTriangles = new List<int>();
            var meshNormals = new List<Vector3>();
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
                 
                meshVertices.Add(v1);
                meshVertices.Add(v2);
                meshVertices.Add(v3);
                 
                meshTriangles.Add(vertexIndex);
                meshTriangles.Add(vertexIndex + 1);
                meshTriangles.Add(vertexIndex + 2);

                vertexIndex += 3;
            }
            
            if (mesh == null) mesh = new Mesh();
            mesh.name = string.IsNullOrEmpty(meshName) ? MESH_NAME_DEFAULT : meshName;
            mesh.vertices = meshVertices.ToArray();
            mesh.triangles = meshTriangles.ToArray();
            mesh.RecalculateNormals(); 
            meshFilter.mesh = mesh;

            return convexHull.Result.Faces;
        }
        

    }
}
