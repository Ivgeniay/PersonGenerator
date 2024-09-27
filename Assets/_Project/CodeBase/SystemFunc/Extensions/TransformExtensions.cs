using System.Collections.Generic; 
using System.Reflection;
using UnityEngine;

namespace AtomEngine
{
    public static class TransformExtensions
    {
        public static IEnumerable<Transform> GetBoneTransforms(this Transform transform, object obj)
        {
            var fields = obj.GetType()
                .GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.Name == "RootPerson") continue;
                if (field.FieldType == typeof(Transform) && field.GetValue(obj) != null)
                    yield return (Transform)field.GetValue(obj);
            }
        }
    }
}
