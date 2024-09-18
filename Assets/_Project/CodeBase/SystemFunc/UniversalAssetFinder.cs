using System.IO;
using UnityEngine;

namespace AtomEngine.SystemFunc
{
    public static class UniversalAssetFinder
    {
        /// <summary>
        /// Ищет файл по названию и расширению в Editor или Runtime.
        /// В Editor используется AssetDatabase, в Runtime — Resources и системный файловый поиск.
        /// </summary>
        /// <param name="name">Название файла (без расширения).</param>
        /// <param name="extension">Расширение файла (например, "uxml", "png").</param>
        /// <returns>Путь к файлу или null, если файл не найден.</returns>
        public static string FindAsset(string name, string extension)
        { 
#if UNITY_EDITOR
            string path = FindAssetInEditor(name, extension);
            if (path != null)
            {
                return path;
            }
#endif 
            return FindAssetInRuntime(name, extension);
        }

#if UNITY_EDITOR 
        private static string FindAssetInEditor(string name, string extension)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets(name);

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith($".{extension}", System.StringComparison.OrdinalIgnoreCase))
                {
                    return path; 
                }
            }

            Debug.Log($"Asset with name '{name}' and extension '{extension}' not found in Editor.");
            return null;
        }
#endif
         
        private static string FindAssetInRuntime(string name, string extension)
        { 
            var resourcePath = Path.Combine(name, extension);
            var resourceFile = Resources.Load(resourcePath);
            if (resourceFile != null)
            { 
                return resourcePath;
            } 
            string fullPath = Path.Combine(Application.streamingAssetsPath, $"{name}.{extension}");
            if (File.Exists(fullPath))
            { 
                return fullPath;
            } 
            return null;
        }


        public static T LoadAsset<T>(string name, string extension) where T : UnityEngine.Object
        {
            string path = FindAsset(name, extension);

            if (path == null)
            {
                Debug.LogError($"Failed to find asset with name '{name}' and extension '{extension}'");
                return null;
            }

#if UNITY_EDITOR 
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                Debug.LogError($"Failed to load asset at path: {path}");
            }
            return asset;
#else
            string resourcePath = Path.GetFileNameWithoutExtension(path);  
            T asset = Resources.Load<T>(resourcePath);
            if (asset == null)
            {
                Debug.LogError($"Failed to load asset from Resources: {resourcePath}");
            }
            return asset;
#endif
        }


    }
}
