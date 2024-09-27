using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System; 
using MvLib; 

namespace AtomEngine
{
    [Serializable]
    public class MultiAtomsTransform : AtomEngineTransform
    {
        private ReactivePropertyBinder<Vector3> positionBinder;
        private ReactivePropertyBinder<Quaternion> rotationBinder;
        private ReactivePropertyBinder<Vector3> scaleBinder;
         
        [SerializeField] private List<AtomEngineTransform> atomTransforms;

        private IDisposable positionDisposable;

        public MultiAtomsTransform(params AtomEngineTransform[] atomTransforms) : base()
        {
            this.atomTransforms = atomTransforms.ToList();
        }

        public override void OnEnable()
        {
            positionBinder = new ReactivePropertyBinder<Vector3>(ReactivePosition, (e) => e.Average()); 
            positionBinder.Bind(atomTransforms.Select(e => e.ReactivePosition).ToArray());

            positionDisposable = ReactivePosition.Subscribe((e) =>
            {
                ReactivePosition.Value = e;
            });
        }

        public override void OnDisable()
        {
            positionBinder?.Dispose();
            rotationBinder?.Dispose();

            positionDisposable?.Dispose();
        }
    }
}
