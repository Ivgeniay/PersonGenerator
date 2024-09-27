using MvLib.Testing.Inspector;
using UnityEditor;
using UnityEngine;
using AtomEngine;

namespace Edit.Export
{
    [CustomEditor(typeof(FBXExporter))]
    internal class FBXExporterInspector : TestedEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FBXExporter exporter = (FBXExporter)target;
            if (GUILayout.Button("Export"))
            {
                exporter.Export();
            }
        }
    }
}
