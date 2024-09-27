using System.Collections.Generic; 
using AtomEngine.Components;
using AtomEngine.Meshes;
using MIConvexHull;
using System.Linq;
using UnityEngine;
using System;
using System.Reactive;

namespace AtomEngine
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class AtomConstructed : MonoBehaviour
    {
        public event Action OnGenerateNewFigure;

        [SerializeField] private List<Atom> atoms = new List<Atom>();
        [SerializeField] private List<Edge> edges = new List<Edge>();
        [SerializeField] private List<Face> faces = new List<Face>(); 
        [SerializeField] private MarkerConfig markerConfig = new MarkerConfig();

        private MeshFilter MeshFilter;
        public Atom[] Atoms { get => atoms.ToArray(); }
        public Edge[] Edges { get => edges.ToArray(); }
        public AtomObject[] AtomObject { get => atoms.Cast<AtomObject>()
                .Concat(edges.Cast<AtomObject>())
                .Concat(faces.Cast<AtomObject>())
                .ToArray(); }

        private Mesh mesh;

        private void OnEnable()
        {
            if (mesh == null) mesh = new Mesh();
        }

        public void GenerateCube()
        {
            UnsubscribeAtomsTransition();

            atoms = new();
            edges = new();
            faces = new();

            foreach (Vector3 cubeVertPos in FigureConstants.CubeDefault)
            {
                Atom atom = CreateAtom(transform.position + cubeVertPos);
                SubscribeAtomTransition(atom);
            }

            IEnumerable<DefaultConvexFace<Vertex>> _faces = GenerateMesh();
            GenerateEdgesFromFaces(_faces);
            ComponentsOnEnableInvoke();
        }

        private void GenerateEdgesFromFaces(IEnumerable<DefaultConvexFace<Vertex>> faces)
        {
            foreach (var face in faces)
            {
                List<Vertex> vertices = face.Vertices.ToList();

                Face _face = new Face(vertices.Select(e => atoms[e.Index]).ToArray());
                _face.AddComponent<AtomEngineOutlineComponent>(markerConfig);
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
            Atom atom1 = atoms[vertices[i].Index];  
            Atom atom2 = atoms[vertices[j].Index];  

            if (!EdgeExists(atom1, atom2))
            {
                Edge edge = new Edge(atom1, atom2);
                edge.AddComponent<AtomEngineOutlineComponent>(markerConfig);
                edges.Add(edge);
            }
        }
        private bool EdgeExists(Atom atom1, Atom atom2) => edges.Any(e => (e.Atom == atom1 && e.Atom2 == atom2) || (e.Atom == atom2 && e.Atom2 == atom1));
        private Atom CreateAtom(Vector3 instatiatePosition)
        {
            Atom atom = new Atom();
            atom.GetComponent<AtomEngineTransform>().SetPosition(instatiatePosition);
            atom.GetComponent<AtomEngineAtomIndex>().Index = atoms.Count;
            atom.AddComponent<AtomEngineOutlineComponent>(markerConfig);

            atoms.Add(atom);
            return atom;
        }
         
        private IEnumerable<DefaultConvexFace<Vertex>> GenerateMesh()
        { 
            var vert = atoms.Select(a => (Vertex)a);
            var meshFilter = GetComponent<MeshFilter>();
            var result = Meshes.MeshGenerators.MeshGenerator.Generate("Mesh", vert, meshFilter, true, mesh);
            return result;
        }

        private List<IDisposable> atomDisposables = new List<IDisposable>();
        public void OnDiasabled()
        { 
            ComponentsOnDisableInvoke();
            UnsubscribeAtomsTransition(); 
        }
        public void OnEnabled()
        { 
            ComponentsOnEnableInvoke(); 
            atomDisposables.Clear();
            foreach (var e in atoms)
            {
                SubscribeAtomTransition(e);
            }
        }

        private void ComponentsOnDisableInvoke()
        {
            AtomObject[] atomObjects = AtomObject;
            foreach (var atomObject in atomObjects)
            {
                List<AtomEngineComponent> components = atomObject.GetComponents<AtomEngineComponent>();
                foreach (var component in components) 
                    component.OnDisable();
            }
        }

        private void ComponentsOnEnableInvoke()
        {
            AtomObject[] atomObjects = AtomObject;
            foreach (var atomObject in atomObjects)
            {
                List<AtomEngineComponent> components = atomObject.GetComponents<AtomEngineComponent>();
                foreach (var component in components) 
                    component.OnEnable();
            }
        }

        private void UnsubscribeAtomsTransition()
        {
            atomDisposables.ForEach(e => e?.Dispose());
            atomDisposables.Clear();
        }

        private void SubscribeAtomTransition(Atom e)
        {
            IDisposable disposable = e.Transform.ReactivePosition.SubscribeWithotNotification(Observer.Create<Vector3>((position) =>
            {
                GenerateMesh();
            }));
            atomDisposables.Add(disposable);
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

        public void GenerateTorus()
        {
            Debug.Log("Torus");
        }

        public void GenerateCylinder()
        {
            Debug.Log("Cylinder");
        }
        public void GenerateCone()
        {
            Debug.Log("Cone");
        }

        public void MakeEdgeAtom()
        {
            
        }

        public void MakeNewAtom()
        {
            
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
