using AtomEngine;
using AtomEngine.Components;
using AtomEngine.Meshes;
using MIConvexHull;
using UnityEngine; 
using System;

namespace AtomEngine
{
    [Serializable]
    public class AtomEngineTransform : AtomEngineComponent, IVertex
    {
        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public Quaternion Rotation { get; set; } 
        [field: SerializeField] public Vector3 Scale { get; set; } = Vector3.one;
        public Vector3 RotationVector { get => Rotation.eulerAngles; }
         
        public AtomEngineTransform(AtomObject parenObject) : base(parenObject)
        {
            Rotation = Quaternion.identity; 
        }
        public Vector3 Forward => RotationVector.normalized; 
        double[] IVertex.Position => new double[] { Position.x, Position.y, Position.z };

        public void Translate(Vector3 translation) => Position += translation;
        public void TranslateRotation(Quaternion quaternion) => Rotation = Rotation * quaternion;
        public void SetPosition(Vector3 position) => Position = position;
        public void SetPosition(float x, float y, float z) => Position = new Vector3(x, y, z); 
    }
}
