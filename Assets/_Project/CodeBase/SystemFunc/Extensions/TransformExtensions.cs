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

        public static IEnumerable<Transform> GetAllTransformChilds(this Transform source)
        {
            var count = source.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = source.GetChild(i);
                yield return child;
                if (child.childCount > 0)
                {
                    foreach (var e in GetAllTransformChilds(child))
                        yield return e;
                }
            }
        }
        public static void CloneHyerarhy(this Transform source, Transform destination, ref List<Transform> newTransforms)
        {
            foreach (Transform child in source)
            {
                if (newTransforms.Contains(source)) continue;

                GameObject newChild = new GameObject($"{child.name}");
                newTransforms.Add(newChild.transform); 

                newChild.transform.SetParent(destination);
                newChild.transform.localPosition = child.localPosition;
                newChild.transform.localRotation = child.localRotation;
                newChild.transform.localScale = child.localScale;

                child.CloneHyerarhy(newChild.transform, ref newTransforms);
            } 
        }
    }
}
