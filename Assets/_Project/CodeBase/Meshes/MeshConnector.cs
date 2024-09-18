using UnityEngine; 

namespace AtomEngine.Meshes
{
    public static class MeshConnector
    {
        public static Mesh Combine(params MeshFilter[] meshes)
        {
            int counter = 0;

            CombineInstance[] combine = new CombineInstance[meshes.Length];
            foreach (MeshFilter item in meshes)
            {
                combine[counter].mesh = item.sharedMesh;
                combine[counter].transform = item.transform.localToWorldMatrix;
                counter++;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine); 

            return combinedMesh;
        }
    }
}
