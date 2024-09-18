using System.Collections.Generic;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System;

namespace AtomEngine.Meshes.Chains
{
    public class MeshChain : MonoBehaviour
    {
        [SerializeField] public List<VertexMono> Vertexes = new List<VertexMono>();
        [SerializeField][Range(1, 100)] public int VertexCount = 1;
        [SerializeField] public float Radius = 0.25f;
        [SerializeField] private int prevVertexCount = 0;
         
        private void OnEnable()
        { 
            if (Vertexes.Count == 0)
            {
                Vertexes = transform.GetComponentsInChildren<VertexMono>().ToList();
                VertexCount = Vertexes.Count;
            }
            Validate();
#if UNITY_EDITOR
            EditorApplication.update += TrackVertexCountChange;
#endif
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= TrackVertexCountChange;
#endif
        }
        private void OnValidate() => Validate(); 
        internal void SetCountVertexe(int vCount)
        {
            VertexCount = vCount;
            prevVertexCount = vCount;
            AdjustVertexCount();
            ArrangeVertexes();
        } 
        internal void SetRadius(float radius)
        {
            Radius = radius;
        }
        internal void Initialize(int vCount = 4, float radius = 0.25f)
        {
            SetRadius(radius);
            SetCountVertexe(vCount);
        }
        private void TrackVertexCountChange() => Validate();
        

        private void Validate()
        {
            Vertexes.RemoveAll(e => e == null);
            if (prevVertexCount != VertexCount)
            {
                prevVertexCount = VertexCount; 
                AdjustVertexCount();
                ArrangeVertexes();
            }
            if (VertexCount < 1) VertexCount = 1;
        }

        private void AdjustVertexCount()
        { 
            while (Vertexes.Count > VertexCount)
            {
                var vertexToRemove = Vertexes[Vertexes.Count - 1];
#if UNITY_EDITOR 
                EditorApplication.delayCall += DestroyGODelay;
                vertexMonosToDestroy.Add(vertexToRemove);
#else
                Destroy(vertexToRemove.gameObject);
#endif
                Vertexes.RemoveAt(Vertexes.Count - 1);
            }
             
            while (Vertexes.Count < VertexCount)
            { 
                VertexMono vertexMono = new GameObject($"Vertex").AddComponent<VertexMono>();
                vertexMono.transform.SetParent(transform);
                Vertexes.Add(vertexMono);

                vertexMono.Initialize();
            }
        } 

        private List<VertexMono> vertexMonosToDestroy = new List<VertexMono>();
        private void DestroyGODelay()
        {
            for (int i = 0; i < vertexMonosToDestroy.Count; i++)
            {
                if (vertexMonosToDestroy[i] != null )
                    DestroyImmediate(vertexMonosToDestroy[i].gameObject);
            }
            vertexMonosToDestroy.Clear();
            EditorApplication.delayCall -= DestroyGODelay;
        }
        private void InstantiateDelay()
        {
            VertexMono vertexMono = new GameObject($"Vertex").AddComponent<VertexMono>();
            vertexMono.transform.SetParent(transform);
            Vertexes.Add(vertexMono);
            EditorApplication.delayCall -= InstantiateDelay;
        }

        private void ArrangeVertexes()
        {
            if (VertexCount == 1)
            {
                Vertexes[0].transform.localPosition = Vector3.zero;
            }
            else
            {
                float angleStep = 360f / VertexCount;
                for (int i = 0; i < VertexCount; i++)
                {
                    float angle = i * angleStep * Mathf.Deg2Rad; 
                    Vector3 newPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Radius;
                    Vertexes[i].transform.localPosition = newPosition; 
                } 
            }

        }


        public static implicit operator Vertex[](MeshChain meshChain) => meshChain.Vertexes.Select(e => (Vertex)e).ToArray();
    }
}
