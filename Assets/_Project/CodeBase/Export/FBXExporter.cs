using System.IO;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using Object = UnityEngine.Object;

public class FBXExporter : MonoBehaviour
{
    [SerializeField] private ExportModelOptions exportModelOptions;
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
        
        ModelExporter.ExportObject(exportFilePath, exportObject, exportModelOptions);
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
