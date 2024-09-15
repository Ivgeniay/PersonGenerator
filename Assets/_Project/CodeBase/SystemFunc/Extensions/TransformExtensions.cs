using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SystemFunc.Transforms
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

        public static IEnumerable<Transform> GetAllTransformChilds(this Transform parent)
        {
            var count = parent.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = parent.GetChild(i);
                yield return child;
                if (child.childCount > 0)
                {
                    foreach (var e in GetAllTransformChilds(child))
                        yield return e;
                }
            }
        }

    }
}
