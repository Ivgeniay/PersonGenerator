using AtomEngine.Components;
using AtomEngine.Meshes.Constructor;
using System;
using UnityEditor;
using UnityEngine;

namespace AtomEngine.SceneViews.Engine
{
    //[InitializeOnLoad]
    internal static class AtomObjectRenderer
    {
        private static Atom[] atoms;
        private static bool showHandle = false;

        static AtomObjectRenderer()
        {
            //SceneView.duringSceneGui += OnSceneGUI;
            //Selection.selectionChanged += OnSelection;
        }

        private static void OnSelection()
        {
            foreach (var obj in Selection.objects)
            { 
                if (obj is UnityEngine.GameObject go)
                {
                    ConstructedElement constructElement = go.GetComponent<ConstructedElement>();
                    if (constructElement)
                    {
                        atoms = constructElement.Atoms;
                        EnableHandle();
                    }
                    else
                    {
                        DisableHandle();
                    }
                }
            }
        }

        public static void EnableHandle()
        {
            if (!showHandle)
            {
                //SceneView.duringSceneGui += OnSceneGUI;
                showHandle = true;
                SceneView.RepaintAll();
            }
        }
        public static void DisableHandle()
        {
            if (showHandle)
            {
                //SceneView.duringSceneGui -= OnSceneGUI;
                showHandle = false;
                SceneView.RepaintAll();
            }
        }

        //private static void OnSceneGUI(SceneView sceneView)
        //{
        //    if (!showHandle) return;

        //    foreach(var atom in atoms)
        //    {
        //        //AtomEngineTransform transform = atom.GetComponent<AtomEngineTransform>();
        //        //Handles.color = Color.green;
             
        //        //Handles.SphereHandleCap(0, transform.Position, Quaternion.identity, 0.05f, EventType.Repaint);

        //        //Vector3 newPosition = Handles.PositionHandle(transform.Position, Quaternion.identity);
        //        //if (newPosition != transform.Position)
        //        //{
        //        //    //constructedElement.AtomObject_SetNewPosition(atom2, endPos + offset / 2);
        //        //    //transform.SetPosition(newPosition);
        //        //    //Debug.Log("Object moved to: " + transform.Position);
        //        //}

        //        //// Отрисовка текстовой метки рядом с объектом
        //        //Handles.Label(transform.Position + Vector3.up * 0.5f, $"Atom {atom.GetComponent<AtomEngineAtomIndex>().Index}");
        //    }
        //}
    }
}
