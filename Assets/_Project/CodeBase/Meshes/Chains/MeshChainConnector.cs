using System.Collections.Generic; 
using Meshes.MeshGenerators; 
using UnityEngine; 

namespace Meshes.Chains
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshChainConnector : MonoBehaviour
    {
        [SerializeField] private List<MeshChain> meshChains = new List<MeshChain>();

        public void Connect()
        {
            Connect(meshChains);
        }
        public void Connect(List<MeshChain> meshChains, MeshFilter meshfilter = null)
        {
            if (meshChains == null || meshChains.Count == 0)
            {
                Debug.LogError("MeshChainConnector: MeshChain is null");
                return;
            }

            List<Vertex> vertices = new List<Vertex>();
            foreach (var meshChain in meshChains) 
                vertices.AddRange((Vertex[])meshChain);
            
            if (meshfilter == null) meshfilter = GetComponent<MeshFilter>(); 
            MeshGenerator.Generate("GeneratedMesh", vertices, meshfilter, true);
        }
    } 
}
