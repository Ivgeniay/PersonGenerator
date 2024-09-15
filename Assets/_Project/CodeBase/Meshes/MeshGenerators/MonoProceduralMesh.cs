using System.Collections.Generic; 
using UnityEngine; 

namespace Meshes.MeshGenerators
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MonoProceduralMesh : MonoBehaviour
    {
        public const string MESH_NAME_DEFAULT = "GeneratedMesh";

        public void Generate(string meshName, IEnumerable<Vertex> vertices, MeshFilter meshFilter = null)
        {
            MeshGenerator.Generate(meshName, vertices, meshFilter ?? GetComponent<MeshFilter>());
        }
    }
}
