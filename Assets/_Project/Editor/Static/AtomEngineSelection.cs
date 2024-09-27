using UnityEditor;
using AtomEngine;
using System;
using System.Collections.Generic;
using AtomEngine.SceneViews;
using AtomEngine.Components;
using UnityEngine;
using System.Linq;

using Edge = AtomEngine.Edge;
using static AtomEngine.SceneViews.SceneToolsOverlay;
using UnityEditor.TerrainTools;

[InitializeOnLoad]
public static class AtomEngineSelection
{
    public static Action selectionChanged;

    private static List<AtomObject> selectedAtomObjects = new();
    public static IEnumerable<AtomObject> SelectedAtomObjects { get => selectedAtomObjects; } 
    private static bool isUpdate { get => SelectionWatcher.atomConstructed; }
    private static ModeType _modeType;
    private static ToolsType _toolsType;

    static AtomEngineSelection() 
    {
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload; 
         
        EditorApplication.update += OnEditorUpdate;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    { 
        if (!isUpdate) return;
        if (_modeType == ModeType.None) return;

        Event e = Event.current;
        if (e == null) return;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            bool isMultiSelect = e.shift || e.control;
            Type workingType = null;
            switch (_modeType)
            {
                case ModeType.None: return;
                case ModeType.Object: return;
                case ModeType.Atom: workingType = typeof(Atom); break;
                case ModeType.Edge: workingType = typeof(Edge); break;
                case ModeType.Face: workingType = typeof(Face); break;
            }

            IEnumerable<AtomObject> r = SelectionWatcher.atomConstructed.AtomObject.Where(g => g.GetType() == workingType);
            IEnumerable<AtomObject> sortedObjects = SortAtomObjectByDistanceToCamera(r);

            bool isStop = false;
            foreach (var atomObject in sortedObjects)
            {
                if (isStop) break;
                var checker = atomObject.GetAssignableComponent<AtomEngineDistanceCheckerComponent>();

                if (checker.CheckDistance(Event.current.mousePosition))
                {
                    if (isMultiSelect)
                    {
                        if (selectedAtomObjects.Contains(atomObject)) SetUnselect(atomObject);
                        else SetSelect(atomObject);
                    }
                    else
                    {
                        selectedAtomObjects.Clear();
                        SetSelect(atomObject);
                    }
                    isStop = true;
                    e.Use();
                }
            }

        }

    }

    private static void OnEditorUpdate()
    { 
        if (!isUpdate) return;
    }

    private static void OnAfterAssemblyReload() {  }

    private static void OnBeforeAssemblyReload()
    {
        selectedAtomObjects.Clear(); 
        SetSelectedObjects(selectedAtomObjects);
    }  
    private static void SetSelectedObjects(List<AtomObject> atomObjects)
    {
        selectedAtomObjects = atomObjects;
        selectionChanged?.Invoke();
    } 
    private static void SetSelect(AtomObject atomObject)
    {
        selectedAtomObjects.Add(atomObject);
        selectionChanged?.Invoke();
    } 
    private static void SetUnselect(AtomObject atomObject)
    {
        selectedAtomObjects.Remove(atomObject);
        selectionChanged?.Invoke();
    } 
    private static void UnSelectAll()
    {
        selectedAtomObjects.Clear();
        selectionChanged?.Invoke();
    } 
     
    
    private static IEnumerable<AtomObject> SortAtomObjectByDistanceToCamera(IEnumerable<AtomObject> atomObjects)
    {
        if (atomObjects == null || atomObjects.Count() == 0) return atomObjects;
          
        if (atomObjects.First() is Atom atom) return SortAtomsByDistanceToCamera(atomObjects.Cast<Atom>());
        else if (atomObjects.First() is Edge edge) return SortEdgesByDistanceToCamera(atomObjects.Cast<Edge>());
        else if (atomObjects.First() is Face face) return SortFacesByDistanceToCamera(atomObjects.Cast<Face>());

        return atomObjects;
    }
    private static IEnumerable<Atom> SortAtomsByDistanceToCamera(IEnumerable<Atom> atoms)
    {
        var camera = SceneView.lastActiveSceneView.camera;
        return atoms.OrderBy(atom => Vector3.Distance(camera.transform.position, atom.Transform.Position)).ToList();
    }
    private static IEnumerable<Edge> SortEdgesByDistanceToCamera(IEnumerable<Edge> edges)
    {
        var camera = SceneView.lastActiveSceneView.camera;

        return edges.OrderBy(edge =>
        {
            Vector3 midPoint = (edge.Atom.Transform.Position + edge.Atom2.Transform.Position) / 2;
            return Vector3.Distance(camera.transform.position, midPoint);
        });
    }
    private static IEnumerable<Face> SortFacesByDistanceToCamera(IEnumerable<Face> faces)
    {
        var camera = SceneView.lastActiveSceneView.camera;
        return faces.OrderBy(face =>
        {
            Vector3 centroid = Vector3.zero;
            foreach (var atom in face.Atoms)
            {
                centroid += atom.Transform.Position;
            }
            centroid /= face.Atoms.Length;
            return Vector3.Distance(camera.transform.position, centroid);
        });
    }
    
    internal static void SwitchModeTools(SceneToolsOverlay.ModeType modeType) => _modeType = modeType; 
    internal static void SwitchToolsType(SceneToolsOverlay.ToolsType toolsType) => _toolsType = toolsType;
}
