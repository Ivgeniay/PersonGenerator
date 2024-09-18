using UnityEngine;
using System.IO; 

using Object = UnityEngine.Object;

namespace AtomEngine
{
    public class FBXExporter : MonoBehaviour
    { 
        [SerializeField] private GameObject exportGO;
        [SerializeField] private string relativePath = "ExportedFBX";

        /// <summary>
        /// By default, the FBXExporter will export the GameObject to the Assets/ExportedFBX folder as .fbx file.
        /// </summary>
        /// <param name="exportObject"></param>
        /// <param name="_relativePath"></param>
        public void Export(Object exportObject, string _relativePath = null)
        { 
    #if UNITY_EDITOR 
            var _path = Application.dataPath;
    #elif UNITY_STANDALONE 
            var _path = Directory.GetCurrentDirectory();
    #elif UNITY_ANDROID || UNITY_IOS 
            var _path = Application.persistentDataPath; 
    #else 
            var _path = Application.dataPath;
    #endif
            if (string.IsNullOrEmpty(relativePath)) relativePath = "ExportedFBX";
            string exportFolderPath = _relativePath == null ? Path.Combine(_path, relativePath) : Path.Combine(_path, _relativePath);
            string exportFilePath = Path.Combine(exportFolderPath, exportGO.name + ".fbx");

            exportFolderPath = Path.GetFullPath(exportFolderPath);
            if (!Directory.Exists(exportFolderPath)) Directory.CreateDirectory(exportFolderPath);

    #if UNITY_EDITOR
            UnityEditor.Formats.Fbx.Exporter.ExportModelOptions exportModelOptions = new UnityEditor.Formats.Fbx.Exporter.ExportModelOptions();
            UnityEditor.Formats.Fbx.Exporter.ModelExporter.ExportObject(exportFilePath, exportObject, exportModelOptions);
    #endif
        }

        public void Export()
        {
            if (exportGO == null)
            {
                Debug.LogError("Export Object is not set");
                return;
            }
            Export(exportGO);
        }
    }

}
