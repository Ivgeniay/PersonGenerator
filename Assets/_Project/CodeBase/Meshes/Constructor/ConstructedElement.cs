using AtomEngine.Testing.Inspector;
using System.Collections.Generic;
using Color = UnityEngine.Color;
using AtomEngine.Components;
using UnityEngine;
using System;
using System.Linq;
using MIConvexHull;

namespace AtomEngine.Meshes.Constructor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class ConstructedElement : MonoBehaviour//, IPublicMethodsInspector, IFieldsInspector
    {
        public event Action OnGenerateNewFigure; 
        [SerializeField] private List<Atom> atoms = new List<Atom>();
        [SerializeField] private List<Edge> edges = new List<Edge>();
        [SerializeField] private List<Face> faces = new List<Face>();
        [SerializeField] private Vector3 previousPosition = Vector3.zero;

        private MeshFilter MeshFilter; 
        public Atom[] Atoms { get => atoms.ToArray(); }
        public Edge[] Edges { get => edges.ToArray(); }
        public AtomObject[] AtomObject { get => atoms.Cast<AtomObject>()
                .Concat(edges.Cast<AtomObject>())
                .Concat(faces.Cast<AtomObject>())
                .ToArray();}

        private Mesh mesh;
        [SerializeField]
        MarkerConfig markerConfig = new MarkerConfig()
        {
            SelectedWidth = .05f,
            NonSelectedWidth = .025f,
            SelectedColor = new Color(61, 226, 178, 150),
            NonSelecteColor = new Color32(144, 178, 255, 20),
            HoveredColor = Color.red,
            DisabledColor = new Color32(144, 178, 255, 150)
        };


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
        }

        public void GenerateCube()
        {
            atoms = new();
            edges = new();
            faces = new(); 

            foreach (Vector3 cubeVertPos in FigureConstants.CubeDefault) CreateAtom(transform.position + cubeVertPos);

            IEnumerable<DefaultConvexFace<Vertex>> _faces = GenerateMesh();
            GenerateEdgesFromFaces(_faces);
        }

        private void GenerateEdgesFromFaces(IEnumerable<DefaultConvexFace<Vertex>> faces)
        {
            foreach (var face in faces)
            {
                List<Vertex> vertices = face.Vertices.ToList();

                Face _face = new Face(vertices.Select(e => atoms[e.Index]).ToArray());
                _face.AddComponent<AtomEngineOutlineComponent>(_face, markerConfig);
                this.faces.Add(_face);

                for (int i = 0; i < vertices.Count; i++)
                {
                    for (int j = i + 1; j < vertices.Count; j++)
                    {
                        CreateEdge(vertices, i, j);
                    }
                }
            }
        }

        private void CreateEdge(List<Vertex> vertices, int i, int j)
        {
            Atom atom1 = atoms[vertices[i].Index];  // Вершина 1
            Atom atom2 = atoms[vertices[j].Index];  // Вершина 2

            if (!EdgeExists(atom1, atom2))
            {
                Edge edge = new Edge(atom1, atom2);
                edge.AddComponent<AtomEngineOutlineComponent>(edge, markerConfig);
                edges.Add(edge);
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
            atom.GetAssignableComponent<AtomEngineDistanceCheckerComponent>().ChangePointValue(instatiatePosition);
            atom.GetComponent<AtomEngineAtomIndex>().Index = atoms.Count;
            atom.AddComponent<AtomEngineOutlineComponent>(atom, markerConfig);

            atoms.Add(atom);
        }
         
        private IEnumerable<DefaultConvexFace<Vertex>> GenerateMesh()
        { 
            var vert = atoms.Select(a => (Vertex)a);
            var meshFilter = GetComponent<MeshFilter>();
            var result = MeshGenerators.MeshGenerator.Generate("Mesh", vert, meshFilter, true, mesh);
            return result;
        }

        public void TEST() => AtomMoved(null);
        public void AtomMoved(AtomObject atom)
        {
            GenerateMesh();
        }

        public void AtomRotate(AtomObject atom)
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
