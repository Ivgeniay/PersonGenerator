using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Test
{
    internal class Section : MonoBehaviour
    {
        [SerializeField] public List<Transform> Points;

        private void Awake()
        {
            Points = GetComponentsInChildren<Transform>(true).ToList();
            Points.Remove(this.transform);
            Debug.Log(Points.Count);
        }
    }
}
