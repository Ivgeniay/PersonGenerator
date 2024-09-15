﻿using UnityEditor;
using UnityEngine;

namespace Edit.Export
{
    [CustomEditor(typeof(FBXExporter))]
    internal class FBXExporterInspector : Editor
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
