using AtomEngine.Meshes.MeshGenerators;
using System.Collections.Generic;
using UnityEngine; 

namespace AtomEngine.Meshes.Chains
{
    public class MeshChainConnector : MonoBehaviour
    {
        [SerializeField] private List<MeshChain> meshChains = new List<MeshChain>();

        public void Connect()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            if (!mf)
            {
                Debug.LogError("MeshChainConnector: MeshFilter is null");
                return;
            }
            Connect(meshChains, mf);
        }
        public void Connect(List<MeshChain> meshChains, MeshFilter meshfilter)
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
