using AtomEngine.Components; 
using MIConvexHull;
using UnityEngine; 
using System;
using MvLib.Reactive;

namespace AtomEngine
{
    [Serializable]
    public class AtomEngineTransform : AtomEngineComponent, IVertex
    {
        [SerializeReference] public ReactiveProperty<Vector3> ReactivePosition = new ReactiveProperty<Vector3>();
        [SerializeReference] public ReactiveProperty<Quaternion> ReactiveRotation = new ReactiveProperty<Quaternion>();
        [SerializeReference] public ReactiveProperty<Vector3> ReactiveScale = new ReactiveProperty<Vector3>(Vector3.one);

        public Vector3 RotationVector { get => Rotation.eulerAngles; }

        public Vector3 Position { get => ReactivePosition.Value; }
        public Quaternion Rotation { get => ReactiveRotation.Value; }
        public Vector3 Scale { get => ReactiveScale.Value; }

        public AtomEngineTransform() : base()
        {
            ReactiveRotation.Value = Quaternion.identity; 
        }
        public Vector3 Forward => RotationVector.normalized; 
        double[] IVertex.Position => new double[] { Position.x, Position.y, Position.z };

        public void Translate(Vector3 translation)
        {
            ReactivePosition.Value = ReactivePosition + translation;
        }

        public void TranslateRotation(Quaternion quaternion)
        {
            SetRotation(Rotation * quaternion);
        }

        public void SetPosition(Vector3 position)
        {
            ReactivePosition.Value = position;
        } 

        public void SetRotation(Quaternion rotation)
        {
            ReactiveRotation.Value = rotation;
        }

        public void SetScale(Vector3 scale)
        {
            ReactiveScale.Value = scale;
        }
    }
}
