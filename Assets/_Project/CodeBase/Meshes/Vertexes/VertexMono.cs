using UnityEngine.Events;
using UnityEngine;
using System;

namespace AtomEngine.Meshes
{
    public class VertexMono : MonoBehaviour
    {
        public UnityEvent<Vertex> OnVertexChangedEvent;

        [SerializeField] private Vector3 previousPosition;
        [SerializeField] private Vertex vertex;

        public Vertex Value { get
            {
                if (vertex == null)
                {
                    vertex = new Vertex(transform.position);
                    previousPosition = transform.position;
                }
                else
                {
                    if (transform.position != previousPosition)
                    {
                        previousPosition = transform.position;
                        vertex.Position = transform.position;
                        OnVertexChangedEvent?.Invoke(vertex);
                    }
                }
                return vertex;
            }
            private set
            {
                vertex = value;
                transform.position = vertex.Position;
                previousPosition = transform.position;
                OnVertexChangedEvent?.Invoke(vertex);
            }
        }
         
        private void OnEnable()
        {
            if (vertex == null)
                vertex = new Vertex(transform.position);
            
            previousPosition = transform.position;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += TrackPositionChange;
#endif
        } 
        private void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= TrackPositionChange;
#endif
        }
        private void OnValidate()
        {
            Validate();
#if UNITY_EDITOR
            if (UnityEditor.EditorGUIUtility.GetIconForObject(this) == null)
            { 
                Texture2D icon = UnityEditor.EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D;
                UnityEditor.EditorGUIUtility.SetIconForObject(this, icon); 
            }
#endif
        }
        private void TrackPositionChange() => Validate(); 
        private void Validate() {
            if (vertex == null)
            {
                Value = new Vertex(transform.position);
            }
            else
            {
                vertex.Position = transform.position;
            }
            if (transform.position != previousPosition)
            {
                previousPosition = transform.position;
                vertex.Position = transform.position;
                OnVertexChangedEvent?.Invoke(vertex); 
            }
        }

        internal void Initialize()
        {
            Validate();
        }

        public static implicit operator VertexMono(Vertex vertex)
        {
            GameObject go = new GameObject($"Vertex_{vertex.Index}");
            VertexMono vertexMono = go.AddComponent<VertexMono>();
            vertexMono.Value = vertex;
            return vertexMono;
        } 

        public static implicit operator Vertex(VertexMono vertexMono) => vertexMono.Value; 
    }
}
