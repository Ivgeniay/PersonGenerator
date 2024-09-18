using AtomEngine.Testing.Inspector;
using System.Collections.Generic;
using Color = UnityEngine.Color;
using AtomEngine.Components;
using UnityEngine; 
using System;
using System.Linq;
using UnityEngine.Rendering;
using MIConvexHull;

namespace AtomEngine.Meshes.Constructor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ConstructedElement : MonoBehaviour, IPublicMethodsInspector, IFieldsInspector
    {
        public event Action OnGenerateNewFigure;

        [NonSerialized] public AtomObject SelectedAtomObject;
        [NonSerialized] public AtomObject PreviousSelectedAtomObject;
        [NonSerialized] public List<AtomObject> SelectedAtomObjects = new();

        [SerializeField] private List<Atom> atoms = new();
        [SerializeField] private List<Edge> edges = new();
        [SerializeField] private Vector3 previousPosition = Vector3.zero;

        private MeshFilter MeshFilter;
#if UNITY_EDITOR
#region Editor
        [SerializeField] public float NonSelectedSphereRadius = .05f;
        [SerializeField] public float SelectedSphereRadius = .025f;
        [SerializeField] public Color SelectedAtomColor = Color.red;
        [SerializeField] public Color NonSelectedAtomColor = new Color(.1f, .7f, .1f, .2f);
         
        public Atom[] Atoms { get => atoms.ToArray(); }
        public Edge[] Edges { get => edges.ToArray(); }

        private Mesh mesh;
        #endregion
#endif


        private void OnEnable()
        {
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            previousPosition = transform.position;
            UnityEditor.EditorApplication.update += TrackPositionChange;
        }
        private void OnDisable() 
        {
            transform.position = previousPosition;
            UnityEditor.EditorApplication.update -= TrackPositionChange;
        }

        private void TrackPositionChange() => Validate();
        private void Validate()
        { 
            if (transform.position != previousPosition)
            {
                foreach (Atom atom in atoms)
                {
                    atom.GetComponent<AtomEngineTransform>().Translate(transform.position - previousPosition);
                }
                previousPosition = transform.position;
            }

            foreach (var edge in edges)
            {
                edge.Draw();
            }
        }

        public void GenerateCube()
        {
            SelectedAtomObject = null;
            PreviousSelectedAtomObject = null;
            SelectedAtomObjects.Clear();

            atoms = new();
            edges = new();

            foreach (Vector3 cubeVertPos in FigureConstants.CubeDefault) CreateAtom(transform.position + cubeVertPos);

            IEnumerable<DefaultConvexFace<Vertex>> faces = GenerateMesh();
            GenerateEdgesFromFaces(faces);
        }

        private void GenerateEdgesFromFaces(IEnumerable<DefaultConvexFace<Vertex>> faces)
        {
            // Генерация ребер на основе вершин граней
            foreach (var face in faces)
            {
                var vertices = face.Vertices.ToList();
                for (int i = 0; i < vertices.Count; i++)
                {
                    for (int j = i + 1; j < vertices.Count; j++)
                    {
                        Atom atom1 = atoms[vertices[i].Index];  // Вершина 1
                        Atom atom2 = atoms[vertices[j].Index];  // Вершина 2

                        // Создаем ребро, если оно еще не существует
                        if (!EdgeExists(atom1, atom2))
                        {
                            edges.Add(new Edge(atom1, atom2));
                        }
                    }
                }
            }
        } 

        private bool EdgeExists(Atom atom1, Atom atom2)
        {
            return edges.Any(e => (e.Atom == atom1 && e.Atom2 == atom2) || (e.Atom == atom2 && e.Atom2 == atom1));
        }

        private void CreateAtom(Vector3 instatiatePosition)
        {
            Atom atom = new Atom();
            atom.GetComponent<AtomEngineTransform>().SetPosition(instatiatePosition);
            atom.GetComponent<AtomEngineAtomIndex>().Index = atoms.Count;
            atoms.Add(atom);
        }
        
        private IEnumerable<DefaultConvexFace<Vertex>> GenerateMesh() =>
            MeshGenerators.MeshGenerator.Generate("kek", atoms.Select(a => (Vertex)a.GetComponent<AtomEngineTransform>()), GetComponent<MeshFilter>(), true, mesh);
        

        public void AtomMoved(Atom atom)
        {
            GenerateMesh();
        }

        public void AtomRotate(Atom atom)
        {
            Debug.Log("AtomRotate");
        }

        public void GenerateCapsule()
        {
            Debug.Log("Capsule");
        }

        public void GeneratePlane()
        {
            Debug.Log("Plane");
        }

        public void GenerateSphere()
        {
            Debug.Log("Sphere");
        }
    }

    public static class FigureConstants
    {
        public static readonly Vector3[] CubeDefault = new[] {
        new Vector3(-.5f, 0f, -.5f),
        new Vector3(-.5f, 0f, .5f),
        new Vector3(.5f, 0f, .5f),
        new Vector3(.5f, 0f, -.5f),
        new Vector3(-.5f, 1f, -.5f),
        new Vector3(-.5f, 1f, .5f),
        new Vector3(.5f, 1f, .5f),
        new Vector3(.5f, 1f, -.5f)};
    }
}
