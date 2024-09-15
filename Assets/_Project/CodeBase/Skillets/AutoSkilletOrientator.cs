using System.Collections.Generic;
using SystemFunc.Transforms;
using UnityEngine;
using System.Linq;
using System.ComponentModel;

namespace Skillets
{
    [Description("Orientates the bones of a skilet to match the bones of another skilet.")]
    public class AutoSkilletOrientator : MonoBehaviour
    {
        [SerializeField] private SkilletBuilder skiletBuilder;
        public Transform transformReference;
        public Transform transformToOrientate;

        public void Orientate()
        {
            IEnumerable<Transform> bones = this.transform.GetBoneTransforms(skiletBuilder);
            IEnumerable<Transform> refBones = transformReference.GetAllTransformChilds(); 
            Dictionary<Transform, Transform> matching = bones.ToDictionary(
                bone => bone,
                bone => refBones.FirstOrDefault(refBone => refBone.name.IndexOf(bone.name, System.StringComparison.OrdinalIgnoreCase) >= 0)
                );
            matching = matching.Where(pair => pair.Value != null)
                                     .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var pair in matching)
            {
                pair.Key.localPosition = pair.Value.localPosition;
                pair.Key.localRotation = pair.Value.localRotation;
                pair.Key.localScale = pair.Value.localScale;
            } 
        }
    }
}
